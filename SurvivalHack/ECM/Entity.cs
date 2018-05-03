using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class Entity : IDescriptionProvider
    {
        public String Name { get; set; }
        public override string ToString() => Name;

        public Bar Health;
        public Bar Hunger;
        public AttackComponent Attack;
        public MoveComponent Move;
        public ConsumableComponent Consume;
        public StackComponent StackComponent;

        public TerrainFlag Flags;
        public Symbol Symbol;

        public readonly Inventory Inventory = new Inventory();
        public bool Alive = true;

        public string Description { get; set; }

        public float Speed = 1;
        public FieldOfView FoV;
        public AiActor Ai;
        public Attitude Attitude;

        public float LeftoverMove;

        public void TakeDamage(int damage)
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

        public bool UseItem(Entity item)
        {
            if (item.Consume == null)
            {
                Message.Write("You can't use that, silly person.", null, Color.Red);
                return false;
            }

            item.Consume.Use(item, this);

            if (item.StackComponent.Consume())
                Inventory.Remove(item);

            return true;
        }
    }
}
