using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.ECM
{
    public class AiAttitude
    {
        public Entity Target;
        public EAttitude TargetAction;

        public int Team;
        public EAttitude AttitudeOtherTeams = EAttitude.Hate;
        public EAttitude AttitudeAnimals = EAttitude.Hate;
        public EAttitude AttitudeDamage = EAttitude.Hate;
        public bool HelpAllies;

        public void UpdateTarget(Entity self)
        {
            if (Target == null || !Target.Alive)
            {
                Target = null;
                TargetAction = EAttitude.Ignore;
            }

            var pos = self.Move.Pos;

            // Todo: Sight radius
            var area = new Rect(pos - new Vec(10, 10), new Vec(21, 21));

            foreach (var e in self.Move.Level.GetEntities(area))
            {
                if (e == self)
                    continue;

                var delta = self.Move.Pos - e.Move.Pos;

                if (delta.LengthSquared < 100) // Todo: Sight radius
                    OnSee(self,e);
            }
        }

        private void OnSee(Entity self, Entity other)
        {
            var otherAttitude = other.Attitude;

            if (otherAttitude == null)
                return;

            if (Team != 0 && Team == otherAttitude.Team)
                return; // Being friendly to creatures of the same team.

            SetTarget(other, otherAttitude.Team == 0 ? AttitudeAnimals : AttitudeOtherTeams);
        }
        
        public void TakeDamage(Entity self, Entity other)
        {
            var pos = self.Move.Pos;

            OnTakeDamage(self, other);

            // Todo: Sight radius
            var area = new Rect(pos - new Vec(10, 10), new Vec(21, 21));

            foreach (var ally in self.Move.Level.GetEntities(area))
            {
                if (ally == self)
                    continue;
                var allyAttitude = ally.Attitude;

                if (allyAttitude != null && allyAttitude.HelpAllies)
                {
                    var delta = self.Move.Pos - ally.Move.Pos;

                    if (delta.LengthSquared < 100) // Todo: Sight radius
                        allyAttitude.OnTakeDamage(ally, other);
                }
            }
        }

        private void OnTakeDamage(Entity self, Entity other)
        {
            var otherAttitude = other.Attitude;

            if (Team != 0 && Team == otherAttitude.Team)
                return; // Being friendly to creatures of the same team.

            SetTarget(other, AttitudeDamage);
        }

        private bool SetTarget(Entity newTarget, EAttitude newAttitude) {

            // TODO: Check priority.

            Target = newTarget;
            TargetAction = newAttitude;
            return true;
        }
    }

    public enum EAttitude
    {
        Ignore, // Do nothing
        Follow, // Follow it
        Chase,  // Chase afer enemy
        Hate,   // Attacks it
        Fear,   // Run away
    }
}
