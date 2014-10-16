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
        private List<DrawData> _allDrawables = new List<DrawData>();

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
            _allDrawables.Clear();
            DrawData dd;
            foreach (Entity e in drawables)
            {
                foreach (IGameDrawable d in e.Get<DrawableComponent>().Drawables)
                {
                    dd = new DrawData();
                    dd.Position = e.Get<PositionComponent>();
                    dd.Drawable = d;
                    _allDrawables.Add(dd);
                }
            }

            // roughly good enough for now
            _allDrawables.Sort(delegate(DrawData a, DrawData b)
            {
                // sort by Y position
                if (b.Drawable.Layer == a.Drawable.Layer)
                {
                    return a.Position.Y.CompareTo(b.Position.Y);
                }
                else
                {
                    return b.Drawable.Layer.CompareTo(a.Drawable.Layer);
                }
            });
            
            foreach (DrawData d in _allDrawables)
            {
                // if the drawable is not visible continue
                if (!d.Drawable.Visible) continue;

                if(d.Drawable.Static)
                    d.Drawable.Draw(Graphics, spriteBatch, spriteFont, (int)d.Position.X, (int)d.Position.Y);
                else
                    d.Drawable.Draw(Graphics, spriteBatch, spriteFont, (int)(d.Position.X - cameraPosition.X), (int)(d.Position.Y - cameraPosition.Y));
            }

            // end the scene
            spriteBatch.End();
        }
    }
}
