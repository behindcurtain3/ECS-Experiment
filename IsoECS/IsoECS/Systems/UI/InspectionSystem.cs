using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataRenderers;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.GamePlay;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;

namespace IsoECS.Systems.UI
{
    public class InspectionSystem : ISystem
    {
        private Window _selectionWindow;
        private List<Button> _selectionButtons;
        private Window _entityWindow;
        private TabControl _entityTabs;

        private EntityRenderer _renderer;

        private InputController _input;
        private PositionComponent _camera;
        private EntityManager _em;

        private int _updateRate = 250;
        private int _updateCountdown;

        public void Update(EntityManager em, int dt)
        {
            if (_input.CurrentMouse.RightButton == ButtonState.Pressed && _input.PrevMouse.RightButton != ButtonState.Pressed)
            {
                foreach (Control c in em.UI.Controls)
                {
                    if (c.Passive || !c.Visible)
                        continue;

                    if (c.ControlRect.Contains((int)_input.CurrentMouse.X, (int)_input.CurrentMouse.Y))
                    {
                        return;
                    }
                }

                // find any entities at the index the mouse was clicked on
                // set the starting coords
                int x = _input.CurrentMouse.X + (int)_camera.X;
                int y = _input.CurrentMouse.Y + (int)_camera.Y;

                // pick out the tile index that the screen coords intersect
                Point index = em.Map.GetIndexFromPosition(x, y);
                List<Entity> potentialEntities = em.Entities.FindAll(ValidEntity);
                List<Entity> selectedEntities = new List<Entity>();

                foreach (Entity potential in potentialEntities)
                {
                    PositionComponent position = potential.Get<PositionComponent>();

                    if (position.Index == index)
                    {
                        selectedEntities.Add(potential);
                        continue;
                    }
                }

                Entity foundationEntity = null;
                if(em.Foundations.SpaceTaken.ContainsKey(index))
                    foundationEntity = em.GetEntity(em.Foundations.SpaceTaken[index]);

                if (foundationEntity != null && !selectedEntities.Contains(foundationEntity))
                    selectedEntities.Add(foundationEntity);

                if (selectedEntities.Count > 0)
                {
                    if (selectedEntities.Count == 1)
                    {
                        // show the entity!
                        ShowEntity(em, selectedEntities[0]);
                    }
                    else
                    {
                        // show the selection window
                        ShowSelection(selectedEntities);
                    }
                }
                else
                {
                    if(_renderer != null)
                        _renderer.Shutdown();
                    _selectionWindow.Hide();
                    _entityWindow.Hide();
                }
            }

            if (_entityWindow.Visible)
            {
                _updateCountdown -= dt;

                if (_updateCountdown <= 0)
                {
                    _updateCountdown += _updateRate;
                    // TODO: update the displayed tab
                }
            }
        }

        public void Init(EntityManager em)
        {
            _em = em;
            _input = em.Entities.Find(delegate(Entity e) { return e.HasComponent<InputController>(); }).Get<InputController>();
            _camera = em.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>(); }).Get<PositionComponent>();

            _selectionWindow = new Window(em.UI)
            {
                Visible = false,
                Text = "Inspector",
                AutoScroll = true,
                IconVisible = false,
                CloseButtonVisible = false,
                Height = 250,
                Width = 190,
                Left = -1000
            };
            _selectionWindow.Init();
            em.UI.Add(_selectionWindow);

            _entityWindow = new Window(em.UI)
            {
                Visible = false,
                AutoScroll = true,
                IconVisible = true,
                CloseButtonVisible = true,
                Text = "",
                Height = 300,
                Width = 450,
                Left = -1000,
                MinimumWidth = 350,
                MinimumHeight = 200
            };
            _entityWindow.Init();
            em.UI.Add(_entityWindow);

            _selectionButtons = new List<Button>();
        }

        public void Shutdown(EntityManager em)
        {
            if (_renderer != null)
                _renderer.Shutdown();

            em.UI.Remove(_selectionWindow);
            em.UI.Remove(_entityWindow);
        }

        private void ShowEntity(EntityManager em, Entity e)
        {
            _selectionWindow.Hide();

            if (e != null)
            {
                if (_renderer != null)
                {
                    _renderer.Shutdown();
                }
                else
                {
                    _renderer = new EntityRenderer(e, em.UI);
                }
                _renderer.Update(e);
            }

            return;
            /*

            if (e.HasComponent<StockpileComponent>())
            {
                if (_renderer != null)
                    _renderer.Shutdown(em.UI);

                _renderer = new StockpileRenderer(e.Get<StockpileComponent>());
                _renderer.GetControl(em.UI);
                return;
            }
            if (_entityTabs != null)
            {
                for (int i = 0; i < _entityTabs.TabPages.Length; i++)
                    ClearTabPageAndEvents(_entityTabs.TabPages[i]);

                _entityWindow.Remove(_entityTabs);
            }

            _entityTabs = new TabControl(_entityWindow.Manager)
            {   Anchor = Anchors.All,
                Top = 2,
                Left = 2,
                Width = _entityWindow.ClientWidth - 4,
                Height = _entityWindow.ClientHeight - 4
            };
            _entityTabs.Init();

            _entityWindow.Text = "";
            TabPage page;

            foreach (IsoECS.Components.Component component in e.Components.Values)
            {

                if (component is CitizenComponent)
                    _entityWindow.Text = ((CitizenComponent)component).Name;
                else if (component is BuildableComponent)
                    _entityWindow.Text = ((BuildableComponent)component).Name;

                if (component is DrawableComponent 
                    || component is CollisionComponent 
                    || component is FoundationComponent
                    || component is BuildableComponent
                    || component is PositionComponent)
                    continue;

                page = _entityTabs.AddPage(component.GetType().Name);
                page.Tag = component;

                if (component is Inventory)
                {
                    page.Text = "Inventory";
                    DisplayInventory((Inventory)component, page);
                }
                else
                    LabelAllProperties(component, page);
            }

            _entityWindow.Text = string.Format("{0} (#{1})", _entityWindow.Text, e.ID);
            _entityWindow.Add(_entityTabs);
            _entityWindow.Show();
            _entityWindow.Tag = e;

            if (_entityWindow.Left + _entityWindow.Width < 0)
            {
                _entityWindow.Left = _entityWindow.Manager.GraphicsDevice.Viewport.Width - _entityWindow.Width;
                _entityWindow.Top = _entityWindow.Manager.GraphicsDevice.Viewport.Height - _entityWindow.Height;
            }
             */
        }

        private void ShowSelection(List<Entity> entities)
        {
            _entityWindow.Hide();

            foreach (Button b in _selectionButtons)
                _selectionWindow.Remove(b);

            _selectionButtons.Clear();

            Button btn;
            foreach (Entity e in entities)
            {
                btn = new Button(_selectionWindow.Manager)
                {
                    Height = 20,
                    Width = _selectionWindow.ClientWidth - 4,
                    Left = 2,
                    Top = _selectionButtons.Count * 20 + _selectionButtons.Count * 3
                };
                btn.Init();
                btn.Tag = e;
                btn.Click += new EventHandler(SelectEntity_Click);

                if (e.HasComponent<CitizenComponent>())
                {
                    CitizenComponent citizen = e.Get<CitizenComponent>();
                    btn.Text = string.Format("{0} {1}", citizen.Name, citizen.FamilyName);
                }
                else if (e.HasComponent<BuildableComponent>())
                {
                    BuildableComponent buildable = e.Get<BuildableComponent>();
                    btn.Text = buildable.Name;
                }

                _selectionButtons.Add(btn);
                _selectionWindow.Add(btn);
            }

            _selectionWindow.Show();

            if (_selectionWindow.Left + _selectionWindow.Width < 0)
            {
                _selectionWindow.Center();
                _selectionWindow.Left = _selectionWindow.Manager.GraphicsDevice.Viewport.Width - _selectionWindow.Width;
            }
        }

        private void SelectEntity_Click(object sender, EventArgs e)
        {
            ShowEntity(_em, (Entity)((Button)sender).Tag);
        }

        private bool ValidEntity(Entity e)
        {
            return e.HasComponent<PositionComponent>() &&
                (e.HasComponent<CitizenComponent>()
                || e.HasComponent<BuildableComponent>()
                || e.HasComponent<Inventory>()
                || e.HasComponent<StockpileComponent>());
        }

        /// <summary>
        /// Uses reflection to display all properties of an object.
        /// Useful for debugging.
        /// </summary>
        /// <param name="obj">Object to display</param>
        /// <param name="page">TabPage to display the object on</param>
        private void LabelAllProperties(object obj, TabPage page)
        {
            int y = 2;
            Label lbl;
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                lbl = new Label(_entityWindow.Manager)
                {
                    Text = info.Name,
                    Left = 2,
                    Top = y,
                    Width = page.ClientWidth - 4,
                    Anchor = Anchors.Horizontal
                };
                page.Add(lbl);

                object value = info.GetValue(obj, null);
                string txt = (value == null) ? "null" : value.ToString();
                lbl = new Label(_entityWindow.Manager)
                {
                    Text = txt,
                    Alignment = Alignment.MiddleRight,
                    Left = 2,
                    Top = y,
                    Width = page.ClientWidth - 4,
                    Anchor = Anchors.Horizontal
                };
                page.Add(lbl);

                y += 18;
            }
        }

        /// <summary>
        /// Display an inventory component
        /// </summary>
        /// <param name="inventory">Inventory to display</param>
        /// <param name="page">TabPage to display on</param>
        private void DisplayInventory(Inventory inventory, TabPage page)
        {
            Label lbl;
            int y = 2;
            int ySpacer = 24;
            int col1 = 2;
            int col2 = page.ClientWidth - 77;

            // Loop through each item in the inventory and display it
            foreach (string i in inventory.Items.Keys)
            {
                Item item = GameData.Instance.GetItem(i);

                if (item == null)
                    continue;

                // name label
                lbl = new Label(page.Manager)
                {
                    Text = item.Name,
                    Left = col1,
                    Top = y,
                    Width = page.ClientWidth - 4,
                    Anchor = Anchors.Horizontal | Anchors.Top
                };
                page.Add(lbl);

                lbl = new Label(page.Manager)
                {
                    Text = inventory.Get(i).ToString(),
                    Left = col2,
                    Top = y,
                    Width = 75,
                    Alignment = Alignment.MiddleRight,
                    Anchor = Anchors.Right | Anchors.Top
                };
                page.Add(lbl);

                y += ySpacer;
            }
        }


        /// <summary>
        /// Clears a TabPage of all its controls and events
        /// </summary>
        /// <param name="page"></param>
        private void ClearTabPageAndEvents(TabPage page)
        {
            foreach (Control c in page.Controls.ToList())
            {
                page.Remove(c);
            }
        }
    }
}
