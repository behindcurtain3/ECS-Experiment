using System;
using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.GamePlay.Map;
using IsoECS.Systems;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using IsoECS.GamePlay;
using System.IO;
using Newtonsoft.Json.Linq;

namespace IsoECS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        List<Entity> entities;
        List<ISystem> systems;
        List<IRenderSystem> renderers;
        Random random;

        RenderSystem renderSystem;
        DiagnosticInfo diagnostics;
        Entity diagnosticEntity;
        Entity inputControlEntity;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            random = new Random();

            diagnostics = new DiagnosticInfo("System Performance");

            // Add the content to the textures singleton
            Textures.Instance.Graphics = GraphicsDevice;
            Textures.Instance.Content = Content;

            // Load textures from config
            Textures.Instance.LoadFromJson("Content/Data/textures.json", true);

            // Load in entities
            EntityLibrary.Instance.LoadFromJson("Content/Data/entities.json", true);

            systems = new List<ISystem>();
            systems.Add(new InputSystem()); // input system should update before any other system that needs to read the input
            systems.Add(new ControlSystem());
            systems.Add(new ProductionSystem());

            renderers = new List<IRenderSystem>();
            renderers.Add(new IsometricMapSystem());

            renderSystem = new RenderSystem()
            {
                Graphics = GraphicsDevice,
                ClearColor = Color.Black
            };
            renderers.Add(renderSystem);

            entities = new List<Entity>();

            // Load the scenario
            // TODO: put this in a method somewhere
            string s = File.ReadAllText("Content/Data/Scenarios/alpha.json");
            Scenario scenario = JsonConvert.DeserializeObject<Scenario>(s);
            foreach (JObject o in scenario.DefaultEntities)
            {
                Entity e = EntityLibrary.Instance.LoadEntity(o);

                if (e.HasComponent<IsometricMapComponent>())
                {
                    IsometricMapComponent map = e.Get<IsometricMapComponent>();

                    if (map.Terrain == null)
                    {
                        map = CreateMap(map.SpriteSheetName, map.TxWidth, map.TxHeight, map.PxTileWidth, map.PxTileHeight);

                        // replace the map
                        e.RemoveComponent(e.Get<IsometricMapComponent>());
                        e.AddComponent(map);
                    }
                }

                entities.Add(e);
            }

            // add some nodes to the map
            for (int j = 0; j < 3; j++)
            {
                Entity node = new Entity();

                node.AddComponent(new PositionComponent()
                {
                    X = random.Next(GraphicsDevice.Viewport.Width),
                    Y = random.Next(GraphicsDevice.Viewport.Height)
                });

                node.AddComponent(new Inventory());
                node.AddComponent(new Generator()
                {
                    Recipe = new GamePlay.Recipe()
                    {
                       Output = new GamePlay.Item()
                       {
                           Name = "Default",
                           Amount =  1
                       }
                    },

                    Rate = 3000,
                    RateCountdown = 3000
                });

                entities.Add(node);
            }

            // TODO: create a settings file to read any key bindings from
            inputControlEntity = new Entity();
            inputControlEntity.AddComponent(new InputController());
            inputControlEntity.AddComponent(new PositionComponent());
            inputControlEntity.AddComponent(new CameraController());
            inputControlEntity.Get<CameraController>().Up.AddRange(new List<Keys>() { Keys.W, Keys.Up });
            inputControlEntity.Get<CameraController>().Down.AddRange(new List<Keys>() { Keys.S, Keys.Down });
            inputControlEntity.Get<CameraController>().Left.AddRange(new List<Keys>() { Keys.A, Keys.Left });
            inputControlEntity.Get<CameraController>().Right.AddRange(new List<Keys>() { Keys.D, Keys.Right });
            entities.Add(inputControlEntity);

            DrawableComponent diagDrawable = new DrawableComponent();
            diagDrawable.Drawables.Add(new DrawableText()
            {
                Text = "",
                Color = Color.White,
                Visible = true,
                Layer = 0,
                Static = true
            });
            diagnosticEntity = new Entity();
            diagnosticEntity.AddComponent(new PositionComponent());
            diagnosticEntity.AddComponent(diagDrawable);
            entities.Add(diagnosticEntity);

            // init the systems
            foreach (ISystem system in systems)
                system.Init(entities);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Default");
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update the game systems
            foreach (ISystem system in systems)
            {
                diagnostics.RestartTiming(system.GetType().ToString());
                system.Update(entities, gameTime.ElapsedGameTime.Milliseconds);
                diagnostics.StopTiming(system.GetType().ToString());
            }

            ((DrawableText)diagnosticEntity.Get<DrawableComponent>().Drawables[0]).Text = diagnostics.ShowTop(8, true);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            foreach (IRenderSystem render in renderers)
            {
                diagnostics.RestartTiming(render.GetType().ToString());
                render.Draw(entities, spriteBatch, spriteFont);
                diagnostics.StopTiming(render.GetType().ToString());
            }

            base.Draw(gameTime);
        }

        private IsometricMapComponent CreateMap(string spriteSheetName, int txWidth, int txHeight, int pxTileWidth, int pxTileHeight)
        {
            IsometricMapComponent map = new IsometricMapComponent();

            map.Graphics = GraphicsDevice;
            map.Buffer = new RenderTarget2D(map.Graphics, map.Graphics.Viewport.Width, map.Graphics.Viewport.Height);

            map.SpriteSheetName = spriteSheetName;

            map.TxWidth = txWidth;
            map.TxHeight = txHeight;

            map.PxTileWidth = pxTileWidth;
            map.PxTileHeight = pxTileHeight;

            map.PxTileHalfWidth = map.PxTileWidth / 2;
            map.PxTileHalfHeight = map.PxTileHeight / 2;

            // Create the tile data structure
            map.Terrain = new int[1, txHeight, txWidth];

            // fill in the array
            for (int y = 0; y < map.TxHeight; y++)
            {
                for (int x = 0; x < map.TxWidth; x++)
                {
                    map.Terrain[0, y, x] = (int)Tiles.Grass;
                }
            }

            return map;
        }

        private CollisionMapComponent CreateCollisionMap(IsometricMapComponent map)
        {
            CollisionMapComponent collisions = new CollisionMapComponent();

            for (int x = 0; x < map.TxWidth; x++)
            {
                for (int y = 0; y < map.TxHeight; y++)
                {
                    if (map.Terrain[0, y, x] == (int)Tiles.Grass)
                        collisions.Collision.Add(new Point(x, y), 64);
                    else
                        collisions.Collision.Add(new Point(x, y), -1);
                }
            }

            return collisions;
        }
    }
}
