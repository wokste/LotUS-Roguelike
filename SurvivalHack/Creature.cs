using System;
using System.Linq;
using HackConsole;

namespace SurvivalHack
{
    public class Creature : IDescriptionProvider
    {
        public String Name { get; set; }
        public Bar Health;
        public Bar Hunger;
        public AttackComponent Attack;

        public Symbol Symbol;

        public readonly Inventory Inventory = new Inventory();
        public bool Alive = true;

        public Vec Position { get; set; }
        public Vec Facing { get; set; }

        public string Description { get; set; }

        public float Speed = 1;
        public TerrainFlag MovementType = TerrainFlag.Walk;

        public World Map;

        public virtual bool Walk(Vec direction)
        {
            Facing = direction;

            var newPosition = Position;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!Map.InBoundary(newPosition.X, newPosition.Y))
                return false;

            // Terrain collisions
            if (!Map.HasFlag(newPosition.X, newPosition.Y, MovementType))
                return false;

            Position = newPosition;
            return true;
        }

        public void TakeDamage(int damage)
        {
            Health.Current -= damage;

            if (Health.Current == 0)
            {
                Alive = false;
                Map.Creatures.Remove(this);
                // TODO: Stuff
                
                Message.Write($"{Name} died", Position, Color.Red);
            }
        }

        public bool Mine()
        {
            var minePosition = Position;
            minePosition += Facing;

            var x = minePosition.X;
            var y = minePosition.Y;

            if (!Map.InBoundary(x, y))
                return false;

            var wall = Map.GetWall(x, y);

            // Nothing to drop
            if (wall == null)
                return false;

            Inventory.Add(new Item
            {
                Type = ItemTypeList.Get(wall.DropTag),
                Count = Dicebag.Randomize(wall.DropCount),
            });

            Map.DestroyWall(x, y);

            return true;
        }

        public bool Eat()
        {
            var foodItems = Inventory._items.Where(i => i.Type.OnEat != null);
            var food = foodItems.OrderBy(f => f.Type.OnEat.Quality).LastOrDefault();

            if (food == null)
            {
                Message.Write("You have no food in your inventory.", Position, Color.Red);
                return false;
            }

            food.Type.OnEat.Use(food, this);
            return true;
        }
    }

    public class Player : Creature{
        public FieldOfView FoV;

        public Player(World map, Vec position) {
            Position = position;
            Map = map;

            FoV = new FieldOfView(map.Map);
            FoV.Update(position);
        }

        public override bool Walk(Vec direction)
        {
            var ret = base.Walk(direction);
            FoV.Update(Position);
            return ret;
        }
    }

    public class Monster : Creature{
        public float LeftoverMove;

        public Creature Enemy;

        public AiController Ai;

        public void Act()
        {
            Ai.FindEnemy(this);

            // Moving.
            LeftoverMove += Speed;
            for (var i = 1; i < LeftoverMove; i++)
                Ai.Move(this);
            LeftoverMove = LeftoverMove - (int) LeftoverMove;

            // Acting
            Ai.Act(this);
        }
    }
}
