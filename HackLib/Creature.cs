﻿using System;
using System.Drawing;
using System.Linq;

namespace HackLib
{
    public class Creature
    {
        public String Name;
        public Bar Health;
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
            var foodItems = Inventory._items.Where(i => i.Type.EatComponent != null);
            var food = foodItems.OrderBy(f => f.Type.EatComponent.Quality).LastOrDefault();
            
            if (food == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You have no food in your inventory.");
                return;
            }
            
            food.Type.EatComponent.Use(food, this);
            DisplayStats();
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
