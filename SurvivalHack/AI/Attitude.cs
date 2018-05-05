using HackConsole;
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
        private IAttitudeRule[] _rules;

        public Attitude(ETeam team, IAttitudeRule[] rules)
        {
            Team = team;
            _rules = rules;
        }

        public Goal GetGoal(Entity self)
        {
            if (_goal.IsNull || !_goal.Target.Alive)
            {
                _goal = Goal.NullGoal;
            }

            var pos = self.Move.Pos;

            // Todo: Sight radius
            var area = new Rect(pos - new Vec(10, 10), new Vec(21, 21));

            foreach (var e in self.Move.Level.GetEntities(area))
            {
                if (e == self)
                    continue;

                var delta = self.Move.Pos - e.Move.Pos;
                if (delta.LengthSquared >= 100) // Todo: Sight radius
                    continue;

                OnSee(self, e);
            }

            return _goal;
        }

        private void OnSee(Entity self, Entity other)
        {
            foreach (var rule in _rules)
                rule.On(self, other, ref _goal, ERisk.None);
        }

        public void TakeDamage(Entity self, Entity other)
        {
            var pos = self.Move.Pos;

            foreach (var rule in _rules)
                rule.On(self, other, ref _goal, ERisk.Threaten | ERisk.AttackFaction | ERisk.Attack);

            if (Team == 0)
                return;

            // Warn allies within 10 squares
            var area = new Rect(pos - new Vec(10, 10), new Vec(21, 21));

            foreach (var ally in self.Move.Level.GetEntities(area))
            {
                if (ally == self)
                    continue;
                var aiAttitude = ally.Attitude;

                if (aiAttitude == null || aiAttitude.Team != Team)
                    continue;

                var delta = self.Move.Pos - ally.Move.Pos;

                if (delta.LengthSquared < 100) // Todo: Sight radius
                    aiAttitude.TeamAttacked(ally, other);
            }
        }

        private void TeamAttacked(Entity self, Entity other)
        {
            foreach (var rule in _rules)
                rule.On(self, other, ref _goal, ERisk.Threaten | ERisk.AttackFaction);
        }
    }

    public interface IAttitudeRule
    {
        bool On(Entity self, Entity other, ref Goal goal, ERisk risk);
    }

    public class TeamAttitudeRule : IAttitudeRule
    {
        private ETargetAction _act;
        private ETeam _team;
        private int _priority;

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
