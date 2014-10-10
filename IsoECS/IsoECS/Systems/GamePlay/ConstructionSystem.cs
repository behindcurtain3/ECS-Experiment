using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IsoECS.Systems.GamePlay
{
    public class ConstructionSystem : ISystem
    {
        private int _category = 0;
        private int _selection = 0;

        private Dictionary<string, List<Entity>> _db;

        private Entity _dataTracker;

        private IsometricMapComponent _map;
        private PositionComponent _camera;
        private InputController _input;
        private RoadPlannerComponent _roadPlanner;
        private CollisionMapComponent _collisionMap;
        private FoundationPlannerComponent _foundationPlanner;

        public void Init(List<Entity> entities)
        {
            _dataTracker = entities.Find(delegate(Entity e) { return e.HasComponent<RoadPlannerComponent>(); });

            Entity inputEntity = entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            Entity cameraEntity = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity mapEntity = entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });

            _camera = cameraEntity.Get<PositionComponent>();
            _map = mapEntity.Get<IsometricMapComponent>();
            _input = inputEntity.Get<InputController>();
            _roadPlanner = _dataTracker.Get<RoadPlannerComponent>();
            _collisionMap = _dataTracker.Get<CollisionMapComponent>();
            _foundationPlanner = _dataTracker.Get<FoundationPlannerComponent>();

            _db = new Dictionary<string, List<Entity>>();

            List<Entity> allBuildables = EntityLibrary.Instance.GetAll<BuildableComponent>();

            foreach (Entity e in allBuildables)
            {
                BuildableComponent bc = e.Get<BuildableComponent>();

                if (!_db.ContainsKey(bc.Category))
                    _db.Add(bc.Category, new List<Entity>());

                _db[bc.Category].Add(e);
            }
        }

        public void Shutdown(List<Entity> entities)
        {
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            foreach(IGameDrawable sprite in drawable.Drawables)
                sprite.Visible = false;
        }

        public void Update(List<Entity> entities, int dt)
        {
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            PositionComponent drawablePosition = _dataTracker.Get<PositionComponent>();

            List<string> categories = new List<string>(_db.Keys);
            if (_input.CurrentKeyboard.IsKeyDown(Keys.OemPlus) && !_input.PrevKeyboard.IsKeyDown(Keys.OemPlus))
            {
                _selection++;

                if (_selection >= _db[categories[_category]].Count)
                    _selection = 0;
            }

            Entity selectedEntity = _db[categories[_category]][_selection];
            BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
            FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();
            
            // set the starting coords
            int x = _input.CurrentMouse.X + (int)_camera.X;
            int y = _input.CurrentMouse.Y + (int)_camera.Y;

            // pick out the tile index that the screen coords intersect
            Point index = Isometric.GetPointAtScreenCoords(_map, x, y);

            // TODO: only update the drawable when first shown or the selection has changed
            // translate the index into a screen position
            Vector2 dPositiion = Isometric.GetIsometricPosition(_map, 0, index.Y, index.X);
            drawablePosition.X = dPositiion.X;
            drawablePosition.Y = dPositiion.Y;

            bool spaceTaken = false;
            DrawableSprite sprite = (DrawableSprite)drawable.Drawables[0];

            foreach (LocationValue lv in foundation.Plan)
            {
                if (_foundationPlanner.SpaceTaken.ContainsKey(new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y)))
                {
                    spaceTaken = true;
                    break;
                }
            }
            
            if (!spaceTaken)
                sprite.Color = new Color(sprite.Color.R, sprite.Color.G, sprite.Color.B, 228);
            else
                sprite.Color = new Color(128, 128, 128, 128);

            sprite.SpriteSheet = selectedBuildable.ConstructSpriteSheetName;
            sprite.ID = selectedBuildable.ConstructSourceID;
            sprite.Visible = Isometric.ValidIndex(_map, index.X, index.Y);
            sprite.Layer = 1;

            if (!sprite.Visible)
                return;

            if (_input.CurrentMouse.LeftButton == ButtonState.Pressed && (selectedBuildable.DragBuildEnabled || _input.PrevMouse.LeftButton != ButtonState.Pressed))
            {
                // don't build over a spot that is already taken
                if (spaceTaken)
                    return;

                Entity buildable = selectedEntity.DeepCopy();
                buildable.AddComponent(new PositionComponent(drawablePosition.Position));
                entities.Add(buildable);

                // update the game data
                foreach (Component c in buildable.Components.Values)
                {
                    switch (c.GetType().Name)
                    {
                        case "CollisionComponent":
                            CollisionComponent collision = (CollisionComponent)c;

                            foreach (LocationValue lv in collision.Plan)
                            {
                                Point p = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);

                                if (_collisionMap.Collision.ContainsKey(p))
                                    _collisionMap.Collision[p] = lv.Value;
                                else
                                    _collisionMap.Collision.Add(p, lv.Value);
                            }
                            break;

                        case "FoundationComponent":
                            FoundationComponent floor = (FoundationComponent)c;

                            // update the floor planner
                            foreach (LocationValue lv in floor.Plan)
                            {
                                Point update = new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y);
                                _foundationPlanner.SpaceTaken.Add(update, true);
                            }
                            break;

                        case "RoadComponent":
                            // setup the road component
                            RoadComponent road = (RoadComponent)c;
                            road.BuiltAt = index;

                            // update the roads
                            RoadsHelper.AddOrUpdateRoad(_roadPlanner, _map, index, true);

                            // update the other roads
                            List<Entity> roadEntities = entities.FindAll(delegate(Entity e) { return e.HasComponent<RoadComponent>(); });
                            RoadsHelper.UpdateRoadsGfx(roadEntities, _roadPlanner);
                            break;
                    }
                }
            }
        }
    }
}
