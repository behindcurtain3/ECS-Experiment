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
        private FoundationPlannerComponent _foundationPlanner;

        public void Init(EntityManager em)
        {
            _dataTracker = em.Entities.Find(delegate(Entity e) { return e.HasComponent<RoadPlannerComponent>(); });

            Entity inputEntity = em.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            Entity cameraEntity = em.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });
            Entity mapEntity = em.Entities.Find(delegate(Entity e) { return e.HasComponent<IsometricMapComponent>(); });

            _camera = cameraEntity.Get<PositionComponent>();
            _map = mapEntity.Get<IsometricMapComponent>();
            _input = inputEntity.Get<InputController>();
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

            SelectEntity();
        }

        public void Shutdown(EntityManager em)
        {
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            
            foreach (List<IGameDrawable> d in drawable.Drawables.Values)
            {
                foreach (IGameDrawable gd in d)
                {
                    gd.Alpha = 1.0f;
                    gd.Visible = true;
                }
            }

            drawable.Drawables.Clear();
        }

        public void Update(EntityManager em, int dt)
        {
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            PositionComponent drawablePosition = _dataTracker.Get<PositionComponent>();

            List<string> categories = new List<string>(_db.Keys);
            if (_input.CurrentKeyboard.IsKeyDown(Keys.OemPlus) && !_input.PrevKeyboard.IsKeyDown(Keys.OemPlus))
            {
                _selection++;

                if (_selection >= _db[categories[_category]].Count)
                    _selection = 0;

                SelectEntity();
            }

            Entity selectedEntity = _db[categories[_category]][_selection];
            BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
            FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();
            
            // set the starting coords
            int x = _input.CurrentMouse.X + (int)_camera.X;
            int y = _input.CurrentMouse.Y + (int)_camera.Y;

            // pick out the tile index that the screen coords intersect
            Point index = Isometric.GetPointAtScreenCoords(_map, x, y);

            // translate the index into a screen position and up the position component
            Vector2 dPositiion = Isometric.GetIsometricPosition(_map, 0, index.Y, index.X);
            drawablePosition.X = dPositiion.X;
            drawablePosition.Y = dPositiion.Y;

            bool spaceTaken = false;
            foreach (LocationValue lv in foundation.Plan)
            {
                if (_foundationPlanner.SpaceTaken.ContainsKey(new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y)))
                {
                    spaceTaken = true;
                    break;
                }
            }

            bool visible = Isometric.ValidIndex(_map, index.X, index.Y);

            foreach (List<IGameDrawable> d in drawable.Drawables.Values)
            {
                foreach (IGameDrawable gd in d)
                    gd.Visible = visible;
            }

            foreach (List<IGameDrawable> d in drawable.Drawables.Values)
            {
                foreach (IGameDrawable gd in d)
                    gd.Alpha = (spaceTaken) ? 0.5f : 0.85f;
            }
            
            if (!visible)
                return;
            
            if (_input.CurrentMouse.LeftButton == ButtonState.Pressed && (selectedBuildable.DragBuildEnabled || _input.PrevMouse.LeftButton != ButtonState.Pressed))
            {
                // don't build over a spot that is already taken
                if (spaceTaken)
                    return;

                Entity buildable = Serialization.DeepCopy<Entity>(selectedEntity);
                buildable.ResetID();

                if (buildable.HasComponent<PositionComponent>())
                {
                    PositionComponent aPosition = buildable.Get<PositionComponent>();
                    aPosition.X = drawablePosition.X;
                    aPosition.Y = drawablePosition.Y;
                    aPosition.Index = index;
                    aPosition.GenerateAt = string.Empty;
                }
                else
                {
                    PositionComponent bPosition = new PositionComponent(drawablePosition.Position);
                    bPosition.Index = index;
                    buildable.AddComponent(bPosition);
                }

                em.AddEntity(buildable);
            }
        }

        // called when the selected entity changes
        private void SelectEntity()
        {
            List<string> categories = new List<string>(_db.Keys);
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            Entity selectedEntity = _db[categories[_category]][_selection];
            BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
            DrawableComponent selectedDrawable = selectedEntity.Get<DrawableComponent>();
            FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();

            drawable.Drawables.Clear();

            foreach (KeyValuePair<string, List<IGameDrawable>> kvp in selectedDrawable.Drawables)
            {
                foreach (IGameDrawable gd in kvp.Value)
                {
                    IGameDrawable nd = Serialization.DeepCopy<IGameDrawable>(gd);
                    drawable.Add(kvp.Key, nd);
                }
            }
        }
    }
}
