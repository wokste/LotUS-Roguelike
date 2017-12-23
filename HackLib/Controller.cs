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
            FieldOfView = new FieldOfView(Self.Map);
            FieldOfView.Update(Self.Position);
        }

        public override bool Act()
        {
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
            if (Enemy == null)
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

            delta.X = delta.X < -1 ? -1 : delta.X > 1 ? 1 : delta.X;
            delta.Y = delta.Y < -1 ? -1 : delta.Y > 1 ? 1 : delta.Y;

            Self.Walk(delta);

            return true;
        }

        public Creature FindEnemy()
        {
            return null;
        }

    }
}
