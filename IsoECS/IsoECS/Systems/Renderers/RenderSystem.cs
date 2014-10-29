using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IsoECS.DataStructures;

namespace IsoECS.Systems
{
    public struct DrawData
    {
        public IGameDrawable Drawable;
        public PositionComponent Position;
    }

    public class RenderSystem : IRenderSystem
    {
        public GraphicsDevice Graphics { get; set; }
        public Color ClearColor { get; set; }
        protected List<DrawData> _background = new List<DrawData>();
        protected List<DrawData> _foundation = new List<DrawData>();
        protected List<DrawData> _foreground = new List<DrawData>();
        protected List<DrawData> _text = new List<DrawData>();

        protected List<Entity> drawables = new List<Entity>();
        protected PositionComponent cameraPosition;

        public void Init()
        {
            // Get the list of drawable text entities from the main list
            drawables.AddRange(EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); }));
            cameraPosition = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>() && e.HasComponent<PositionComponent>(); }).Get<PositionComponent>();

            EntityManager.Instance.EntityAdded += new EntityManager.EntityEventHandler(Instance_EntityAdded);
            EntityManager.Instance.EntityRemoved += new EntityManager.EntityEventHandler(Instance_EntityRemoved);
        }

        public void Shutdown()
        {
            drawables.Clear();
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // TODO: render any background
            // TODO: this should be cached and only updated when drawbles are added, removed or changed
            _background.Clear();
            _foundation.Clear();
            _foreground.Clear();
            _text.Clear();
            
            // TODO: separate the drawables out into their layers?
            // Draw order (back to front):
            // 1. Background
            // 2. Foundation
            // 3. Foreground
            // 4. Text
            foreach (Entity e in drawables)
            {   
                AddToDrawables(_background, e.Get<DrawableComponent>().Get("Background"), e);
                AddToDrawables(_foundation, e.Get<DrawableComponent>().Get("Foundation"), e);
                AddToDrawables(_foreground, e.Get<DrawableComponent>().Get("Foreground"), e);
                AddToDrawables(_text, e.Get<DrawableComponent>().Get("Text"), e);
            }

            // roughly good enough for now
            _background.Sort(SortDrawables);
            _foundation.Sort(SortDrawables);
            _foreground.Sort(SortDrawables);
            _text.Sort(SortDrawables);

            DrawBackgroundLayer(spriteBatch, spriteFont, cameraPosition, _background);
            DrawFoundationLayer(spriteBatch, spriteFont, cameraPosition, _foundation);
            DrawForegroundLayer(spriteBatch, spriteFont, cameraPosition, _foreground);
            DrawTextLayer(spriteBatch, spriteFont, cameraPosition, _text);

            // end the scene
            spriteBatch.End();
        }

        protected void AddToDrawables(List<DrawData> addTo, List<IGameDrawable> list, Entity e)
        {
            DrawData dd;
            foreach (IGameDrawable d in list)
            {
                dd = new DrawData();
                dd.Position = e.Get<PositionComponent>();
                dd.Drawable = d;
                addTo.Add(dd);
            }
        }

        protected void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, DrawData dd, PositionComponent cameraPosition)
        {
            // if the drawable is not visible continue
            if (!dd.Drawable.Visible) return;

            if (dd.Drawable.Static)
                dd.Drawable.Draw(Graphics, spriteBatch, spriteFont, (int)dd.Position.X, (int)dd.Position.Y);
            else
                dd.Drawable.Draw(Graphics, spriteBatch, spriteFont, (int)(dd.Position.X - cameraPosition.X), (int)(dd.Position.Y - cameraPosition.Y));
        }

        protected int SortDrawables(DrawData a, DrawData b)
        {
            return a.Position.Y.CompareTo(b.Position.Y);
        }

        protected virtual void DrawBackgroundLayer(SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> backgrounds)
        {
            foreach (DrawData d in backgrounds)
            {
                Draw(spriteBatch, spriteFont, d, camera);
            }
        }

        protected virtual void DrawFoundationLayer(SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> foundations)
        {
            foreach (DrawData d in foundations)
            {
                Draw(spriteBatch, spriteFont, d, camera);
            }
        }

        protected virtual void DrawForegroundLayer(SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> foregrounds)
        {
            foreach (DrawData d in foregrounds)
            {
                Draw(spriteBatch, spriteFont, d, camera);
            }
        }

        protected virtual void DrawTextLayer(SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> texts)
        {
            foreach (DrawData d in texts)
            {
                Draw(spriteBatch, spriteFont, d, camera);
            }
        }

        private void Instance_EntityRemoved(Entity e)
        {
            drawables.Remove(e);
        }

        private void Instance_EntityAdded(Entity e)
        {
            if (e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>())
                drawables.Add(e);
        }
    }
}
