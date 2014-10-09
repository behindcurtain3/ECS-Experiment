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

        public void Draw(List<Entity> entities, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Get the list of drawable text entities from the main list
            List<Entity> drawables = entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); });
            List<Entity> drawableText = entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableTextComponent>() && e.HasComponent<PositionComponent>(); });
            PositionComponent cameraPosition = entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>() && e.HasComponent<PositionComponent>(); }).Get<PositionComponent>();

            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin();

            // TODO: render any background
            // TODO: this should be cached and only updated when drawbles are added, removed or changed
            List<DrawData> allDrawables = new List<DrawData>();
            DrawData dd;
            foreach (Entity e in drawables)
            {
                foreach (IGameDrawable d in e.Get<DrawableComponent>().Drawables)
                {
                    dd = new DrawData();
                    dd.Position = e.Get<PositionComponent>();
                    dd.Drawable = d;
                    allDrawables.Add(dd);
                }
            }

            // roughly good enough for now
            allDrawables.Sort(delegate(DrawData a, DrawData b)
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
            
            foreach (DrawData d in allDrawables)
            {
                // if the drawable is not visible continue
                if (!d.Drawable.Visible) continue;

                d.Drawable.Draw(Graphics, spriteBatch, (int)(d.Position.X - cameraPosition.X), (int)(d.Position.Y - cameraPosition.Y));
            }

            // Render the text entities
            // TODO: update this
            string text;
            Vector2 position;
            Color color;
            
            foreach (Entity e in drawableText)
            {
                if (!e.Get<DrawableTextComponent>().Visible)
                    continue;

                // get the needed data
                text = e.Get<DrawableTextComponent>().Text;
                position = e.Get<PositionComponent>().Position;
                color = e.Get<DrawableTextComponent>().Color;
                
                // do the draw call
                if(!string.IsNullOrWhiteSpace(text))
                    spriteBatch.DrawString(spriteFont, text, position, color);
            }

            // end the scene
            spriteBatch.End();
        }
    }
}
