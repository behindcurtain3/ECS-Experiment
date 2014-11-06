using IsoECS.GamePlay;
using TecsDotNet;

namespace IsoECS.Behaviors
{
    public class IdleBehavior : Behavior
    {
        public double IdleTime { get; set; }
        public double IdleCountdown { get; set; }

        public IdleBehavior()
        {
            IdleTime = -1;
        }

        public override void Init(GameWorld world, Entity self)
        {
            base.Init(world, self);

            if (IdleTime <= 0)
                IdleTime = World.Random.NextDouble();
            IdleCountdown = IdleTime;
        }

        public override BehaviorStatus Update(Entity self, double dt)
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
