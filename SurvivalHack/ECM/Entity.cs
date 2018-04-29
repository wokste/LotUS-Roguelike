using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class Entity : IDescriptionProvider
    {
        public String Name { get; set; }
        public Bar Health;
        public Bar Hunger;
        public AttackComponent Attack;
        public MoveComponent Move;
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

        public bool Eat(Item food)
        {
            if (food.Type.OnEat == null)
            {
                Message.Write("You can't eat that, silly person.", null, Color.Red);
                return false;
            }

            food.Type.OnEat.Use(food, this);
            return true;
        }
    }
}
