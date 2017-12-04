using System;
using System.Data;
using System.Drawing;

namespace HackLib
{
    public class Creature
    {
        public String Name;
        public Bar HitPoints;
        public Bar Hunger;
        public Attack Attack;
        
        public readonly Inventory Inventory = new Inventory();
        public bool Alive = true;

        public Point Position { get; set; }
        public Point Facing { get; set; }
        
        public bool Walk(Point direction, TileGrid map)
        {
            // TODO: Precondition |direction| == 1

            Facing = direction;

            var newPosition = Position;
            newPosition.Offset(direction);

            // You cannot walk of the edge of map
            if (newPosition.X < 0 || newPosition.X >= map.Width || newPosition.Y < 0 || newPosition.Y >= map.Height)
                return false;

            // Terrain collisions;
            if (map.Grid[newPosition.X, newPosition.Y].Wall != null)
                return false;

            // TODO: Creature collision

            Position = newPosition;
            return true;
        }

        public bool Mine(TileGrid map)
        {
            var minePosition = Position;
            minePosition.Offset(Facing);

            if (minePosition.X < 0 || minePosition.X >= map.Width || minePosition.Y < 0 || minePosition.Y >= map.Height)
                return false;

            var wall = (map.Grid[minePosition.X, minePosition.Y]).Wall;

            // Nothing to drop
            if (wall == null)
                return false;

            Inventory.Add(new Item
            {
                Type = ItemTypeList.Get(wall.DropTag),
                Count = Dicebag.Randomize(wall.DropCount),
            });

            map.Grid[minePosition.X, minePosition.Y].Wall = null;

            return true;
        }

        public void Eat()
        {
            if (Hunger.Current == Hunger.Max)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You are not hungry");
                return;
            }

            var foodStack = Inventory.Find(ItemTypeList.Get("food"));

            if (foodStack == null || foodStack.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nothing to eat");
                return;
            }

            foodStack.Count--;
            Hunger.Current += 5;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Eaten food. Food left: {foodStack.Count}.");
            Hunger.PrintBar("Hunger");
        }
    }
}
