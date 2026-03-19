using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RunPlanGenerator
{
    public static RunPlan Generate(
        int dex,
        int seed,
        MiniGameConfig config,
        DexDifficultyProfile profile,
        PatternDatabase db)
    {
        var scriptedActs = TryGenerateScriptedActs(dex, config, profile, db);
        if (scriptedActs != null)
            return scriptedActs;

        var plan = new RunPlan();
        var rng = new System.Random(seed);

        float dex01 = profile.NormalizeDex(dex);

        float easyShare = profile.easyShare.Evaluate(dex01);
        float mediumShare = profile.mediumShare.Evaluate(dex01);
        float hardShare = profile.hardShare.Evaluate(dex01);

        float maxBudget = profile.maxIntensityBudget.Evaluate(dex01);
        float comboChance = profile.comboChance.Evaluate(dex01);

        int slots = Mathf.Max(1, Mathf.FloorToInt(config.runDuration / config.slotSeconds));

        PatternDefinition? last = null;
        PatternTag recentTags = PatternTag.None;
        PatternTier lastTier = PatternTier.Easy;

        float time = 0f;

        for (int i = 0; i < slots; i++)
        {
            if (time >= config.runDuration)
                break;

            // “Кривая сложности по времени”: ближе к концу повышаем вероятность Medium/Hard
            float t01 = slots <= 1 ? 1f : i / (float)(slots - 1);
            (float e, float m, float h) = TimeBias(easyShare, mediumShare, hardShare, t01);

            var targetTier = RollTier(rng, e, m, h);

            // Выбираем 1 паттерн
            var p1 = PickPattern(rng, db, dex, targetTier, last, recentTags, maxBudget, 0f);
            if (p1 == null) break;

            Add(plan, p1, ref time, config.slotSeconds, config.runDuration);
            last = p1;
            recentTags = p1.tags;
            lastTier = p1.tier;

            // Опционально: связка (2 паттерна), если позволяет бюджет
            bool wantCombo = rng.NextDouble() < comboChance && p1.tier != PatternTier.Hard;
            if (wantCombo && time < config.runDuration)
            {
                var p2 = PickPattern(rng, db, dex, targetTier, last, recentTags, maxBudget, p1.intensity);
                if (p2 != null)
                {
                    Add(plan, p2, ref time, config.slotSeconds * 0.5f, config.runDuration);
                    last = p2;
                    recentTags = p2.tags;
                    lastTier = p2.tier;
                }
            }

            if (time >= config.runDuration)
                break;

            if (lastTier >= PatternTier.Medium && rng.NextDouble() < profile.restFrequency.Evaluate(dex01))
            {
                float restSeconds = Mathf.Lerp(config.slotSeconds * 0.2f, config.slotSeconds * 0.05f, dex01);
                time = Mathf.Min(config.runDuration, time + restSeconds);
            }
        }

        return plan;
    }

    private static void Add(RunPlan plan, PatternDefinition p, ref float time, float slotSeconds, float maxRunDuration)
    {
        float dur = Mathf.Min(p.duration + p.cooldownAfter, slotSeconds);
        dur = Mathf.Min(dur, Mathf.Max(0f, maxRunDuration - time));
        if (dur <= 0f) return;

        var item = new RunPlanItem
        {
            pattern = p,
            startTime = time,
            endTime = time + dur
        };
        plan.items.Add(item);
        time += dur;
    }

    private static (float e, float m, float h) TimeBias(float e, float m, float h, float t01)
    {
        // Небольшой дрейф: чем ближе к концу, тем меньше easy и больше hard.
        float drift = Mathf.Lerp(0f, 0.25f, t01);
        e = Mathf.Clamp01(e - drift);
        h = Mathf.Clamp01(h + drift);
        // m оставляем как есть, потом нормализуем
        float sum = e + m + h;
        if (sum <= 0.0001f) return (0.33f, 0.33f, 0.34f);
        return (e / sum, m / sum, h / sum);
    }

    private static PatternTier RollTier(System.Random rng, float e, float m, float h)
    {
        double r = rng.NextDouble();
        if (r < e) return PatternTier.Easy;
        if (r < e + m) return PatternTier.Medium;
        return PatternTier.Hard;
    }

    private static PatternDefinition? PickPattern(
        System.Random rng,
        PatternDatabase db,
        int dex,
        PatternTier tier,
        PatternDefinition? last,
        PatternTag recentTags,
        float maxBudget,
        float alreadyUsedBudget)
    {
        var candidates = db.All
            .Where(p => p != null)
            .Where(p => p.tier == tier)
            .Where(p => dex >= p.minDex && dex <= p.maxDex)
            .Where(p => last == null || p != last)
            .Where(p => (p.forbiddenWithTags & recentTags) == 0)
            .Where(p => (alreadyUsedBudget + p.intensity) <= maxBudget)
            .ToList();

        if (candidates.Count == 0)
        {
            // fallback: разрешаем tier ниже
            candidates = db.All
                .Where(p => p != null)
                .Where(p => dex >= p.minDex && dex <= p.maxDex)
                .Where(p => last == null || p != last)
                .Where(p => (p.forbiddenWithTags & recentTags) == 0)
                .Where(p => (alreadyUsedBudget + p.intensity) <= maxBudget)
                .OrderBy(p => p.tier) // Easy сначала
                .ToList();
        }

        if (candidates.Count == 0) return null;

        // Взвешенный выбор по dex
        float dex01 = Mathf.InverseLerp(1f, 10f, dex); // если у вас другой диапазон — замените
        var weights = new float[candidates.Count];
        float total = 0f;
        for (int i = 0; i < candidates.Count; i++)
        {
            float w = Mathf.Max(0.001f, candidates[i].weightByDex.Evaluate(dex01));
            weights[i] = w;
            total += w;
        }

        double roll = rng.NextDouble() * total;
        float acc = 0f;
        for (int i = 0; i < candidates.Count; i++)
        {
            acc += weights[i];
            if (roll <= acc) return candidates[i];
        }

        return candidates[^1];
    }

    private static RunPlan TryGenerateScriptedActs(
        int dex,
        MiniGameConfig config,
        DexDifficultyProfile profile,
        PatternDatabase db)
    {
        var plus = db.All.FirstOrDefault(p => p != null && p.id == "AGILITY_ACT_PLUS");
        var cross = db.All.FirstOrDefault(p => p != null && p.id == "AGILITY_ACT_CROSS");
        var orbit = db.All.FirstOrDefault(p => p != null && p.id == "AGILITY_ACT_ORBIT");

        if (plus == null || cross == null || orbit == null)
            return null;

        float dex01 = profile != null ? profile.NormalizeDex(dex) : Mathf.InverseLerp(1f, 10f, dex);
        float phaseDuration = Mathf.Max(1f, config.runDuration / 3f);

        var plan = new RunPlan();
        float startTime = 0f;

        PatternDefinition[] sourcePatterns = { plus, orbit, cross };
        for (int i = 0; i < sourcePatterns.Length; i++)
        {
            PatternDefinition source = sourcePatterns[i];
            float segmentDuration = (i == sourcePatterns.Length - 1) ? config.runDuration - startTime : phaseDuration;
            float telegraph = Mathf.Min(source.telegraphTime, segmentDuration * 0.24f);
            float activeDuration = Mathf.Max(0.85f, segmentDuration - telegraph);

            var timedPattern = UnityEngine.Object.Instantiate(source);
            timedPattern.hideFlags = HideFlags.DontSave;
            timedPattern.duration = activeDuration;
            timedPattern.telegraphTime = telegraph;
            timedPattern.cooldownAfter = 0f;
            timedPattern.intensity = Mathf.Clamp01(source.intensity + dex01 * (0.08f + i * 0.04f));

            plan.items.Add(new RunPlanItem
            {
                pattern = timedPattern,
                startTime = startTime,
                endTime = Mathf.Min(config.runDuration, startTime + segmentDuration)
            });

            startTime += segmentDuration;
        }

        return plan;
    }
}
