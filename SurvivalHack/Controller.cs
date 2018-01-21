using System;
using System.Collections.Generic;
using HackConsole;

namespace SurvivalHack
{
    public abstract class Controller
    {
        public Creature Self;

        public virtual bool ShouldDelete => !Self.Alive;

        protected Controller(Creature self)
        {
            Self = self;
        }

        public abstract double Do();
    }

    public class PlayerController : Controller
    {
        private readonly Queue<Func<Creature, double>> _actions = new Queue<Func<Creature, double>>();
        public FieldOfView FieldOfView;

        public override bool ShouldDelete => false;

        public PlayerController(Creature self) : base(self)
        {
            FieldOfView = new FieldOfView(Self.Map._map);
            FieldOfView.Update(Self.Position);
        }

        public override double Do()
        {
            if (!Self.Alive)
                return -1;

            if (_actions.Count == 0)
                return -1;

            var nextAction = _actions.Dequeue();
            var res = nextAction(Self);

            if (res >= 0)
                FieldOfView.Update(Self.Position); // Forces update, even if no position change

            return res;
        }

        public void Plan(Func<Creature, double> action)
        {
            _actions.Enqueue(action);
        }

        public void PlanWalk(Vec direction)
        {
            var actPoint = Self.Position + direction;
            foreach (var c in Self.Map.Creatures)
            {
                if (c.Position == actPoint)
                {
                    Plan(s =>
                    {
                        s.Attack.Attack(s, c);
                        return 1;
                    });
                    return;
                }
            }

            Plan(s => s.Walk(direction) ? direction.ManhattanLength : -1);
        }
    }

    public class AiController : Controller
    {
        public Creature Enemy;
        
        public AiController(Creature self) : base(self)
        {
        }

        public override double Do()
        {
            if (Enemy == null || !Enemy.Alive)
                Enemy = FindEnemy();

            if (Enemy == null)
                return ActWander();

            var r = AttackEnemy();

            if (r < 0)
            {
                r = Math.Max(ChaseEnemy(), AttackEnemy());
            }
            return r;
        }

        public double ActWander()
        {
            for (var i = 0; i < 10; i++)
            {
                var delta = new Vec(Dicebag.UniformInt(-1,2), Dicebag.UniformInt(-1, 2));
                
                if (Self.Walk(delta))
                    return delta.ManhattanLength;
            }
            
            return 1;
        }

        public double ChaseEnemy()
        {
            var delta = Enemy.Position - Self.Position;

            var deltaClamped = new Vec(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));
            if (Self.Walk(deltaClamped))
                return deltaClamped.ManhattanLength;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (Self.Walk(new Vec(deltaClamped.X, 0)))
                    return 1;

                if (Self.Walk(new Vec(0, deltaClamped.Y)))
                    return 1;
            }
            else
            {
                if (Self.Walk(new Vec(0, deltaClamped.Y)))
                    return 1;
                
                if (Self.Walk(new Vec(deltaClamped.X, 0)))
                    return 1;
            }

            // Fallback. Enemy could not be reached.
            Enemy = null;
            return ActWander();
        }

        public double AttackEnemy()
        {
            if (Enemy != null && Self.Attack != null && Self.Attack.InRange(Self, Enemy))
            {
                Self.Attack.Attack(Self, Enemy);
                return 1;
            }
            return -1;
        }

        public Creature FindEnemy()
        {
            foreach(var c in Self.Map.Creatures)
            {
                if (c == Self)
                    continue;

                if (c.Name != "Steven") // TODO: Better criteria for what creature to attack
                    continue;

                var delta = Self.Position - c.Position;

                if (delta.LengthSquared < 100)
                    return c;
            }

            return null;
        }

    }
}
