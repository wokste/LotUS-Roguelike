﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLib
{
    public class ConsumableComponent
    {
        /// <summary>
        /// AI based value to sort items based on quality. The programmer writes these down.
        /// </summary>
        public int Quality;

        public int FoodRestore;
        public int HealthRestore;

        public bool Use(Item item, Creature user)
        {
            item.Count--;
            user.Hunger.Current += 5;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when the item has influence.</returns>
        public bool UseEffect(Item item, Creature user)
        {
            bool change = false;
            change |= (HealthRestore > 0 && user.Health.Current < user.Health.Max);
            change |= (FoodRestore > 0 && user.Hunger.Current < user.Hunger.Max);

            return change;
        }
    }
}
