using System;
using System.Collections.Generic;
using HackConsole;
using SurvivalHack.ECM;

#if WIZTOOLS

namespace SurvivalHack
{
    static public class WizTools
    {
        public static Inventory Tools = new Inventory();

        public static void Init() {
            Entity entity;
            entity = new Entity('w', "Reveal Map", EEntityFlag.Pickable);
            entity.Add(new MapRevealComponent(FieldOfView.SET_ALWAYSVISIBLE, typeof(CastEvent)));
            Tools.Add(entity);

            entity = new Entity('w', "Discover Map - scroll?", EEntityFlag.Pickable);
            entity.Add(new MapRevealComponent(FieldOfView.FLAG_DISCOVERED, typeof(CastEvent)));
            Tools.Add(entity);

            entity = new Entity('w', "Genocide", EEntityFlag.Pickable);
            entity.Add(new AreaAttack(9001, Combat.EDamageType.Piercing, typeof(CastEvent)));
            Tools.Add(entity);

            entity = new Entity('w', "Heal", EEntityFlag.Pickable);
            entity.Add(new HealComponent(9001, 0, typeof(CastEvent)));
            Tools.Add(entity);
        }

        public class AreaAttack : IComponent
        {
            public float Damage;
            public Combat.EDamageType DamageType;
            public Type MessageType;

            public AreaAttack(float damage, Combat.EDamageType damageType, Type messageType)
            {
                Damage = damage;
                DamageType = damageType;
                MessageType = messageType;
            }

            public void GetActions(Entity self, BaseEvent msg, EUseSource source)
            {
                if (source == EUseSource.Item && MessageType.IsAssignableFrom(msg.GetType()))
                    msg.OnEvent += Genocide;
            }

            public void Genocide(BaseEvent msg)
            {
                var level = msg.User.Level;
                foreach (var e in level.GetEntities(new Rect(Vec.Zero, level.Size)))
                {
                    if (!e.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                        continue;

                    var dmgMsg = new DamageEvent(msg, 9001, Combat.EDamageType.Fire, Combat.EDamageLocation.Head)
                    {
                        Target = e
                    };
                    Eventing.On(dmgMsg, msg);
                }
            }

            public string Describe() => $"Area Attack deals {Damage} damage";
        }
    }
}

#endif