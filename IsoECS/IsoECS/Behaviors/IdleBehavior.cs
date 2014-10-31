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

        public override BehaviorStatus Update(Entity self, int dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                case BehaviorStatus.SUCCESS:
                case BehaviorStatus.FAIL:
                    return status;

                case BehaviorStatus.RUN:
                    IdleCountdown -= dt;

                    if (IdleCountdown <= 0)
                    {
                        return BehaviorStatus.SUCCESS;
                    }
                    break;
            }

            return BehaviorStatus.WAIT;
        }
    }
}
