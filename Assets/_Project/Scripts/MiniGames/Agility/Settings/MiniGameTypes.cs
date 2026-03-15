using System;

public enum MiniGameState
{
    Idle,
    Countdown,
    SafePhase,
    Running,
    Win,
    Lose
}

public enum PatternTier
{
    Easy,
    Medium,
    Hard
}

[Flags]
public enum PatternTag
{
    None = 0,
    Body = 1 << 0,
    Projectile = 1 << 1,
    Surface = 1 << 2,
    Formation = 1 << 3,
    Summon = 1 << 4,
    Environment = 1 << 5,

    Constrain = 1 << 10,   // Узагон/перекрытиеФ
    Slow = 1 << 11,        // замедление
    Slip = 1 << 12,        // скольжение
    Trap = 1 << 13,        // ловушка/замок геометрии
}
