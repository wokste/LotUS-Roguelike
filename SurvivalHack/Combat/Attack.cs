using System;

namespace SurvivalHack.Combat
{
    public enum EAttackResult
    {
        HitNoDamage, HitDamage, HitKill, Miss, Dodge, Blocked, Parried
    }

    [Flags]
    public enum EDamageType
    {
        Blunt = 1,
        Pierce = 2,
        Cut = 4,
        Poison = 8,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }
}
