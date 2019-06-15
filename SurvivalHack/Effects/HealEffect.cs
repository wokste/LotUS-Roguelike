using SurvivalHack.Combat;
using System.Text;

namespace SurvivalHack.Effects
{
    public class HealEffect : IEntityEffect
    {
        public int Restore;
        public int StatID;

        public EntityTarget UseOn { get; }

        public HealEffect(int restore, int statID, EntityTarget useOn)
        {
            Restore = restore;
            StatID = statID;
            UseOn = useOn;
        }
        
        public bool Use(Entity instignator, Entity target, StringBuilder sb)
        {
            var stats = target.GetOne<StatBlock>();

            stats.Heal(Restore, StatID);
            sb?.Append($"{Word.AName(target)} {Word.Verb(target, "heal")} {Restore} HP. "); // TODO: not always HP
            return true;
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1; // TODO
        }
    }
}
