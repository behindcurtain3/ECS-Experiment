using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using System.Reflection;
using System.Linq;
using IsoECS.GamePlay;
using IsoECS.DataStructures;
using System;
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

        private InputController _input;
        private PositionComponent _camera;

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
                Point index = Isometric.GetPointAtScreenCoords(em.Map, x, y);
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
                        ShowEntity(selectedEntities[0]);
                    }
                    else
                    {
                        // show the selection window
                        ShowSelection(selectedEntities);
                    }
                }
                else
                {
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
            em.UI.Remove(_selectionWindow);
            em.UI.Remove(_entityWindow);
        }

        private void ShowEntity(Entity e)
        {
            _selectionWindow.Hide();

            if(_entityTabs != null)
                _entityWindow.Remove(_entityTabs);

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

                if (component is Inventory)
                {
                    page.Text = "Inventory";
                    DisplayInventory((Inventory)component, page);
                }
                else if (component is StockpileComponent)
                {
                    page.Text = "Stockpile";
                    DisplayStockpile((StockpileComponent)component, page);
                }
                else
                    LabelAllProperties(component, page);
            }

            if(string.IsNullOrWhiteSpace(_entityWindow.Text))
                _entityWindow.Text = string.Format("#{0}", e.ID);
            _entityWindow.Add(_entityTabs);
            _entityWindow.Show();
            _entityWindow.Tag = e;

            if (_entityWindow.Left + _entityWindow.Width < 0)
            {
                _entityWindow.Left = _entityWindow.Manager.GraphicsDevice.Viewport.Width - _entityWindow.Width;
                _entityWindow.Top = _entityWindow.Manager.GraphicsDevice.Viewport.Height - _entityWindow.Height;
            }
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
                btn.Click += new EventHandler(btn_Click);

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

        private void btn_Click(object sender, EventArgs e)
        {
            ShowEntity((Entity)((Button)sender).Tag);
        }

        private bool ValidEntity(Entity e)
        {
            return e.HasComponent<PositionComponent>() &&
                (e.HasComponent<CitizenComponent>()
                || e.HasComponent<BuildableComponent>()
                || e.HasComponent<Inventory>());
        }

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

        private void DisplayInventory(Inventory inventory, TabPage page)
        {
            Label lbl;
            int y = 2;

            foreach (string i in inventory.Items.Keys)
            {
                Item item = GameData.Instance.GetItem(i);

                // name label
                lbl = new Label(page.Manager)
                {
                    Text = item.Name,
                    Left = 2,
                    Top = y,
                    Width = page.ClientWidth - 4,
                    Anchor = Anchors.Horizontal
                };
                page.Add(lbl);

                lbl = new Label(page.Manager)
                {
                    Text = inventory.Get(i).ToString(),
                    Left = 2,
                    Top = y,
                    Width = 150,
                    Alignment = Alignment.MiddleRight,
                };
                page.Add(lbl);

                y += 18;
            }
        }

        private void DisplayStockpile(StockpileComponent stockpile, TabPage page)
        {
            List<Item> items = GameData.Instance.GetAllItems();
            Label lbl;
            SpinBox spinner;
            int y = 2;
            int ySpacer = 24;
            int col1 = 2;
            int col4 = page.ClientWidth - 82;
            int col3 = col4 - 80;
            int col2 = col3 - 80;
            int col1Width = col2 - col1 - 5;

            lbl = new Label(page.Manager)
            {
                Text = "Item",
                Left = col1,
                Width = col1Width,
                Top = y,
                Height = 20,
                Alignment = Alignment.MiddleCenter,
                Anchor = Anchors.Horizontal | Anchors.Top
            };
            page.Add(lbl);

            lbl = new Label(page.Manager)
            {
                Text = "Amount",
                Left = col2,
                Width = 75,
                Top = y,
                Height = 20,
                Alignment = Alignment.MiddleCenter,
                Anchor = Anchors.Right | Anchors.Top
            };
            page.Add(lbl);

            lbl = new Label(page.Manager)
            {
                Text = "Minimum",
                Left = col3,
                Width = 75,
                Top = y,
                Height = 20,
                Alignment = Alignment.MiddleCenter,
                Anchor = Anchors.Right | Anchors.Top
            };
            page.Add(lbl);

            lbl = new Label(page.Manager)
            {
                Text = "Maximum",
                Left = col4,
                Width = 75,
                Top = y,
                Height = 20,
                Alignment = Alignment.MiddleCenter,
                Anchor = Anchors.Right | Anchors.Top
            };
            page.Add(lbl);

            y += ySpacer;

            foreach (Item item in items)
            {
                Button btn = new Button(page.Manager)
                {
                    Name = string.Format("{0}-toggle", item.UniqueID),
                    Text = item.Name,
                    Left = col1,
                    Top = y,
                    Width = col1Width,
                    Height = 20,
                    CanFocus = false,
                    Tag = item,
                    Anchor = Anchors.Horizontal | Anchors.Top
                };
                btn.Init();
                btn.Click += new EventHandler(ToggleStockPileItem);
                page.Add(btn);

                lbl = new Label(page.Manager)
                {
                    Name = string.Format("{0}-amount", item.UniqueID),
                    Text = stockpile.Amount(item.UniqueID).ToString(),
                    Left = col2,
                    Width = 75,
                    Top = y,
                    Height = 20,
                    Alignment = Alignment.MiddleCenter,
                    Anchor = Anchors.Right | Anchors.Top
                };
                page.Add(lbl);

                if(stockpile.IsAccepting(item.UniqueID))
                {
                    spinner = new SpinBox(page.Manager, SpinBoxMode.Range)
                    {
                        Name = string.Format("{0}-minimum", item.UniqueID),
                        Value = stockpile.Minimum(item.UniqueID),
                        Left = col3,
                        Width = 75,
                        Top = y,
                        Rounding = 0,
                        Minimum = 0,
                        Maximum = 5000,
                        Step = 1,
                        DisplayFormat = "f",
                        Anchor = Anchors.Right | Anchors.Top,
                        Tag = item
                    };
                    spinner.Init();
                    // spinner is bugged if Text is set above so do it here
                    spinner.Text = stockpile.Minimum(item.UniqueID).ToString();
                    spinner.TextChanged += new EventHandler(spinner_MinimumTextChanged);
                    page.Add(spinner);

                    spinner = new SpinBox(page.Manager, SpinBoxMode.Range)
                    {
                        Name = string.Format("{0}-maximum", item.UniqueID),
                        Value = stockpile.Maximum(item.UniqueID),
                        Left = col4,
                        Width = 75,
                        Top = y,
                        Rounding = 0,
                        Minimum = 0,
                        Maximum = 5000,
                        Step = 1,
                        DisplayFormat = "f",
                        Anchor = Anchors.Right | Anchors.Top,
                        Tag = item
                    };
                    spinner.Init();
                    // spinner is bugged if Text is set above so do it here
                    spinner.Text = stockpile.Maximum(item.UniqueID).ToString();
                    spinner.TextChanged += new EventHandler(spinner_MaximumTextChanged);
                    page.Add(spinner);
                }
                else
                {
                    lbl = new Label(page.Manager)
                    {
                        Text = "--- Not Accepting --- ",
                        Left = col3,
                        Width = 155,
                        Top = y,
                        Height = 20,
                        Alignment = Alignment.MiddleCenter,
                        Anchor = Anchors.Right | Anchors.Top
                    };
                    page.Add(lbl);
                }

                y += ySpacer;
            }
        }

        private void spinner_MinimumTextChanged(object sender, EventArgs e)
        {
            SpinBox spinner = (SpinBox)sender;
            Item item = (Item)spinner.Tag;
            Entity entity = (Entity)_entityWindow.Tag;

            if (entity.HasComponent<StockpileComponent>())
            {
                StockpileComponent stockpile = entity.Get<StockpileComponent>();

                stockpile.SetMinimum(item.UniqueID, (int)spinner.Value);
            }
        }

        private void spinner_MaximumTextChanged(object sender, EventArgs e)
        {
            SpinBox spinner = (SpinBox)sender;
            Item item = (Item)spinner.Tag;
            Entity entity = (Entity)_entityWindow.Tag;

            if (entity.HasComponent<StockpileComponent>())
            {
                StockpileComponent stockpile = entity.Get<StockpileComponent>();

                stockpile.SetMaximum(item.UniqueID, (int)spinner.Value);
            }
        }

        private void ToggleStockPileItem(object sender, EventArgs e)
        {
            Item item = (Item)((Button)sender).Tag;
            Entity entity = (Entity)_entityWindow.Tag;

            StockpileComponent stockpile = entity.Get<StockpileComponent>();
            stockpile.ToggleAccepting(item.UniqueID);

            foreach (Control c in _entityTabs.SelectedPage.Controls.ToList())
                _entityTabs.SelectedPage.Remove(c);

            DisplayStockpile(stockpile, _entityTabs.SelectedPage);
        }
    }
}
