using HackConsole;
using SurvivalHack.Combat;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    static class Eventing
    {
        public static bool On(BaseEvent evt, BaseEvent parent = null)
        {
            var funcs = new List<UseFunc>();
            funcs.AddRange(evt.Item.Components.SelectMany(c => c.GetActions(evt.Item, evt, EUseSource.This)));

            if (evt.Target != null)
            {
                funcs.AddRange(evt.Target.Components.SelectMany(c => c.GetActions(evt.Target, evt, EUseSource.Target)));
                funcs.AddRange(evt.Target.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(e, evt, EUseSource.TargetItem))));
            }

            if (evt.User != null)
            {
                funcs.AddRange(evt.User.Components.SelectMany(c => c.GetActions(evt.User, evt, EUseSource.User)));
                funcs.AddRange(evt.User.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(e, evt, EUseSource.UserItem))));
            }

            if (!funcs.Any(f => f.Order == EUseOrder.Event))
            {
                return false;
            }

            if (funcs.Any(f => f.Order == EUseOrder.Interrupt))
            {
                return false;
            }

            foreach (var f in funcs.OrderBy(f => f.Order))
            {
                f.Action?.Invoke(evt);
            }

            var message = $"{evt.GetMessage((parent != null))} {evt.PostMessage}";

            if (parent != null)
				parent.PostMessage = $"{parent.PostMessage} {message}";
			else
				ColoredString.Write(message, Color.Pink); //TODO: Color

            return true;
        }
    }

    public abstract class BaseEvent
    {
        public Entity User;
        public Entity Item;
        public Entity Target;

        public abstract String GetMessage(bool isChildMessage);
		
		///<summary>Used in Eventing.On() for the order of events. You typically won't need to manually write to this.</summary>
		public string PostMessage;
    }

    public class DrinkEvent : BaseEvent
    {
        public DrinkEvent(Entity self, Entity item)
        {
            User = self;
            Item = item;
        }

        public override string GetMessage(bool isChildMessage)
        {
            // TODO: drinks
            return $"{Word.Name(User)} drink {Word.AName(Item)}";
        }
    }

    public class CastEvent : BaseEvent
    {
        public CastEvent(Entity self, Entity item)
        {
            User = self;
            Item = item;
        }

        public override string GetMessage(bool isChildMessage)
        {
            // TODO: casts
            return $"{Word.Name(User)} cast {Word.AName(Item)}";
        }
    }

    public class ThreatenEvent : BaseEvent
    {
        public ThreatenEvent(Entity self, Entity target)
        {
            User = self;
            Target = target;
        }

        public override string GetMessage(bool isChildMessage)
        {
            if (Target.EntityFlags.HasFlag(EEntityFlag.IsPlayer))
                return $"{Word.AName(User)} spotted you.";
            else
                return null;
        }
    }

    public class AttackEvent : BaseEvent
    {
        public EAttackState State = EAttackState.Hit;
        public EAttackMove Move;
        public EDamageLocation Location;

        public AttackEvent(Entity self, Entity weapon, Entity target, EAttackMove move)
        {
            User = self;
            Item = weapon;
            Target = target;
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
                var location = "the stomach"; // TODO: Actual locations.
                sb.Append($" and {Word.Verb(User, "hit")} {Word.It(Target)} in {location}.");
            }
            else if (State == EAttackState.Miss)
            {
                sb.Append($" but {Word.Verb(User, "miss", "misses")} the attack.");
            }
            else
            {
                var verbs = new string[,] { { null, null }, { null, null }, { "dodge", null }, { "block", null }, { "parry", "parries" } };
                var verb = Word.Verb(Target, verbs[(int)State, 0], verbs[(int)State, 1]);

                //TODO: What if I add hooking.
                sb.Append($" but {verb} the attack. No damage is dealt.");
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

        public DamageEvent(BaseEvent prevMessage, int damage, EDamageType damageType, EDamageLocation location)
        {
            User = prevMessage.User;
            Item = prevMessage.Item;
            Target = prevMessage.Target;
            BaseDamage = damage;
            DamageType = damageType;
            Location = location;
        }

        public override string GetMessage(bool isChildMessage)
        {
            var dmg = Damage;

            StringBuilder sb = new StringBuilder();
            sb.Append($"{(isChildMessage ? Word.It(Target) : Word.AName(Target))} {Word.Verb(Target, "take")} {(dmg > 0 ? dmg.ToString() : "no") } damage");
            sb.Append(KillHit ? $" killing {(Word.It(Target))}." : ".");
            
            if (Modifiers.Count > 0)
            {
                string adds = string.Join("", Modifiers.Select(p => $" {(p.Item1 >= 0 ? "+" : "-")} {Math.Abs(p.Item1)} {p.Item2}"));
                string formula = ($"({BaseDamage}{adds})");
                sb.Append(formula);
            }

            return sb.ToString();
        }
    }
}
