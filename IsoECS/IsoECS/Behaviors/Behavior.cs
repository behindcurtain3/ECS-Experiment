using System.Collections.Generic;
using IsoECS.Entities;
using System;

namespace IsoECS.Behaviors
{
    [Serializable]
    public enum BehaviorStatus
    {
        RUNNING,
        WAITING,
        SUCCESS,
        FAILURE
    }

    [Serializable]
    public abstract class Behavior
    {
        public BehaviorStatus Status { get; set; }

        public virtual void Init(EntityManager em, Entity self) 
        {
            Status = BehaviorStatus.RUNNING;
        }

        // called to update the behavior
        public abstract void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt);
        
        // called when a sub behavior has finished/quit
        public virtual void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state) { }
    }
}
