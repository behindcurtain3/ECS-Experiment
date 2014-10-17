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
        private List<DrawData> _background = new List<DrawData>();
        private List<DrawData> _foundation = new List<DrawData>();
        private List<DrawData> _foreground = new List<DrawData>();
        private List<DrawData> _text = new List<DrawData>();

        public void Draw(EntityManager em, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Get the list of drawable text entities from the main list
            List<Entity> drawables = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); });
            PositionComponent cameraPosition = em.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>() && e.HasComponent<PositionComponent>(); }).Get<PositionComponent>();

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

            foreach (DrawData d in _background)
            {
                Draw(spriteBatch, spriteFont, d, cameraPosition);
            }
            foreach (DrawData d in _foundation)
            {
                Draw(spriteBatch, spriteFont, d, cameraPosition);
            }
            foreach (DrawData d in _foreground)
            {
                Draw(spriteBatch, spriteFont, d, cameraPosition);
            }
            foreach (DrawData d in _text)
            {
                Draw(spriteBatch, spriteFont, d, cameraPosition);
            }

            // end the scene
            spriteBatch.End();
        }

        private void AddToDrawables(List<DrawData> addTo, List<IGameDrawable> list, Entity e)
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

        private void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, DrawData dd, PositionComponent cameraPosition)
        {
            // if the drawable is not visible continue
            if (!dd.Drawable.Visible) return;

            if (dd.Drawable.Static)
                dd.Drawable.Draw(Graphics, spriteBatch, spriteFont, (int)dd.Position.X, (int)dd.Position.Y);
            else
                dd.Drawable.Draw(Graphics, spriteBatch, spriteFont, (int)(dd.Position.X - cameraPosition.X), (int)(dd.Position.Y - cameraPosition.Y));
        }

        private int SortDrawables(DrawData a, DrawData b)
        {
            return a.Position.Y.CompareTo(b.Position.Y);
        }
    }
}
