using HackConsole;
using HackConsole.Algo;
using System;

namespace SurvivalHack.Ai
{
    public struct Goal
    {
        public Entity Target;
        public ETargetAction TargetAction;
        public int Priority;

        public static Goal NullGoal = new Goal(null, ETargetAction.Ignore, int.MinValue);
        public bool IsNull => Target == null;
        
        public Goal(Entity target, ETargetAction action, int priority)
        {
            Target = target;
            TargetAction = action;
            Priority = priority;
        }
    }

    public class Attitude
    {
        private Goal _goal;

        public ETeam Team;
        public IAttitudeRule[] Rules;


        public Attitude() { }

        public Attitude(ETeam team, IAttitudeRule[] rules)
        {
            Team = team;
            Rules = rules;
        }

        public Goal GetGoal(Entity self)
        {
            if (_goal.IsNull || _goal.Target.EntityFlags.HasFlag(EEntityFlag.Destroyed))
            {
                _goal = Goal.NullGoal;
            }

            var pos = self.Pos;

            // Todo: Sight radius
            var radius = 10;
            foreach (var e in self.Level.GetEntities(new Circle(pos, radius)))
            {
                CheckSee(self, e);
            }

            return _goal;
        }

        private void CheckSee(Entity self, Entity other)
        {
            if (other == self)
                return;

            var level = self.Level;
            var path = Line.Run(self.Pos, other.Pos);
            foreach (var v in path)
                if (level.GetTile(v).BlockSight)
                    return;

            foreach (var rule in Rules)
                rule.On(self, other, ref _goal, ERisk.None);
        }

        public void TakeDamage(Entity self, Entity other)
        {
            var pos = self.Pos;

            foreach (var rule in Rules)
                rule.On(self, other, ref _goal, ERisk.Threaten | ERisk.AttackFaction | ERisk.Attack);

            if (Team == 0)
                return;

            // Warn allies within 10 squares
            var radius = 10; // Todo: Sight radius

            foreach (var ally in self.Level.GetEntities(new Circle(pos, radius)))
            {
                if (ally == self)
                    continue;
                var aiAttitude = ally.Attitude;

                if (aiAttitude == null || aiAttitude.Team != Team)
                    continue;

                aiAttitude.TeamAttacked(ally, other);
            }
        }

        private void TeamAttacked(Entity self, Entity other)
        {
            foreach (var rule in Rules)
                rule.On(self, other, ref _goal, ERisk.Threaten | ERisk.AttackFaction);
        }
    }

    public interface IAttitudeRule
    {
        bool On(Entity self, Entity other, ref Goal goal, ERisk risk);
    }

    public class TeamAttitudeRule : IAttitudeRule
    {
        private readonly ETargetAction _act;
        private readonly ETeam _team;
        private readonly int _priority;

        public TeamAttitudeRule(ETargetAction act, ETeam team, int priority = 1) {
            
            _act = act;
            _team = team;
            _priority = priority;
        }

        public bool On(Entity self, Entity other, ref Goal goal, ERisk risk)
        {
            if (other.Attitude == null)
                return false;

            if (_team != 0 && other.Attitude.Team != _team)
                return false;

            // TODO: Priority based on distance
            if (goal.Priority > _priority)
                return false;

            goal = new Goal(other, _act, _priority);
            return true;
        }
    }

    public enum ETargetAction
    {
        Ignore, // Do nothing
        Follow, // Follow it
        Chase,  // Chase afer enemy
        Hate,   // Attacks it
        Fear,   // Run away
    }

    [Flags]
    public enum ERisk
    {
        None = 0,          // Nothing major. It might have seen or heard the other
        Threaten = 1,      // Do something that is threatening, like preparing an attack
        AttackFaction = 2, // Attack the faction
        Attack = 4,        // Chase afer enemy
        All = ~0,
    }

    public enum ETeam
    {
        None,
        Player,
        Undead,
    }
}
