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
        public static bool On(BaseEvent evt)
        {
            var funcs = new List<UseFunc>();
            funcs.AddRange(evt.Item.Components.SelectMany(c => c.GetActions(evt, EUseSource.This)));

            if (evt.Target != null)
            {
                funcs.AddRange(evt.Target.Components.SelectMany(c => c.GetActions(evt, EUseSource.Target)));
                funcs.AddRange(evt.Target.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(evt, EUseSource.TargetItem))));
            }

            if (evt.User != null)
            {
                funcs.AddRange(evt.User.Components.SelectMany(c => c.GetActions(evt, EUseSource.User)));
                funcs.AddRange(evt.User.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(evt, EUseSource.UserItem))));
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

            Message.Write(evt.GetMessage(), evt.User?.Move?.Pos, Color.Pink); //TODO: Color

            return true;
        }
    }

    public abstract class BaseEvent
    {
        public Entity User;
        public Entity Item;
        public Entity Target;

        public abstract String GetMessage();
    }

    public class DrinkEvent : BaseEvent
    {
        public DrinkEvent(Entity self, Entity item)
        {
            User = self;
            Item = item;
        }

        public override string GetMessage()
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

        public override string GetMessage()
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

        public override string GetMessage()
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

        public AttackEvent(Entity self, Entity weapon, Entity target, EAttackMove move)
        {
            User = self;
            Item = weapon;
            Target = target;
            Move = move;
        }

        public override string GetMessage()
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
        public List<(double, string)> PreMults = new List<(double, string)>();
        public List<(int, string)> Modifiers = new List<(int, string)>();
        public List<(double, string)> PostMults = new List<(double, string)>();

        public int Damage {
            get {
                double dmg = BaseDamage;
                foreach ((var d, var s) in PreMults) dmg += d;
                foreach ((var i, var s) in Modifiers) dmg += i;
                foreach ((var d, var s) in PostMults) dmg += d;

                return (int)(Math.Max(dmg + 0.5, 0));
            }
        }
        public bool Significant => (Damage > 0);

        public DamageEvent(BaseEvent prevMessage, int damage, EDamageType damageType)
        {
            User = prevMessage.User;
            Item = prevMessage.Item;
            Target = prevMessage.Target;
            BaseDamage = damage;
            DamageType = damageType;
        }

        public override string GetMessage()
        {
            var dmg = Damage;

            StringBuilder sb = new StringBuilder();
            sb.Append($"{Word.AName(Target)} {Word.Verb(Target, "take")} {(dmg > 0 ? dmg.ToString() : "no") } damage. (Base damage {BaseDamage})");


            return sb.ToString();
        }
    }
}
