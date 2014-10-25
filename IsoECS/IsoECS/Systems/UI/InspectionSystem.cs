using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataRenderers;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.GamePlay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;

namespace IsoECS.Systems.UI
{
    public class InspectionSystem : ISystem
    {
        private Manager _manager;
        private Window _selectionWindow;
        private List<Button> _selectionButtons;
        private EntityRenderer _renderer;

        private InputController _input;
        private PositionComponent _camera;
        private EntityManager _em;

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
                    if(_renderer != null)
                        _renderer.Shutdown();

                    _selectionWindow.Hide();
                }
            }
        }

        public void Init(EntityManager em)
        {
            _em = em;
            _manager = em.UI;
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

            _selectionButtons = new List<Button>();
        }

        public void Shutdown(EntityManager em)
        {
            if (_renderer != null)
                _renderer.Shutdown();

            em.UI.Remove(_selectionWindow);
        }

        private void ShowEntity(Entity e)
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
                    _renderer = new EntityRenderer(e, _manager);
                    _renderer.Next.Click += new EventHandler(Next_Click);
                    _renderer.Previous.Click += new EventHandler(Previous_Click);
                }
                _renderer.Update(e);
            }
        }

        private void ShowSelection(List<Entity> entities)
        {
            if (_renderer != null)
                _renderer.Shutdown();

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
            ShowEntity((Entity)((Button)sender).Tag);
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            if (_renderer == null)
                return;

            Entity entity = _renderer.Data;
            int index = _em.Entities.IndexOf(entity);
            int previous = (index - 1 < 0) ? _em.Entities.Count - 1 : index - 1;

            while (previous != index)
            {
                if (!string.IsNullOrWhiteSpace(_em.Entities[previous].UniqueID))
                {
                    // do the check
                    if (_em.Entities[previous].UniqueID.Equals(entity.UniqueID))
                    {
                        _renderer.Update(_em.Entities[previous]);
                        return;
                    }
                }

                // increment the index
                previous--;

                if (previous < 0)
                    previous = _em.Entities.Count - 1;
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (_renderer == null)
                return;

            // get the next of the "same" entity
            // warehouse -> warehosue
            // housing -> housing
            // citizen -> citizen

            Entity entity = _renderer.Data;
            int index = _em.Entities.IndexOf(entity);
            int next = (index + 1 >= _em.Entities.Count) ? 0 : index + 1;

            while (next != index)
            {
                if (!string.IsNullOrWhiteSpace(_em.Entities[next].UniqueID))
                {
                    // do the check
                    if (_em.Entities[next].UniqueID.Equals(entity.UniqueID))
                    {
                        _renderer.Update(_em.Entities[next]);
                        return;
                    }
                }

                // increment the index
                next++;

                if (next >= _em.Entities.Count)
                    next = 0;
            }
        }

        private bool ValidEntity(Entity e)
        {
            return e.HasComponent<PositionComponent>() &&
                (e.HasComponent<CitizenComponent>()
                || e.HasComponent<BuildableComponent>()
                || e.HasComponent<Inventory>()
                || e.HasComponent<StockpileComponent>());
        }
    }
}
