using System;

namespace SurvivalHack.Combat
{

    public enum EAttackMove
    {
        /// <summary>Bows Etc.</summary>
        Projectile = 1,

        /// <summary>Any sweeping blows, including axes and maces. Swords can be either swing or thrust.</summary>
        Swing = 2,

        /// <summary>Any thrusting blows, including spears. Swords can be either swing or thrust.</summary>
        Thrust = 3,

        /// <summary>Anything which requires to be very close. Bite attacks, mind flayers extraction, backstab dagger attacks, etc.</summary>
        Close = 4,
    }

    public enum EAttackResult
    {
        HitNoDamage, HitDamage, HitKill, Miss, Dodge, Blocked, Parried
    }

    [Flags]
    public enum EDamageType
    {
        Bludgeoing = 1,
        Piercing = 2,
        Slashing = 4,
        Poison = 8,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }
}
