using System;
using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.Entities;
using IsoECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.GamePlay.Map;
using IsoECS.Util;

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

        Entity cameraEntity;

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
            EntityLibrary.Instance.LoadFromJson("Content/Data/buildables.json", true);

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

            cameraEntity = new Entity();
            cameraEntity.AddComponent(new PositionComponent());
            cameraEntity.AddComponent(new CameraController());
            cameraEntity.Get<CameraController>().Up.AddRange(new List<Keys>() { Keys.W, Keys.Up });
            cameraEntity.Get<CameraController>().Down.AddRange(new List<Keys>() { Keys.S, Keys.Down });
            cameraEntity.Get<CameraController>().Left.AddRange(new List<Keys>() { Keys.A, Keys.Left });
            cameraEntity.Get<CameraController>().Right.AddRange(new List<Keys>() { Keys.D, Keys.Right });
            entities.Add(cameraEntity);

            Entity inputEntity = new Entity();
            inputEntity.AddComponent(new InputController());
            entities.Add(inputEntity);

            IsometricMapComponent mapComponent = CreateMap("isometric_tiles", 64, 64, 32, 16);
            DrawableComponent mapDrawable = new DrawableComponent();
            mapDrawable.Sprites.Add(new DrawableSprite()
            {
                Visible = true,
                Color = Color.White,
                Static = true,
                SpriteSheet = "internal_map_texture",
                ID = "internal_map_source"
            });
            Entity mapEntity = new Entity();
            mapEntity.AddComponent(mapComponent);
            mapEntity.AddComponent(mapDrawable);
            mapEntity.AddComponent(new PositionComponent());
            entities.Add(mapEntity);

            Entity debugEntity = new Entity();
            debugEntity.AddComponent(new PositionComponent());
            debugEntity.AddComponent(new DrawableTextComponent());
            debugEntity.AddComponent(new DebugComponent());
            entities.Add(debugEntity);

            // add an entity that tracks data
            Entity dataTracker = new Entity();
            DrawableComponent dataDrawble = new DrawableComponent();
            dataDrawble.Sprites.Add(new DrawableSprite()
            {
                Visible = false
            });
            dataTracker.AddComponent(new RoadplannerComponent());
            dataTracker.AddComponent(CreateCollisionMap(mapComponent));
            dataTracker.AddComponent(new FoundationPlannerComponent());
            dataTracker.AddComponent(new PositionComponent());
            dataTracker.AddComponent(dataDrawble);
            entities.Add(dataTracker);

            // add test person entity
            Entity person = new Entity();
            person.AddComponent(new PositionComponent());
            DrawableComponent personDrawable = new DrawableComponent();
            personDrawable.Sprites.Add(new DrawableSprite()
            {
                Visible = true,
                Color = Color.White,
                SpriteSheet = "isometric_person",
                ID = "male"
            });
            person.AddComponent(personDrawable);
            person.AddComponent(new MoveToTargetComponent());
            entities.Add(person);

            diagnosticEntity = new Entity();
            diagnosticEntity.AddComponent(new PositionComponent());
            diagnosticEntity.AddComponent(new DrawableTextComponent()
            {
                Text = "",
                Color = Color.White,
                Visible = true
            });
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

            diagnosticEntity.Get<DrawableTextComponent>().Text = diagnostics.ShowTop(8, true);

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
