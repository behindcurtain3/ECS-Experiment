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
            FloorplannerComponent floorPlanner = dataTracker.Get<FloorplannerComponent>();

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
                drawable.Color = new Color(255, 0, 0, 128);

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
                    Console.WriteLine(component.ToString());

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

                            // update the collision map
                            collisionMap.Collision[index] = 8;

                            // update the other roads
                            List<Entity> roadEntities = entities.FindAll(delegate(Entity e) { return e.HasComponent<RoadComponent>(); });
                            RoadsHelper.UpdateRoadsGfx(roadEntities, roadPlanner);
                            break;
                    }
                }

                // add the drawable part
                buildable.AddComponent(buildableDrawable);
                // add the entity
                entities.Add(buildable);

                // update the floor planner
                floorPlanner.SpaceTaken.Add(index, true);
            }
        }
    }
}
