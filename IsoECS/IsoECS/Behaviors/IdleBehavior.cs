using System.Collections.Generic;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class IdleBehavior : Behavior
    {
        public int IdleTime { get; set; }
        public int IdleCountdown { get; set; }

        public override void Init(EntityManager em, Entity self)
        {
            base.Init(em, self);

            IdleTime = Game1.Random.Next(2, 11) * 1000;
            IdleCountdown = IdleTime;
        }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            IdleCountdown -= dt;

            if (IdleCountdown <= 0)
            {
                Status = BehaviorStatus.SUCCESS;
            }
        }
    }
}
