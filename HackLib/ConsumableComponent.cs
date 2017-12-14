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
            user.Inventory.Consume(item,1);
            user.Hunger.Current += FoodRestore;
            user.Health.Current += HealthRestore;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when the item has influence.</returns>
        public bool UseEffect(Item item, Creature user)
        {
            var change = false;
            change |= (HealthRestore > 0 && user.Health.Current < user.Health.Max);
            change |= (FoodRestore > 0 && user.Hunger.Current < user.Hunger.Max);

            return change;
        }
    }
}
