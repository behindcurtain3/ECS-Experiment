using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Util;

namespace IsoECS.Behaviors
{
    public class IdleBehavior : Behavior
    {
        public int IdleTime { get; set; }
        public int IdleCountdown { get; set; }

        public IdleBehavior()
        {
            IdleTime = -1;
        }

        public override void Init(Entity self)
        {
            base.Init(self);

            if(IdleTime == -1)
                IdleTime = EntityManager.Random.Next(5, 25) * 1000;
            IdleCountdown = IdleTime;
        }

        public override void Update(Entity self, Stack<Behavior> state, int dt)
        {
            IdleCountdown -= dt;

            if (IdleCountdown <= 0)
            {
                Status = BehaviorStatus.SUCCESS;
            }
        }
    }
}
