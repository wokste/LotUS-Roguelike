using System.Collections.Generic;
using HackConsole;
using SurvivalHack.ECM;

#if WIZTOOLS

namespace SurvivalHack
{
    static class WizTools
    {
        public static Inventory Tools = new Inventory();

        public static void Init() {
            Entity entity;
            entity = new Entity('\\', "Reveal Map", EEntityFlag.Pickable);
            entity.Add(new MapRevealComponent(FieldOfView.SET_ALWAYSVISIBLE, EUseMessage.Cast));
            Tools.Add(entity);

            entity = new Entity('\\', "Discover Map - scroll?", EEntityFlag.Pickable);
            entity.Add(new MapRevealComponent(FieldOfView.FLAG_DISCOVERED, EUseMessage.Cast));
            Tools.Add(entity);

            entity = new Entity('\\', "Genocide", EEntityFlag.Pickable);
            entity.Add(new AreaAttack(9001, Combat.EDamageType.Piercing, EUseMessage.Cast));
            Tools.Add(entity);

            entity = new Entity('\\', "Heal", EEntityFlag.Pickable);
            entity.Add(new HealComponent(9001, 0, EUseMessage.Cast));
            Tools.Add(entity);
        }

        public class AreaAttack : IComponent
        {
            public float Damage;
            public Combat.EDamageType DamageType;
            public EUseMessage Filter;

            public AreaAttack(float damage, Combat.EDamageType damageType, EUseMessage filter)
            {
                Damage = damage;
                DamageType = damageType;
                Filter = filter;
            }

            public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source)
            {
                throw new System.NotImplementedException();
            }

            public bool Use(Entity user, Entity item, Entity target, EUseMessage filter)
            {
                if (Filter != filter)
                    return false;

                var level = user.Move.Level;
                foreach (var e in level.GetEntities(new Rect(Vec.Zero, level.Size)))
                {
                    if (!e.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                        continue;

                    var Attack = new Combat.Attack
                    {
                        Damage = (int)Damage,
                        DamageType = DamageType
                    };

                    Attack.Fight(user, item, e);
                }

                return true;
            }

            public string Describe() => $"Area Attack deals {Damage} damage";
        }
    }
}

#endif