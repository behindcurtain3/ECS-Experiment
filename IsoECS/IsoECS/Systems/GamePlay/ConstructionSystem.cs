using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TecsDotNet;
using TecsDotNet.Util;
using TomShane.Neoforce.Controls;

namespace IsoECS.Systems.GamePlay
{
    public class ConstructionSystem : GameSystem
    {
        private string _category;
        private int _selection = -1;

        private Dictionary<string, List<Entity>> _db;

        private Entity _dataTracker;
        private PositionComponent _camera;

        private List<Button> _buttons = new List<Button>();

        private double delay = 0.2;
        private double delayCountdown;

        public override void Init()
        {
            base.Init();

            _dataTracker = World.Entities.Find(delegate(Entity e) { return e.HasComponent<RoadPlannerComponent>(); });

            Entity inputEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            Entity cameraEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });

            _camera = cameraEntity.Get<PositionComponent>();

            _db = new Dictionary<string, List<Entity>>();

            List<Entity> allBuildables = World.Prototypes.GetAll<Entity>();

            foreach (Entity e in allBuildables)
            {
                if (!e.HasComponent<BuildableComponent>())
                    continue;

                BuildableComponent bc = e.Get<BuildableComponent>();

                if (!bc.AllowConstruction)
                    continue;

                if (!_db.ContainsKey(bc.Category))
                    _db.Add(bc.Category, new List<Entity>());

                _db[bc.Category].Add(e);
            }

            CreateCategoryButtons();

            World.Input.LeftClick += new InputController.MouseEventHandler(Instance_LeftClick);
            World.Input.LeftButtonDown += new InputController.MouseEventHandler(Instance_LeftButtonDown);
            World.Input.BuildBack.Event += new InputController.KeyboardEventHandler(BuildBack_Event);
            World.Input.BuildHotKey1.Event += new InputController.KeyboardEventHandler(BuildHotKey1_Event);
            World.Input.BuildHotKey2.Event += new InputController.KeyboardEventHandler(BuildHotKey2_Event);
            World.Input.BuildHotKey3.Event += new InputController.KeyboardEventHandler(BuildHotKey3_Event);
            World.Input.BuildHotKey4.Event += new InputController.KeyboardEventHandler(BuildHotKey4_Event);
            World.Input.BuildHotKey5.Event += new InputController.KeyboardEventHandler(BuildHotKey5_Event);
            World.Input.BuildHotKey6.Event += new InputController.KeyboardEventHandler(BuildHotKey6_Event);
            World.Input.BuildHotKey7.Event += new InputController.KeyboardEventHandler(BuildHotKey7_Event);
            World.Input.BuildHotKey8.Event += new InputController.KeyboardEventHandler(BuildHotKey8_Event);
            World.Input.BuildHotKey9.Event += new InputController.KeyboardEventHandler(BuildHotKey9_Event);
            World.Input.BuildHotKey0.Event += new InputController.KeyboardEventHandler(BuildHotKey0_Event);
        }

        public override void Shutdown()
        {
            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();

            foreach (List<GameDrawable> d in drawable.Drawables.Values)
            {
                foreach (GameDrawable gd in d)
                {
                    gd.Alpha = 1.0f;
                    gd.Visible = true;
                }
            }

            drawable.Drawables.Clear();
            ClearButtons();

            World.Input.LeftClick -= Instance_LeftClick;
            World.Input.LeftButtonDown -= Instance_LeftButtonDown;
            World.Input.BuildBack.Event -= BuildBack_Event;
            World.Input.BuildHotKey1.Event -= BuildHotKey1_Event;
            World.Input.BuildHotKey2.Event -= BuildHotKey2_Event;
            World.Input.BuildHotKey3.Event -= BuildHotKey3_Event;
            World.Input.BuildHotKey4.Event -= BuildHotKey4_Event;
            World.Input.BuildHotKey5.Event -= BuildHotKey5_Event;
            World.Input.BuildHotKey6.Event -= BuildHotKey6_Event;
            World.Input.BuildHotKey7.Event -= BuildHotKey7_Event;
            World.Input.BuildHotKey8.Event -= BuildHotKey8_Event;
            World.Input.BuildHotKey9.Event -= BuildHotKey9_Event;
            World.Input.BuildHotKey0.Event -= BuildHotKey0_Event;
        }

        private void BuildBack_Event(Keys key, InputEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_category))
            {
                _category = null;
                _selection = -1;
                DeselectEntity();
                ClearButtons();
                CreateCategoryButtons();
            }
        }

        private void BuildHotKey1_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(0);
        }

        private void BuildHotKey2_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(1);
        }

        private void BuildHotKey3_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(2);
        }

        private void BuildHotKey4_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(3);
        }

        private void BuildHotKey5_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(4);
        }

        private void BuildHotKey6_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(5);
        }

        private void BuildHotKey7_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(6);
        }

        private void BuildHotKey8_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(7);
        }

        private void BuildHotKey9_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(8);
        }

        private void BuildHotKey0_Event(Keys key, InputEventArgs e)
        {
            TriggerHotkey(9);
        }

        private void Instance_LeftClick(InputEventArgs e)
        {
            BuildSelected();
        }

        private void Instance_LeftButtonDown(InputEventArgs e)
        {
            BuildSelected(true);
        }

        private void TriggerHotkey(int key)
        {
            if (_buttons.Count > key)
            {
                if (string.IsNullOrWhiteSpace(_category))
                {
                    DisplayCategory((string)_buttons[key].Tag);
                }
                else
                {
                    _selection = (int)_buttons[key].Tag;
                    SelectEntity();
                }
            }
        }

        private void BuildSelected(bool drag = false)
        {
            if (string.IsNullOrWhiteSpace(_category) || _selection == -1 || delayCountdown > 0)
                return;

            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            PositionComponent drawablePosition = _dataTracker.Get<PositionComponent>();

            Entity selectedEntity = _db[_category][_selection];
            BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
            FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();

            if (!selectedBuildable.DragBuildEnabled && drag)
                return;

            // set the starting coords
            int x = World.Input.CurrentMouse.X + (int)_camera.X;
            int y = World.Input.CurrentMouse.Y + (int)_camera.Y;

            // pick out the tile index that the screen coords intersect
            Point index = World.Map.GetIndexFromPosition(x, y);

            // translate the index into a screen position and up the position component
            Vector2 dPositiion = World.Map.GetPositionFromIndex(index.X, index.Y);
            drawablePosition.X = dPositiion.X;
            drawablePosition.Y = dPositiion.Y;

            bool spaceTaken = false;
            foreach (LocationValue lv in foundation.Plan)
            {
                if (World.Foundations.SpaceTaken.ContainsKey(new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y)))
                {
                    spaceTaken = true;
                    break;
                }
            }

            bool visible = World.Map.IsValidIndex(index.X, index.Y);

            if (!visible)
                return;

            // don't build over a spot that is already taken, don't build if not enough money
            // TODO: the money check shouldn't even allow the building to be selected
            if (spaceTaken || World.CityInformation.Treasury < selectedBuildable.Cost)
                return;

            Entity buildable = Serialization.DeepCopy<Entity>(selectedEntity);

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

            World.Entities.Add(buildable);
            World.CityInformation.Treasury -= selectedBuildable.Cost;
            delayCountdown = delay;
        }
        
        public override void Update(double dt)
        {
            if (delayCountdown > 0)
                delayCountdown -= dt;

            if(string.IsNullOrWhiteSpace(_category) || _selection == -1)
                return;

            DrawableComponent drawable = _dataTracker.Get<DrawableComponent>();
            PositionComponent drawablePosition = _dataTracker.Get<PositionComponent>();

            Entity selectedEntity = _db[_category][_selection];
            BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
            FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();
            
            // set the starting coords
            int x = World.Input.CurrentMouse.X + (int)_camera.X;
            int y = World.Input.CurrentMouse.Y + (int)_camera.Y;

            // pick out the tile index that the screen coords intersect
            Point index = World.Map.GetIndexFromPosition(x, y);

            // translate the index into a screen position and up the position component
            Vector2 dPositiion = World.Map.GetPositionFromIndex(index.X, index.Y);
            drawablePosition.X = dPositiion.X;
            drawablePosition.Y = dPositiion.Y;

            bool spaceTaken = false;
            foreach (LocationValue lv in foundation.Plan)
            {
                if (World.Foundations.SpaceTaken.ContainsKey(new Point(index.X + lv.Offset.X, index.Y + lv.Offset.Y)))
                {
                    spaceTaken = true;
                    break;
                }
            }

            bool visible = World.Map.IsValidIndex(index.X, index.Y);

            foreach (List<GameDrawable> d in drawable.Drawables.Values)
            {
                foreach (GameDrawable gd in d)
                    gd.Visible = visible;
            }

            foreach (List<GameDrawable> d in drawable.Drawables.Values)
            {
                foreach (GameDrawable gd in d)
                    gd.Alpha = (spaceTaken) ? 0.5f : 0.85f;
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

            foreach (KeyValuePair<string, List<GameDrawable>> kvp in selectedDrawable.Drawables)
            {
                foreach (GameDrawable gd in kvp.Value)
                {
                    GameDrawable nd = Serialization.DeepCopy<GameDrawable>(gd);
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
                World.UI.Remove(btn);

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

                World.UI.Add(btn);
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

                World.UI.Add(btn);
                _buttons.Add(btn);
            }

            PositionButtons();
        }

        private Button CreateButton(string text)
        {
            Button btn = new Button(World.UI)
            {
                Text = string.Format("{0}. {1}", (_buttons.Count + 1), text),
                Width = 125,
                Height = 20,
                Left = 5,
                CanFocus = false
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
