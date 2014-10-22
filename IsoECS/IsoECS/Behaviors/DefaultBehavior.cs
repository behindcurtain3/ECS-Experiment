using System.Collections.Generic;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.Components;
using IsoECS.Util;
using Microsoft.Xna.Framework;

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

            // TODO: need to implement "archetype" behaviors like: child, student, adult, retired etc
            // these top-level behaviors will manage the needs and invoke appropriate sub-behaviors

            // the citizen is homeless, find them a home!
            if (citizen.HousingID == -1 && PreviousBehavior.GetType() != typeof(FindHomeBehavior))
            {
                state.Push(new FindHomeBehavior());
                PreviousBehavior = state.Peek();
            }
            else if (citizen.JobID == -1 && citizen.HousingID != -1 && PreviousBehavior.GetType() != typeof(FindJobBehavior))
            {
                state.Push(new FindJobBehavior());
                PreviousBehavior = state.Peek();
            }
            else
            {
                // go to work
                if (citizen.JobID != -1 && PreviousBehavior.GetType() == typeof(IdleBehavior) && citizen.InsideID != citizen.JobID)
                {
                    state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.JobID });
                    PreviousBehavior = state.Peek();
                }
                // go home
                else if (citizen.HousingID != -1 && PreviousBehavior.GetType() == typeof(IdleBehavior) && citizen.InsideID != citizen.HousingID)
                {
                    state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.HousingID });
                    PreviousBehavior = state.Peek();
                }
                else
                {
                    state.Push(new IdleBehavior());
                    PreviousBehavior = state.Peek();
                }
            }
        }

        public override void OnSubFinished(EntityManager em, Entity self, Behavior finished, Stack<Behavior> state)
        {
            base.OnSubFinished(em, self, finished, state);

            if (finished is ExitBuildingBehavior)
            {
                ExitBuildingBehavior exit = (ExitBuildingBehavior)finished;

                if (exit.Status == BehaviorStatus.SUCCESS)
                {
                    // make sure the citizen starts at the right position
                    PositionComponent position = self.Get<PositionComponent>();
                    Vector2 startAt = Isometric.GetIsometricPosition(em.Map, 0, exit.SelectedPath.Start.Y, exit.SelectedPath.Start.X);
                    position.X = startAt.X;
                    position.Y = startAt.Y;
                    position.Index = exit.SelectedPath.Start;

                    GoToBehavior g2b = new GoToBehavior()
                    {
                        GeneratedPath = exit.SelectedPath,
                        TargetID = exit.TargetID
                    };
                    state.Push(g2b);
                }
            }
        }
    }
}
