using System.Collections.Generic;
using IsoECS.Entities;
using System;

namespace IsoECS.Behaviors
{
    [Serializable]
    public enum BehaviorStatus
    {
        RUN, // tell the behavior to run its logic
        SUCCESS, // tell the behavior it was a success
        FAIL, // failed
        WAIT // tell the behavior to wait
    }

    [Serializable]
    public abstract class Behavior
    {
        public Behavior Child { get; set; }
        public Behavior Finished { get; set; }
        public bool Initialized { get; set; }

        public Behavior()
        {
            Initialized = false;
        }

        public virtual void Init(Entity self)
        {
            Initialized = true;
        }

        // called to update the behavior
        public virtual BehaviorStatus Update(Entity self, int dt)
        {
            if (Child != null)
            {
                if (!Child.Initialized)
                    Child.Init(self);

                BehaviorStatus status = Child.Update(self, dt);

                switch (status)
                {
                    case BehaviorStatus.SUCCESS:
                    case BehaviorStatus.FAIL:
                        Finished = Child;
                        Child = null;
                        break;
                }

                return status;
            }

            return BehaviorStatus.RUN;
        }

        public void AddChild(Behavior child)
        {
            Child = child;
        }
    }
}
