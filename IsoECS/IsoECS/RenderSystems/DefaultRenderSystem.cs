using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TecsDotNet;
using IsoECS.Components;
using IsoECS.DataStructures;
using Microsoft.Xna.Framework;

namespace IsoECS.RenderSystems
{
    public struct DrawData
    {
        public GameDrawable Drawable;
        public Entity Entity;
        public PositionComponent Position;
    }

    public class DefaultRenderSystem : RenderSystem
    {
        private string[] supportedLayers = new string[] { "Background", "Foundation", "Foreground", "Text" };

        protected Dictionary<string, List<DrawData>> drawData = new Dictionary<string, List<DrawData>>();
        protected List<Entity> drawables = new List<Entity>();
        protected PositionComponent cameraPosition;

        public override void Init()
        {
            base.Init();

            // Get the list of drawable text entities from the main list
            drawables.AddRange(World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>(); }));
            foreach (string layer in supportedLayers)
            {
                drawData.Add(layer, new List<DrawData>());
            }

            cameraPosition = World.Entities.Find(delegate(Entity e) { return e.HasComponent<CameraController>() && e.HasComponent<PositionComponent>(); }).Get<PositionComponent>();

            World.Entities.EntityAdded += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityAdded);
            World.Entities.EntityRemoved += new TecsDotNet.Managers.EntityManager.EntityEventHandler(Entities_EntityRemoved);
        }

        public override void Shutdown()
        {
            drawables.Clear();
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Setup the scene
            Graphics.Clear(ClearColor);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // clear the layers
            foreach (KeyValuePair<string, List<DrawData>> kpv in drawData)
                kpv.Value.Clear();

            // add the drawables
            foreach (Entity e in drawables)
            {
                foreach (KeyValuePair<string, List<DrawData>> layer in drawData)
                    AddToDrawables(layer.Value, layer.Key, e);
            }

            // sort each layer && draw each layer
            foreach (KeyValuePair<string, List<DrawData>> layer in drawData)
            {
                layer.Value.Sort(SortDrawables);

                DrawLayer(layer.Key, spriteBatch, spriteFont, cameraPosition, layer.Value);
            }

            // end the scene
            spriteBatch.End();
        }

        protected void AddToDrawables(List<DrawData> addTo, string layer, Entity e)
        {
            DrawData dd;
            foreach (GameDrawable d in e.Get<DrawableComponent>().Get(layer))
            {
                dd = new DrawData();
                dd.Entity = e;
                dd.Position = e.Get<PositionComponent>();
                dd.Drawable = d;
                addTo.Add(dd);
            }
        }

        protected int SortDrawables(DrawData a, DrawData b)
        {
            return a.Position.Y.CompareTo(b.Position.Y);
        }

        protected virtual void DrawLayer(string layer, SpriteBatch spriteBatch, SpriteFont spriteFont, PositionComponent camera, List<DrawData> layerData)
        {
            foreach (DrawData d in layerData)
                Draw(spriteBatch, spriteFont, d, camera);
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

        private void Entities_EntityRemoved(Entity e, World world)
        {
            drawables.Remove(e);   
        }

        private void Entities_EntityAdded(Entity e, World world)
        {
            if (e.HasComponent<DrawableComponent>() && e.HasComponent<PositionComponent>())
            {
                drawables.Add(e);
            }
        }
    }
}
