namespace SurvivalHack.ECM
{
    public class ConsumableComponent
    {
        /// <summary>
        /// AI based value to sort items based on quality. The programmer writes these down.
        /// </summary>
        public int Quality;

        public int FoodRestore;
        public int HealthRestore;

        public bool Use(Entity item, Entity user)
        {
            user.Hunger.Current += FoodRestore;
            user.Health.Current += HealthRestore;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True when the item has influence.</returns>
        public bool UseEffect(Entity item, Entity user)
        {
            var change = false;
            change |= (HealthRestore > 0 && user.Health.Current < user.Health.Max);
            change |= (FoodRestore > 0 && user.Hunger.Current < user.Hunger.Max);

            return change;
        }
    }
}
