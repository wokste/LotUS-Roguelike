using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLib
{
    public abstract class Controller
    {
        public Creature Self;

        public virtual bool ShouldDelete => !Self.Alive;

        protected Controller(Creature self)
        {
            Self = self;
        }

        public abstract bool Act();
    }

    public class PlayerController : Controller
    {
        private readonly Queue<Func<Creature, bool>> _actions = new Queue<Func<Creature, bool>>();
        public FieldOfView FieldOfView;

        public override bool ShouldDelete => false;

        public PlayerController(Creature self) : base(self)
        {
            FieldOfView = new FieldOfView(Self.Map._map);
            FieldOfView.Update(Self.Position);
        }

        public override bool Act()
        {
            if (!Self.Alive)
                return false;

            if (_actions.Count == 0)
                return false;

            var nextAction = _actions.Dequeue();
            var res = nextAction(Self);

            if (res)
                FieldOfView.Update(Self.Position); // Forces update, even if no position change

            return res;
        }

        public void Do(Func<Creature, bool> action)
        {
            _actions.Enqueue(action);
        }

        public void DoWalk(Point point)
        {
            Do(s => s.Walk(point));
        }
    }

    public class AiController : Controller
    {
        public Creature Enemy;
        
        public AiController(Creature self) : base(self)
        {
        }

        public override bool Act()
        {
            if (Enemy == null || !Enemy.Alive)
                Enemy = FindEnemy();

            if (Enemy == null)
                return ActWander();
            return ActChase();
        }

        public bool ActWander()
        {
            for (int i = 0; i < 10; i++)
            {
                var delta = new Point(Dicebag.UniformInt(-1,2), Dicebag.UniformInt(-1, 2));

                if (Self.Walk(delta))
                    return true;
            }
            
            return true;
        }

        public bool ActChase()
        {
            var delta = new Point(Enemy.Position.X - Self.Position.X, Enemy.Position.Y - Self.Position.Y);

            if (Self.Attack != null && Self.Attack.InRange(Self, Enemy))
            {
                Self.Attack.Attack(Self, Enemy);
                return true;
            }

            var deltaClamped = new Point(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));

            if (Self.Walk(deltaClamped))
                return true;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (Self.Walk(new Point(deltaClamped.X, 0)))
                    return true;

                if (Self.Walk(new Point(0, deltaClamped.Y)))
                    return true;
            }
            else
            {
                if (Self.Walk(new Point(0, deltaClamped.Y)))
                    return true;

                if (Self.Walk(new Point(deltaClamped.X, 0)))
                    return true;
            }

            // Fallback. Enemy could not be reached.
            Enemy = null;
            return ActWander();
        }

        public Creature FindEnemy()
        {
            foreach(var c in Self.Map.Creatures)
            {
                if (c == Self)
                    continue;

                if (c.Name != "Steven") // TODO: Better criteria for what creature to attack
                    continue;

                var delta = new Point(Self.Position.X - c.Position.X, Self.Position.Y - c.Position.Y);
                var deltalen = delta.X * delta.X + delta.Y * delta.Y;

                if (deltalen < 100)
                    return c;
            }

            return null;
        }

    }
}
