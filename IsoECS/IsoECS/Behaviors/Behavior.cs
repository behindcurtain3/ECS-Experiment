using System.Collections.Generic;
using IsoECS.Entities;
using System;

namespace IsoECS.Behaviors
{
    [Serializable]
    public enum BehaviorStatus
    {
        STARTING,
        RUNNING,
        WAITING,
        SUCCESS,
        FAILURE
    }

    [Serializable]
    public abstract class Behavior
    {
        public BehaviorStatus Status { get; set; }

        public Behavior()
        {
            Status = BehaviorStatus.STARTING;
        }

        public virtual void Init(Entity self) 
        {
            Status = BehaviorStatus.RUNNING;
        }

        // called to update the behavior
        public abstract void Update(Entity self, Stack<Behavior> state, int dt);
        
        // called when a sub behavior has finished/quit
        public virtual void OnSubFinished(Entity self, Behavior finished, Stack<Behavior> state) { }
    }
}
