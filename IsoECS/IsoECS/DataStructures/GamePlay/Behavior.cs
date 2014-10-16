using IsoECS.Entities;

namespace IsoECS.DataStructures.GamePlay
{
    public enum BehaviorStatus
    {
        RUNNING,
        WAITING,
        SUCCESS,
        FAILURE
    }

    public abstract class Behavior
    {
        public BehaviorStatus Status { get; set; }

        public void Init(EntityManager em, Entity self) { }

        // called to update the behavior
        public void Update(EntityManager em, Entity self) { }

        // called when a sub behavior has finished/quit
        public void OnSubFinished(EntityManager em, Entity self, Behavior finished) { }
        
        public Behavior()
        {
        }
    }
}
