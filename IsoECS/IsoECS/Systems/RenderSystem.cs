using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IsoECS.Systems
{
    public class RenderSystem
    {
        public GraphicsDevice Graphics { get; set; }
        public Color ClearColor { get; set; }

        public void Draw(List<Entity> entities, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Get the list of drawable text entities from the main list
            List<Entity> drawables = entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); });
            List<Entity> drawableText = entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableTextComponent>() && e.HasComponent<PositionComponent>(); });

            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin();

            // TODO: render any background
            // TODO: render the map
            // TODO: render any sprites
            foreach (Entity e in drawables)
            {
                // if this entity is not visible continue to next
                if (!e.Get<DrawableComponent>().Visible) continue;

                spriteBatch.Draw(e.Get<DrawableComponent>().Texture, e.Get<PositionComponent>().Position, e.Get<DrawableComponent>().Source, e.Get<DrawableComponent>().Color);
            }

            // Render the text entities
            string text;
            Vector2 position;
            Color color;
            
            foreach (Entity e in drawableText)
            {
                // get the needed data
                text = e.Get<DrawableTextComponent>().Text;
                position = e.Get<PositionComponent>().Position;
                color = e.Get<DrawableTextComponent>().Color;
                
                // do the draw call
                spriteBatch.DrawString(spriteFont, text, position, color);
            }

            // end the scene
            spriteBatch.End();
        }
    }
}
