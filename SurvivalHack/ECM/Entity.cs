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

        public Symbol Symbol;

        public readonly Inventory Inventory = new Inventory();
        public bool Alive = true;

        public string Description { get; set; }

        public float Speed = 1;
        public FieldOfView FoV;

        public void TakeDamage(int damage)
        {
            Health.Current -= damage;

            if (Health.Current == 0)
            {
                Alive = false;
                OnDestroy?.Invoke(this);
                Move.Remove(this);
                
                Message.Write($"{Name} died", Move.Pos, Color.Red);
            }
        }

        public Action<Entity> OnDestroy;

        public bool Eat(Item food)
        {
            if (food.Type.OnEat == null)
            {
                Message.Write("You can't eat that, silly person.", Vec.NaV, Color.Red);
                return false;
            }

            food.Type.OnEat.Use(food, this);
            return true;
        }
    }

    public class Player : Entity{

        public Player(MoveComponent position) {
            Move = position;
            FoV = new FieldOfView(position);
        }
    }

    public class Monster : Entity{
        public float LeftoverMove;

        public Entity Enemy;

        public AiController Ai;

        public void Act()
        {
            Ai.FindEnemy(this);

            // Moving.
            LeftoverMove += Speed;
            for (var i = 1; i <= LeftoverMove; i++)
                Ai.Move(this);
            LeftoverMove = LeftoverMove - (int) LeftoverMove;

            // Acting
            Ai.Act(this);
        }
    }
}
