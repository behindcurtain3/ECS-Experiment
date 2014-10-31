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

        public override BehaviorStatus Update(Entity self, int dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            switch (status)
            {
                // child returned a success
                case BehaviorStatus.SUCCESS:
                    if (Finished is ExitBuildingBehavior)
                    {
                        ExitBuildingBehavior exit = (ExitBuildingBehavior)Finished;

                        // make sure the citizen starts at the right position
                        PositionComponent position = self.Get<PositionComponent>();
                        Vector2 startAt = EntityManager.Instance.Map.GetPositionFromIndex(exit.SelectedPath.Start.X, exit.SelectedPath.Start.Y);
                        position.X = startAt.X;
                        position.Y = startAt.Y;
                        position.Index = exit.SelectedPath.Start;

                        GoToBehavior g2b = new GoToBehavior()
                        {
                            GeneratedPath = exit.SelectedPath,
                            TargetID = exit.TargetID
                        };
                        AddChild(g2b);
                    }
                    break;

                case BehaviorStatus.FAIL:

                    break;

                case BehaviorStatus.RUN:
                    // look for a secondary behavior to do
                    CitizenComponent citizen = self.Get<CitizenComponent>();

                    // TODO: need to implement "archetype" behaviors like: child, student, adult, retired etc
                    // these top-level behaviors will manage the needs and invoke appropriate sub-behaviors

                    // the citizen is homeless, find them a home!
                    if (citizen.HousingID == -1 && PreviousBehavior.GetType() != typeof(FindHomeBehavior))
                    {
                        AddChild(new FindHomeBehavior());
                    }
                    else if (citizen.JobID == -1 && citizen.HousingID != -1 && PreviousBehavior.GetType() != typeof(FindJobBehavior))
                    {
                        AddChild(new FindJobBehavior());
                    }
                    else
                    {
                        // do the hauling job
                        if (citizen.IsHauler && citizen.JobID != -1)
                        {
                            AddChild(new HaulerBehavior());
                        }
                        // go to work
                        //else if (citizen.JobID != -1 && PreviousBehavior.GetType() == typeof(IdleBehavior) && citizen.InsideID != citizen.JobID)
                        //{
                        //    state.Push(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.JobID });
                        //    PreviousBehavior = state.Peek();
                        //}
                        // go home
                        else if (citizen.HousingID != -1 && PreviousBehavior.GetType() == typeof(IdleBehavior) && citizen.InsideID != citizen.HousingID)
                        {
                            AddChild(new ExitBuildingBehavior() { ExitID = citizen.InsideID, TargetID = citizen.HousingID });
                        }
                        else
                        {
                            AddChild(new IdleBehavior());
                        }
                    }
                    PreviousBehavior = Child;
                    break;
            }           

            return BehaviorStatus.RUN;
        }
    }
}
