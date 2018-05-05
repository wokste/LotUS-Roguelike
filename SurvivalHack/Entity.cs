using System;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ai;
using System.Collections.Generic;

namespace SurvivalHack
{
    public class Entity : IDescriptionProvider
    {
        public String Name { get; set; }
        public override string ToString() => Name;

        public Bar Health;
        public Bar Hunger;

        public List<IComponent> Components = new List<IComponent>();
        public MoveComponent Move;

        public TerrainFlag Flags;
        public Symbol Symbol;

        public bool Alive = true;

        public string Description { get; set; }

        public float Speed = 1;

        public AiActor Ai;
        public Attitude Attitude;

        public float LeftoverMove;

        public void TakeDamage(int damage, EDamageType damageType)
        {
            Health.Current -= damage;

            if (Health.Current == 0)
            {
                Alive = false;
                OnDestroy?.Invoke(this);
                Move.Unbind(this);

                Message.Write($"{Name} died", Move?.Pos, Color.Red);
            }
        }

        public Action<Entity> OnDestroy;

        public IEnumerable<T> Get<T>() where T : class, IComponent
        {
            foreach (var c in Components)
            {
                var c2 = c as T;

                if (c2 != null)
                    yield return c2;
            }
        }

        public T GetOne<T>() where T : class, IComponent
        {
            foreach (var c in Components)
            {
                var c2 = c as T;

                if (c2 != null)
                    return c2;
            }
            return null;
        }

        internal void Add(IComponent component)
        {
            Components.Add(component);
        }

        public bool UseItem(Entity item)
        {
            var ls = item.Get<IConsumeComponent>();
            bool any = false;

            foreach (var c in ls)
            {
                any = true;
                c.Use(item, this);
            }

            if (any == false)
            {
                Message.Write("You can't use that, silly person.", null, Color.Red);
                return false;
            }

            if (item.GetOne<StackComponent>()?.Consume() ?? false)
                GetOne<Inventory>()?.Remove(item);

            return true;
        }

        internal void Attack(Entity enemy, Entity weapon)
        {
            foreach ( var attack in weapon.Get<ECM.IAttackComponent>())
            {
                attack.Attack(this, enemy);
            }
        }
    }
}
