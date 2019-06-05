using HackConsole;
using HackConsole.Algo;
using SurvivalHack.Combat;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalHack
{
    static public class Eventing
    {
        public static bool On(BaseEvent evt, BaseEvent parent = null)
        {
            // Prepare event, filling in PreEvent, OnEvent, PostEvent, etc
            foreach (var c in evt.Item.Components)
                (c as IActionComponent)?.GetActions(evt.Item, evt, EUseSource.Item);

            if (evt.Target != null)
            {
                foreach (var c in evt.Target.Components)
                    (c as IActionComponent)?.GetActions(evt.Target, evt, EUseSource.Target);
                foreach (var e in evt.Target.ListSubEntities())
                    foreach (var c in e.Components)
                        (c as IActionComponent)?.GetActions(e, evt, EUseSource.TargetItem);
            }

            if (evt.User != null)
            {
                foreach (var c in evt.User.Components)
                    (c as IActionComponent)?.GetActions(evt.User, evt, EUseSource.User);
                foreach (var e in evt.User.ListSubEntities())
                    foreach (var c in e.Components)
                        (c as IActionComponent)?.GetActions(e, evt, EUseSource.UserItem);
            }

            // == Execute the event ==
            //evt.PreEventCheck?.Invoke(evt);
            foreach (var check in evt.OnEventCheck)
            {
                var str = check.Invoke(evt);
                if (str != null)
                {
                    ColoredString.Write(str.CleanUp());
                    return false;
                }
            }
            evt.PreEvent?.Invoke(evt);
            evt.OnEvent?.Invoke(evt);
            evt.PostEvent?.Invoke(evt);

            // == Logging ==
            var message = evt.GetMessage(parent != null) + evt.PostMessage;

            if (parent != null)
				parent.PostMessage += message;
			else
				ColoredString.Write(message.CleanUp()); //TODO: Color

            return true;
        }
    }

    public abstract class BaseEvent
    {
        public Entity User;
        public Entity Item;
        public Entity Target;

        //public Action<BaseEvent> PreEventCheck;
        public List<Func<BaseEvent, string>> OnEventCheck = new List<Func<BaseEvent, string>>();
        public Action<BaseEvent> PreEvent;
        public Action<BaseEvent> OnEvent;
        public Action<BaseEvent> PostEvent;

        public BaseEvent(Entity user, Entity item, Entity target)
        {
            User = user;
            Item = item;
            Target = target;
        }

        public BaseEvent(BaseEvent parent)
        {
            User = parent.User;
            Item = parent.Item;
            Target = parent.Target;
        }

        public abstract String GetMessage(bool isChildMessage);
		
		///<summary>Used in Eventing.On() for the order of events. You typically won't need to manually write to this.</summary>
		public string PostMessage;
    }

    public class ConsumeEvent : BaseEvent
    {
        public ConsumeEvent(Entity user, Entity item) : base(user, item, user)
        {
        }

        public override string GetMessage(bool isChildMessage)
        {
            // TODO: drinks
            return $"{Word.Name(User)} {Word.Verb(User, "consume")} {Word.AName(Item)}. ";
        }
    }

    public class CastEvent : BaseEvent
    {
        public CastEvent(Entity user, Entity item) : base(user, item, user)
        {
        }

        public override string GetMessage(bool isChildMessage)
        {
            // TODO: casts
            return $"{Word.Name(User)} cast {Word.AName(Item)}. ";
        }
    }

    public class ThreatenEvent : BaseEvent
    {
        public ThreatenEvent(Entity user, Entity target) : base(user, null, target)
        {
        }

        public override string GetMessage(bool isChildMessage)
        {
            if (Target.EntityFlags.HasFlag(EEntityFlag.IsPlayer))
                return $"{Word.AName(User)} spotted {Word.AName(Target)}. ";
            else
                return "";
        }
    }

    public class AttackEvent : BaseEvent
    {
        public EAttackState State = EAttackState.Hit;
        public EAttackMove Move;
        public EDamageLocation Location;

        public AttackEvent(Entity user, Entity weapon, Entity target, EAttackMove move) : base(user, weapon, target)
        {
            Move = move;
            Location = GetRandomLocation();
        }

        private EDamageLocation GetRandomLocation()
        {
            var rnd = Game.Rnd.NextDouble();
            if (rnd < 0.75)
                return EDamageLocation.Body;
            else if (rnd < 0.9)
                return EDamageLocation.Head;
            else if (rnd < 0.95)
                return EDamageLocation.Hands;
            else
                return EDamageLocation.Feet;
        }

        private string LocationToString(EDamageLocation loc)
        {
            switch (loc) {
                case EDamageLocation.Body:
                    return "torso";
                case EDamageLocation.Head:
                    return "head";
                case EDamageLocation.Hands:
                    return "hand";
                case EDamageLocation.Feet:
                    return "foot";
                case EDamageLocation.LArm:
                    return "left arm";
                case EDamageLocation.RArm:
                    return "right arm";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public override string GetMessage(bool isChildMessage)
        {
            StringBuilder sb = new StringBuilder();

            if (Item == User)
            {
                sb.Append($"{Word.AName(User)} {Word.Verb(User, "attack")} {Word.AName(Target)}");
            }
            else
            {
                // TODO: Verb 'swing' should be based on the actual attack.

                sb.Append($"{Word.AName(User)} {Word.Verb(User, "swing")} {Word.Its(User)} {Word.Name(Item)} at {Word.AName(Target)}");
            }

            if (State == EAttackState.Hit)
            {
                sb.Append($" and {Word.Verb(User, "hit")} {Word.Its(Target)} {LocationToString(Location)}. ");
            }
            else if (State == EAttackState.Miss)
            {
                sb.Append($" but {Word.Verb(User, "miss", "misses")} the attack. ");
            }
            else
            {
                var verbs = new string[,] { { null, null }, { null, null }, { "dodge", null }, { "block", null }, { "parry", "parries" } };
                var verb = Word.Verb(Target, verbs[(int)State, 0], verbs[(int)State, 1]);

                //TODO: What if I add hooking.
                sb.Append($" but {verb} the attack. No damage is dealt. ");
            }
            
            return sb.ToString();
        }
    }

    public class DamageEvent : BaseEvent
    {
        public readonly int BaseDamage;
        public EDamageType DamageType;
        public EDamageLocation Location;
        //public List<(double, string)> PreMults = new List<(double, string)>();
        public List<(int, string)> Modifiers = new List<(int, string)>();
        //public List<(double, string)> PostMults = new List<(double, string)>();
        public bool KillHit = false;

        public int Damage {
            get {
                double dmg = BaseDamage;
                //foreach ((var d, var s) in PreMults) dmg *= (d+1);
                foreach ((var i, var s) in Modifiers) dmg += i;
                //foreach ((var d, var s) in PostMults) dmg *= (d+1);

                return (int)(Math.Max(dmg + 0.5, 0));
            }
        }
        public bool Significant => (Damage > 0);

        public DamageEvent(BaseEvent parentEvent, int damage, EDamageType damageType, EDamageLocation location) : base (parentEvent)
        {
            BaseDamage = damage;
            DamageType = damageType;
            Location = location;
        }

        public override string GetMessage(bool isChildMessage)
        {
            var dmg = Damage;

            StringBuilder sb = new StringBuilder();
            sb.Append($"{(isChildMessage ? Word.It(Target) : Word.AName(Target))} {Word.Verb(Target, "take")}");
            if (dmg > 0)
                sb.Append($" @cd{dmg}@ca damage");
            else
                sb.Append($" @cfNo@ca damage");
            sb.Append(KillHit ? $" killing {(Word.It(Target))}." : ". ");
            
            if (Modifiers.Count > 0)
            {
                string adds = string.Join("", Modifiers.Select(p => $" {(p.Item1 >= 0 ? "+" : "-")} {Math.Abs(p.Item1)} {p.Item2}"));
                string formula = ($"({BaseDamage}{adds}). ");
                sb.Append(formula);
            }

            return sb.ToString();
        }
    }

    public class GrabEvent : BaseEvent
    {
        public GrabEvent(Entity user, Entity item, Entity target) : base(user, item, target)
        {
        }

        public override string GetMessage(bool isChildMessage)
        {
            return $"{Word.AName(User)} {Word.Verb(User, "grab")} {Word.AName(Item)}. ";
        }
    }

    public class IdentifyEvent : BaseEvent
    {
        public IdentifyEvent(BaseEvent parent) : base(parent)
        {
        }

        public override string GetMessage(bool isChildMessage)
        {
            return $"It is {Word.AName(Item)}. ";
        }
    }

    public class DownEvent : BaseEvent
    {
        public string Method = "";

        public DownEvent(Entity user, Entity stairs) : base(user, stairs, null)
        {
        }

        public override string GetMessage(bool isChildMessage)
        {
            return $"{Word.AName(User)} {Word.Verb(User, "walk")} down the {Method}. ";
        }
    }
}
