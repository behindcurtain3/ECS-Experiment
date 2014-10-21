using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;

namespace IsoECS.Systems.GamePlay
{
    public class ConstructionSystem : ISystem
    {
        public List<Keys> HotKeys { get; set; }

        private string _category;
        private int _selection = -1;

        private Dictionary<string, List<Entity>> _db;

        private Entity _dataTracker;

        private IsometricMapComponent _map;
        private PositionComponent _camera;
        private InputController _input;
        private FoundationPlannerComponent _foundationPlanner;

        private List<Button> _buttons = new List<Button>();
        private Manager _mgr;

        public void Init(EntityManager em)
        {
            HotKeys = new List<Keys>() { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9 };
            _mgr = em.UI;
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

            CreateCategoryButtons();
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
            ClearButtons();
        }

        public void Update(EntityManager em, int dt)
        {
            // a category has not been chosen
            if(string.IsNullOrWhiteSpace(_category))
            {
                // listen for hotkeys
                for (int i = 0; i < _buttons.Count; i++)
                {
                    if (_input.CurrentKeyboard.IsKeyDown(HotKeys[i]) && !_input.PrevKeyboard.IsKeyDown(HotKeys[i]))
                    {
                        DisplayCategory((string)_buttons[i].Tag);
                        break;
                    }
                }
            }
            // a category has been chosen but no selection
            else
            {
                // listen for hotkeys
                if (_input.CurrentKeyboard.IsKeyDown(Keys.Escape) && !_input.PrevKeyboard.IsKeyDown(Keys.Escape))
                {
                    _category = null;
                    _selection = -1;
                    DeselectEntity();
                    ClearButtons();
                    CreateCategoryButtons();
                    return;
                }
                for (int i = 0; i < _buttons.Count; i++)
                {
                    if (_input.CurrentKeyboard.IsKeyDown(HotKeys[i]) && !_input.PrevKeyboard.IsKeyDown(HotKeys[i]))
                    {
                        _selection = (int)_buttons[i].Tag;
                        SelectEntity();
                        break;
                    }
                }
            }

            if(string.IsNullOrWhiteSpace(_category) || _selection == -1)
                return;

            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            PositionComponent drawablePosition = _dataTracker.Get<PositionComponent>();

            Entity selectedEntity = _db[_category][_selection];
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
            Entity selectedEntity = _db[_category][_selection];
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

        private void DeselectEntity()
        {
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();

            drawable.Drawables.Clear();
        }

        private void ClearButtons()
        {
            foreach (Button btn in _buttons)
                _mgr.Remove(btn);

            _buttons.Clear();
        }

        private void CreateCategoryButtons()
        {
            Button btn;
            foreach (string category in _db.Keys)
            {
                btn = CreateButton(category);
                btn.Click += new EventHandler(CategoryBtn_Click);
                btn.Tag = category;

                _mgr.Add(btn);
                _buttons.Add(btn);
            }

            PositionButtons();
        }

        private void CreateButtonsForCategory(string category)
        {
            List<Entity> entities = _db[category];
            Button btn;

            foreach (Entity e in entities)
            {
                BuildableComponent buildable = e.Get<BuildableComponent>();

                btn = CreateButton(buildable.Name);
                btn.Click += new EventHandler(Selection_Click);
                btn.Tag = _buttons.Count;

                _mgr.Add(btn);
                _buttons.Add(btn);
            }

            PositionButtons();
        }

        private Button CreateButton(string text)
        {
            Button btn = new Button(_mgr)
            {
                Text = string.Format("{0}. {1}", (_buttons.Count + 1), text),
                Width = 125,
                Height = 20,
                Left = 5
            };
            btn.Init();

            return btn;
        }

        private void PositionButtons()
        {
            // TODO: height should be the graphics device viewport height
            int height = 400;

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Top = height - ((_buttons.Count - i) * _buttons[i].Height) + (i * 3);
            }
        }

        private void CategoryBtn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string category = (string)btn.Tag;
            DisplayCategory(category);
        }

        private void Selection_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            _selection = (int)btn.Tag;
            SelectEntity();
        }

        private void DisplayCategory(string category)
        {
            ClearButtons();
            CreateButtonsForCategory(category);
            _category = category;
        }
    }
}
