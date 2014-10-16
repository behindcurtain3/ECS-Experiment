using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;

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
                fhb.Init(em, self);
                state.Push(fhb);
                return;
            }
        }
    }
}
