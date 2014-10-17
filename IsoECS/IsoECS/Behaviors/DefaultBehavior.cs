using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class DefaultBehavior : Behavior
    {
        public Behavior PreviousBehavior { get; set; }

        public DefaultBehavior()
        {
            PreviousBehavior = new IdleBehavior();
        }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            // look for a secondary behavior to do
            CitizenComponent citizen = self.Get<CitizenComponent>();

            // the citizen is homeless, find them a home!
            if (citizen.HousingID == -1 && PreviousBehavior.GetType() != typeof(FindHomeBehavior))
            {
                state.Push(new FindHomeBehavior());
                PreviousBehavior = state.Peek();
            }
            else if (citizen.JobID == -1 && PreviousBehavior.GetType() != typeof(FindJobBehavior))
            {
                state.Push(new FindJobBehavior());
                PreviousBehavior = state.Peek();
            }
            else
            {
                state.Push(new IdleBehavior());
                PreviousBehavior = state.Peek();
            }
        }
    }
}
