using System;
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
using EventHandler = TomShane.Neoforce.Controls.EventHandler;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace IsoECS.Systems.GamePlay
{
    public class ConstructionSystem : GameSystem
    {
        private string selectedCategory;
        private int selectedEntityIndex = -1;

        private Dictionary<string, List<Entity>> indexedBuildables;

        private Entity dataTracker;
        private PositionComponent camera;

        private List<Button> buttons = new List<Button>();

        private double delay = 0.1;
        private double delayCountdown;

        private bool isDragging = false;
        private Point dragStart;
        private Point dragEnd;
        private List<Entity> draggableEntities = new List<Entity>();

        public override void Init()
        {
            base.Init();

            dataTracker = World.Entities.Find(delegate(Entity e) { return e.HasComponent<RoadPlannerComponent>(); });

            Entity inputEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); });
            Entity cameraEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); });

            camera = cameraEntity.Get<PositionComponent>();

            indexedBuildables = new Dictionary<string, List<Entity>>();

            List<Entity> allBuildables = World.Prototypes.GetAll<Entity>();

            foreach (Entity e in allBuildables)
            {
                if (!e.HasComponent<BuildableComponent>())
                    continue;

                BuildableComponent bc = e.Get<BuildableComponent>();

                if (!bc.AllowConstruction)
                    continue;

                if (!indexedBuildables.ContainsKey(bc.Category))
                    indexedBuildables.Add(bc.Category, new List<Entity>());

                indexedBuildables[bc.Category].Add(e);
            }

            CreateCategoryButtons();

            World.Input.LeftClick += new InputController.MouseEventHandler(Instance_LeftClick);
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
            DrawableComponent drawable = dataTracker.Get<DrawableComponent>();

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

            ClearDragState();
        }

        private void BuildBack_Event(Keys key, InputEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                ClearDragState();
                selectedCategory = null;
                selectedEntityIndex = -1;
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

        private void TriggerHotkey(int key)
        {
            if (buttons.Count > key)
            {
                if (string.IsNullOrWhiteSpace(selectedCategory))
                {
                    DisplayCategory((string)buttons[key].Tag);
                }
                else
                {
                    selectedEntityIndex = (int)buttons[key].Tag;
                    SelectEntity();
                }
            }
        }

        private void BuildSelected()
        {
            if (string.IsNullOrWhiteSpace(selectedCategory) || selectedEntityIndex == -1 || delayCountdown > 0)
                return;
            
            Entity selectedEntity = indexedBuildables[selectedCategory][selectedEntityIndex];
            BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
            FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();

            // pick out the tile index that the screen coords intersect
            Point index = GetMouseIndex();

            if (selectedBuildable.DragBuildEnabled)
            {
                if (isDragging)
                {
                    // actually do the build
                    foreach (Entity e in draggableEntities)
                    {
                        Entity buildable = Serialization.DeepCopy<Entity>(selectedEntity);

                        if (buildable.HasComponent<PositionComponent>())
                        {
                            PositionComponent p = buildable.Get<PositionComponent>();
                            p.X = e.Get<PositionComponent>().X;
                            p.Y = e.Get<PositionComponent>().Y;
                            p.Index = e.Get<PositionComponent>().Index;
                            p.GenerateAt = string.Empty;
                        }
                        else
                        {
                            buildable.AddComponent(Serialization.DeepCopy<PositionComponent>(e.Get<PositionComponent>()));
                        }

                        World.Entities.Add(buildable);
                        World.City.Funds -= selectedBuildable.Cost;
                    }

                    ClearDragState();
                }
                else
                {
                    isDragging = true;
                    dragStart = index;
                    dragEnd = new Point(-1, -1);

                    UpdateDraggables();
                }
            }
            else
            {
                DrawableComponent drawable = dataTracker.Get<DrawableComponent>();
                PositionComponent drawablePosition = dataTracker.Get<PositionComponent>();

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
                if (spaceTaken || World.City.Funds < selectedBuildable.Cost)
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
                World.City.Funds -= selectedBuildable.Cost;
                delayCountdown = delay;
            }
        }
        
        public override void Update(double dt)
        {
            if (delayCountdown > 0)
                delayCountdown -= dt;

            if(string.IsNullOrWhiteSpace(selectedCategory) || selectedEntityIndex == -1)
                return;

            if (isDragging)
            {
                UpdateDraggables();
            }
            else
            {

                DrawableComponent drawable = dataTracker.Get<DrawableComponent>();
                PositionComponent drawablePosition = dataTracker.Get<PositionComponent>();

                Entity selectedEntity = indexedBuildables[selectedCategory][selectedEntityIndex];
                BuildableComponent selectedBuildable = selectedEntity.Get<BuildableComponent>();
                FoundationComponent foundation = selectedEntity.Get<FoundationComponent>();

                // pick out the tile index that the screen coords intersect
                Point index = GetMouseIndex();

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
        }

        private void ClearDragState()
        {
            isDragging = false;

            // remove current draggableEntities from the world
            foreach (Entity e in draggableEntities)
                World.Entities.Remove(e);

            // clear the list
            draggableEntities.Clear();
        }

        private void UpdateDraggables()
        {
            Point currentIndex = GetMouseIndex();

            // do the update
            if (dragEnd != currentIndex)
            {
                // remove current draggableEntities from the world
                foreach (Entity e in draggableEntities)
                    World.Entities.Remove(e);

                // clear the list
                draggableEntities.Clear();

                dragEnd = currentIndex;
                // 0 = same tile, 1 NW, 2 = NE, 3 = SE, 4 = SW, 5 = wtf to do
                int xDiff = Math.Abs(dragStart.X - dragEnd.X);
                int yDiff = Math.Abs(dragStart.Y - dragEnd.Y);
                if(xDiff == yDiff)
                    xDiff++;

                int direction = 
                    (dragStart == dragEnd) ? 0
                    : (xDiff > yDiff && dragEnd.X < dragStart.X) ? 1
                    : (yDiff > xDiff && dragEnd.Y < dragStart.Y) ? 2
                    : (xDiff > yDiff && dragEnd.X > dragStart.X) ? 3
                    : (yDiff > xDiff && dragEnd.Y > dragStart.Y) ? 4 : 5;
                
                int startX, startY, endX, endY;
                switch (direction)
                {
                    case 0:
                        AddDraggableAt(dragStart);
                        break;

                    case 1:
                        startX = dragStart.X;
                        endX = dragEnd.X;

                        while (startX >= endX)
                        {
                            AddDraggableAt(new Point(startX, dragStart.Y));
                            startX--;
                        }
                        break;

                    case 2:
                        startY = dragStart.Y;
                        endY = dragEnd.Y;

                        while (startY >= endY)
                        {
                            AddDraggableAt(new Point(dragStart.X, startY));
                            startY--;
                        }
                        break;

                    case 3:
                        startX = dragStart.X;
                        endX = dragEnd.X;

                        while (startX <= endX)
                        {
                            AddDraggableAt(new Point(startX, dragStart.Y));
                            startX++;
                        }
                        break;

                    case 4:
                        startY = dragStart.Y;
                        endY = dragEnd.Y;

                        while (startY <= endY)
                        {
                            AddDraggableAt(new Point(dragStart.X, startY));
                            startY++;
                        }
                        break;
                }
            }
        }

        private void AddDraggableAt(Point index)
        {
            if (!World.Map.IsValidIndex(index.X, index.Y))
                return;

            if (World.Foundations.SpaceTaken.ContainsKey(index))
                return;

            Entity e = new Entity();
            e.AddComponent(Serialization.DeepCopy<DrawableComponent>(dataTracker.Get<DrawableComponent>()));
            e.AddComponent(Serialization.DeepCopy<PositionComponent>(dataTracker.Get<PositionComponent>()));

            PositionComponent p = e.Get<PositionComponent>();
            p.Index = new Point(index.X, index.Y);

            Vector2 v = World.Map.GetPositionFromIndex(index.X, index.Y);
            p.X = v.X;
            p.Y = v.Y;

            draggableEntities.Add(e);
            World.Entities.Add(e);
        }

        // called when the selected entity changes
        private void SelectEntity()
        {
            List<string> categories = new List<string>(indexedBuildables.Keys);
            DrawableComponent drawable = dataTracker.Get<DrawableComponent>();
            Entity selectedEntity = indexedBuildables[selectedCategory][selectedEntityIndex];
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
            DrawableComponent drawable = dataTracker.Get<DrawableComponent>();

            drawable.Drawables.Clear();
        }

        private void ClearButtons()
        {
            foreach (Button btn in buttons)
                World.UI.Remove(btn);

            buttons.Clear();
        }

        private void CreateCategoryButtons()
        {
            Button btn;
            foreach (string category in indexedBuildables.Keys)
            {
                btn = CreateButton(category);
                btn.Click += new EventHandler(CategoryBtn_Click);
                btn.Tag = category;

                World.UI.Add(btn);
                buttons.Add(btn);
            }

            PositionButtons();
        }

        private void CreateButtonsForCategory(string category)
        {
            List<Entity> entities = indexedBuildables[category];
            Button btn;

            foreach (Entity e in entities)
            {
                BuildableComponent buildable = e.Get<BuildableComponent>();

                btn = CreateButton(buildable.Name);
                btn.Click += new EventHandler(Selection_Click);
                btn.Tag = buttons.Count;

                World.UI.Add(btn);
                buttons.Add(btn);
            }

            PositionButtons();
        }

        private Button CreateButton(string text)
        {
            Button btn = new Button(World.UI)
            {
                Text = string.Format("{0}. {1}", (buttons.Count + 1), text),
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

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Top = height - ((buttons.Count - i) * buttons[i].Height) + (i * 3);
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
            selectedEntityIndex = (int)btn.Tag;
            SelectEntity();
        }

        private void DisplayCategory(string category)
        {
            ClearButtons();
            CreateButtonsForCategory(category);
            selectedCategory = category;
        }

        private Point GetMouseIndex()
        {
            // set the starting coords
            int x = World.Input.CurrentMouse.X + (int)camera.X;
            int y = World.Input.CurrentMouse.Y + (int)camera.Y;

            // pick out the tile index that the screen coords intersect
            return World.Map.GetIndexFromPosition(x, y);
        }
    }
}
