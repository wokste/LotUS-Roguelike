using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Diagnostics;
using System.Linq;

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

    public enum EAttackState
    {
        Hit, Miss, Blocked, Parried
    }

    [Flags]
    public enum EDamageType
    {
        Bludgeoing = 1,
        Piercing = 2,
        Slashing = 4,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }

    public enum EDamageLocation
    {
        Head,
        Body,
        LArm,
        RArm,
        Legs,
        Wings,
        Tail,
    }
}
