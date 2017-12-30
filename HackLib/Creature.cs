using System;
using System.Linq;

namespace HackLib
{
    public class Creature
    {
        public String Name;
        public Bar Health;
        public Bar Hunger;
        public AttackComponent Attack;
        
        public Symbol Symbol;

        public readonly Inventory Inventory = new Inventory();
        public bool Alive = true;

        public Vec Position { get; set; }
        public Vec Facing { get; set; }

        public float Speed = 1;

        public World Map;
        
        public bool Walk(Vec direction)
        {
            // TODO: Precondition |direction| == 1

            Facing = direction;

            var newPosition = Position;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!Map.InBoundary(newPosition.X, newPosition.Y))
                return false;

            // Terrain collisions;
            if (Map.HasFlag(newPosition.X, newPosition.Y, TerrainFlag.BlockWalk))
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

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Name} died");
            }
        }

        public bool Mine()
        {
            var minePosition = Position;
            minePosition += Facing;

            int x = minePosition.X;
            int y = minePosition.Y;

            if (!Map.InBoundary(x, y))
                return false;

            var wall = Map.GetWall(x,y);

            // Nothing to drop
            if (wall == null)
                return false;

            Inventory.Add(new Item
            {
                Type = ItemTypeList.Get(wall.DropTag),
                Count = Dicebag.Randomize(wall.DropCount),
            });

            Map.DestroyWall(x,y);

            return true;
        }

        public bool Eat()
        {
            var foodItems = Inventory._items.Where(i => i.Type.OnEat != null);
            var food = foodItems.OrderBy(f => f.Type.OnEat.Quality).LastOrDefault();
            
            if (food == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You have no food in your inventory.");
                return false;
            }
            
            food.Type.OnEat.Use(food, this);
            DisplayStats();
            return true;
        }

        public void DisplayStats()
        {
            void DisplayStat(string name, Bar bar)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{name}: ");
                var perc = bar.Perc;
                
                Console.ForegroundColor = (perc > 0.6 ? ConsoleColor.Green : (perc > 0.3 ? ConsoleColor.Yellow : ConsoleColor.Red));
                Console.WriteLine($"{bar.Current:##}/{bar.Max:##}");
            }

            DisplayStat("Health", Health);
            DisplayStat("Hunger", Hunger);
        }
    }
}
