using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Android;

namespace Game.MiniGame.PowerCheck
{
    public class FXManager : MonoBehaviour
    {
        private MiniGamePlayer agent;

        private ParticleSystem AttackEffect;
        private ParticleSystem TrapEffect;
        private ParticleSystem SpeedupEffect;
        private ParticleSystem HealingEffect;

        public void Initialize(MiniGamePlayer agent)
        {
            this.agent = agent;

            AttackEffect = FindEffect("AttackEffect");
            TrapEffect = FindEffect("TrapEffect");
            SpeedupEffect = FindEffect("SpeedupEffect");
            HealingEffect = FindEffect("HealingEffect");

            SetupEffects();
        }

        #region Finding effects
        private ParticleSystem FindEffect(string effectName)
        {
            if (agent == null)
            {
                Debug.LogWarning($"FXManager: agent is null, cannot find {effectName}");
                return null;
            }

            Transform effectTransform = FindChildRecursive(agent.transform, effectName);

            if (effectTransform != null)
            {
                //Debug.Log($"FXManager: Found {effectName} for agent {agent.name}");

                ParticleSystem ps = effectTransform.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    Debug.LogWarning($"FXManager: {effectName} does not have ParticleSystem component");
                }
                return ps;
            }
            else
            {
                Debug.LogWarning($"FXManager: {effectName} not found in children of {agent.name}");
                return null;
            }
        }

        private Transform FindChildRecursive(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                Transform found = FindChildRecursive(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        #endregion

        #region Effects logic
        private void SetupEffects()
        {
            SetupParticleSystem(AttackEffect);
            SetupParticleSystem(TrapEffect);
            SetupParticleSystem(SpeedupEffect);
            SetupParticleSystem(HealingEffect);
        }

        private void SetupParticleSystem(ParticleSystem ps)
        {
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                var main = ps.main;
                main.playOnAwake = false;

                ps.Clear(true);
            }
        }

        public void PlayAttackEffect()
        {
            PlayEffect(AttackEffect, "AttackEffect");
        }

        public void PlaySttopedEffect()
        {
            PlayEffect(TrapEffect, "SttopedEffect");
        }

        public void PlayBuffedEffect()
        {
            PlayEffect(SpeedupEffect, "BuffedEffect");
        }

        public void PlayHealingEffect()
        {
            PlayEffect(HealingEffect, "HealingEffect");
        }

        private void PlayEffect(ParticleSystem effect, string effectName)
        {
            if (effect != null)
            {
                //if (!effect.gameObject.activeInHierarchy)
                //    effect.gameObject.SetActive(true);

                //var emission = effect.emission;
                //emission.enabled = true;

                //var renderer = effect.GetComponent<ParticleSystemRenderer>();
                //if (renderer != null) renderer.enabled = true;

                //effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                effect.Play();


                //Debug.Log($"FXManager: Playing {effectName}");
            }
            else
            {
                Debug.LogWarning($"FXManager: Cannot play {effectName} - effect is null");
            }
        }

        #endregion
    }
}