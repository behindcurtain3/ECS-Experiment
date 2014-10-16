using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Entities;
using IsoECS.Components;
using IsoECS.DataStructures;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using IsoECS.Components.GamePlay;

namespace IsoECS.Behaviors
{
    public class FindPathBehavior : Behavior
    {
        // the ID of the target entity to move towards
        public int TargetID { get; set; }

        // if true just find a path to a road tile next to the target
        // if false find a path to the exact position
        public bool MoveToNearbyRoad { get; set; }

        // the path generated
        public Path GeneratedPath { get; private set; }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            Entity target = em.Entities.Find(delegate(Entity e) { return e.ID == TargetID; });

            // if invalid target fail out
            if (target == null)
            {
                Status = BehaviorStatus.FAILURE;
                return;
            }

            // make sure the target has a position or foundation
            if (!MoveToNearbyRoad)
            {
                // if the target or self has no position fail out
                if (!target.HasComponent<PositionComponent>() || !self.HasComponent<PositionComponent>())
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
                }

                PositionComponent tPosition = target.Get<PositionComponent>();
                PositionComponent sPosition = self.Get<PositionComponent>();

                if (tPosition.Index == null || tPosition.Index == Point.Zero)
                    tPosition.Index = Isometric.GetPointAtScreenCoords(em.Map, (int)tPosition.X, (int)tPosition.Y);

                GeneratedPath = Pathfinder.Generate(em.Collisions, em.Map, sPosition.Index, tPosition.Index);

                // TODO: check for tPosition = sPosition
                if (GeneratedPath.Waypoints.Count == 0)
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
                }

                Status = BehaviorStatus.SUCCESS;
            }
            else
            {
                // check for a starting position and ensure the target has a foundation
                if (!self.HasComponent<PositionComponent>() || !target.HasComponent<FoundationComponent>() || !target.HasComponent<PositionComponent>())
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
                }

                PositionComponent tPosition = target.Get<PositionComponent>();
                FoundationComponent foundation = target.Get<FoundationComponent>();

                // list to hold valid road tiles to move to
                List<Point> validLandings = new List<Point>();

                foreach (LocationValue plot in foundation.Plan)
                {
                    // check the ortho points around the plot position
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            if (Math.Abs(x) == Math.Abs(y))
                                continue;

                            Point p = new Point(tPosition.Index.X + plot.Offset.X + x, tPosition.Index.Y + plot.Offset.Y + y);

                            if (!Isometric.ValidIndex(em.Map, p.X, p.Y))
                                continue;

                            if((!em.Foundations.SpaceTaken.ContainsKey(p) || !em.Foundations.SpaceTaken[p]) && !validLandings.Contains(p))
                                validLandings.Add(p);
                        }
                    }
                }

                if (validLandings.Count == 0)
                {
                    Status = BehaviorStatus.FAILURE;
                    return;
                }
                else
                {
                    // find a multi-path
                    PositionComponent sPosition = self.Get<PositionComponent>();
                    GeneratedPath = Pathfinder.Generate(em.Collisions, em.Map, sPosition.Index, validLandings);

                    // TODO: check for tPosition = sPosition
                    if (GeneratedPath.Waypoints.Count == 0)
                    {
                        Status = BehaviorStatus.FAILURE;
                        return;
                    }

                    Status = BehaviorStatus.SUCCESS;
                }
            }
        }

    }
}
