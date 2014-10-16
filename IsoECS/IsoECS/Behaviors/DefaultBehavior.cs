using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class DefaultBehavior : Behavior
    {
        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            // look for a secondary behavior to do
            CitizenComponent citizen = self.Get<CitizenComponent>();

            // the citizen is homeless, find them a home!
            if (citizen.HousingID == -1)
            {
                FindHomeBehavior fhb = new FindHomeBehavior();
                state.Push(fhb);
                return;
            }
        }
    }
}
