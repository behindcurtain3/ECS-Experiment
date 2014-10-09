using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using IsoECS.DataStructures.Json;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IsoECS.DataStructures.Json.Converters;
using IsoECS.DataStructures.Json.Deserializable;

namespace IsoECS.Systems.GamePlay
{
    public class ConstructionSystem : ISystem
    {
        private int _category = 0;
        private int _selection = 0;

        public void Update(List<Entity> entities, int dt)
        {
            Entity inputEntity = entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });

            if (inputEntity == null)
                return;

            Entity cameraEntity = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });
            Entity dataTracker = entities.Find(delegate(Entity e) { return e.HasComponent<RoadplannerComponent>(); }); 

            PositionComponent camera = cameraEntity.Get<PositionComponent>();
            IsometricMapComponent map = mapEntity.Get<IsometricMapComponent>();
            InputController input = inputEntity.Get<InputController>();
            DrawableComponent drawable = dataTracker.Get<DrawableComponent>();
            PositionComponent drawablePosition = dataTracker.Get<PositionComponent>();
            RoadplannerComponent roadPlanner = dataTracker.Get<RoadplannerComponent>();
            CollisionMapComponent collisionMap = dataTracker.Get<CollisionMapComponent>();
            FoundationPlannerComponent floorPlanner = dataTracker.Get<FoundationPlannerComponent>();

            List<string> categories = Buildables.Instance.GetCategories();
            if (input.CurrentKeyboard.IsKeyDown(Keys.OemPlus) && !input.PrevKeyboard.IsKeyDown(Keys.OemPlus))
            {
                _selection++;

                if (_selection >= Buildables.Instance.GetCategory(categories[_category]).Count)
                    _selection = 0;
            }
            List<BuildableInfo> buildings = Buildables.Instance.GetCategory(categories[_category]);
            BuildableInfo selectedBuildable = buildings[_selection];

            // set the starting coords
            int x = input.CurrentMouse.X + (int)camera.X;
            int y = input.CurrentMouse.Y + (int)camera.Y;

            // pick out the tile index that the screen coords intersect
            Point index = Isometric.GetPointAtScreenCoords(map, x, y);

            // TODO: only update the drawable when first shown or the selection has changed
            // translate the index into a screen position
            Vector2 dPositiion = Isometric.GetIsometricPosition(map, 0, index.Y, index.X);
            drawablePosition.X = dPositiion.X;
            drawablePosition.Y = dPositiion.Y;

            if (!floorPlanner.SpaceTaken.ContainsKey(index))
                drawable.Color = new Color(drawable.Color.R, drawable.Color.G, drawable.Color.B, 228);
            else
                drawable.Color = new Color(128, 128, 128, 128);

            drawable.Texture = Textures.Instance.Get(selectedBuildable.ConstructSpriteSheetName);
            drawable.Source = Textures.Instance.GetSource(selectedBuildable.ConstructSpriteSheetName, selectedBuildable.ConstructSourceID);
            drawable.Visible = Isometric.ValidIndex(map, index.X, index.Y);
            drawable.Origin = Textures.Instance.GetOrigin(selectedBuildable.ConstructSpriteSheetName, selectedBuildable.ConstructSourceID);

            if (!drawable.Visible)
                return;

            if (input.CurrentMouse.LeftButton == ButtonState.Pressed && (selectedBuildable.DragBuildEnabled || input.PrevMouse.LeftButton != ButtonState.Pressed))
            {
                // don't build over a spot that is already taken
                if (floorPlanner.SpaceTaken.ContainsKey(index))
                    return;

                // TODO: add larger footprint support
                // Construct the buildable
                Entity buildable = new Entity();
                DrawableComponent buildableDrawable = new DrawableComponent()
                {
                    Layer = 2,
                    Visible = true,
                    Texture = Textures.Instance.Get(selectedBuildable.SpriteSheetName),
                    Source = Textures.Instance.GetSource(selectedBuildable.SpriteSheetName, selectedBuildable.SourceID),
                    Origin = Textures.Instance.GetOrigin(selectedBuildable.SpriteSheetName, selectedBuildable.SourceID)
                };
                buildable.AddComponent(new PositionComponent(drawablePosition.Position));
                
                // add misc components
                foreach (JObject component in selectedBuildable.Components)
                {
                    JToken tokenName;
                    if (!component.TryGetValue("Type", out tokenName))
                        continue;

                    string typeName = tokenName.ToString();

                    // instantiate the component
                    Component c;
                    try
                    {
                        Type type = Type.GetType("IsoECS.Components.GamePlay." + typeName);
                        c = (Component)Activator.CreateInstance(type);
                    }
                    catch(ArgumentNullException ex)
                    {
                        Console.WriteLine("Unable to load component: " + typeName);
                        Console.WriteLine(ex.StackTrace);
                        continue;
                    }

                    // do a custom action based on the type below
                    switch (typeName)
                    {
                        case "RoadComponent":
                            // setup the road component
                            RoadComponent road = (RoadComponent)c;
                            road.BuiltAt = index;

                            // add it to the new entity
                            buildable.AddComponent(road);

                            // update the roads
                            RoadsHelper.AddOrUpdateRoad(roadPlanner, map, index, true);

                            buildableDrawable.Source = Textures.Instance.GetSource(selectedBuildable.SpriteSheetName, roadPlanner.Built[index]);
                            buildableDrawable.Origin = Textures.Instance.GetOrigin(selectedBuildable.SpriteSheetName, roadPlanner.Built[index]);
                            buildableDrawable.Layer = 89;

                            // update the other roads
                            List<Entity> roadEntities = entities.FindAll(delegate(Entity e) { return e.HasComponent<RoadComponent>(); });
                            RoadsHelper.UpdateRoadsGfx(roadEntities, roadPlanner);
                            break;
                        case "FoundationComponent":
                            FoundationComponent floor = (FoundationComponent)c;

                            // TODO: implement a better way to handle this crap
                            // TODO: all this json reading crap needs to happen when the buildable
                            // is first read and stored in the BuildableInfo
                            JToken plan;

                            // if using floorplan setup the offsets
                            if (component.TryGetValue("FoundationPlan", out plan))
                            {
                                IEnumerable<DeserializablePoint> list = component["FoundationPlan"].ToObject<IEnumerable<DeserializablePoint>>();

                                foreach (DeserializablePoint p in list)
                                {
                                    floor.FloorPlan.Add(p.Value);
                                }
                            }
                            // if using a fill pattern
                            else if (component.TryGetValue("FillFoundationPlan", out plan))
                            {
                                List<DeserializablePoint> flist = new List<DeserializablePoint>(component["FillFoundationPlan"].ToObject<IEnumerable<DeserializablePoint>>());
                                Point start = flist[0].Value;
                                Point end = flist[1].Value;
                                
                                for (int xx = start.X; xx <= end.X; xx++)
                                {
                                    for (int yy = start.Y; yy <= end.Y; yy++)
                                    {
                                        floor.FloorPlan.Add(new Point(xx, yy));
                                    }
                                }
                            }

                            // update the floor planner
                            foreach (Point p in floor.FloorPlan)
                            {
                                Point update = new Point(index.X + p.X, index.Y + p.Y);
                                floorPlanner.SpaceTaken.Add(update, true);
                            }

                            buildable.AddComponent(floor);
                            break;

                        case "CollisionComponent":
                            CollisionComponent collision = (CollisionComponent)c;

                            IEnumerable<LocationValue> planList = component["Plan"].ToObject<IEnumerable<LocationValue>>();

                            foreach (LocationValue lv in planList)
                            {
                                Point p = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);

                                if (collisionMap.Collision.ContainsKey(p))
                                    collisionMap.Collision[p] = lv.Value;
                                else
                                    collisionMap.Collision.Add(p, lv.Value);
                            }
                            break;
                    }
                }

                // add the drawable part
                buildable.AddComponent(buildableDrawable);
                // add the entity
                entities.Add(buildable);
            }
        }
    }
}
