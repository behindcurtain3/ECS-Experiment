using System;
using System.Collections.Generic;
using System.IO;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.GamePlay;
using IsoECS.GamePlay.Map;
using IsoECS.Systems;
using IsoECS.Systems.GamePlay;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TomShane.Neoforce.Controls;
using IsoECS.Systems.UI;
using IsoECS.DataStructures.Json.Converters;
using System.Threading;
using IsoECS.Systems.Threaded;

namespace IsoECS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static Random Random { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        EntityManager em;
        List<ISystem> systems;
        List<IRenderSystem> renderers;
        Thread pathThread;
        Thread pathThread2;

        RenderSystem renderSystem;
        DiagnosticInfo diagnostics;
        Entity diagnosticEntity;
        Entity inputControlEntity;
        int _updateDiagnosticsRate = 500;
        int _updateDiagnosticsCountdown;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 680;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Random = new Random();
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

            diagnostics = new DiagnosticInfo("System Performance");

            // Add the content to the textures singleton
            Textures.Instance.Graphics = GraphicsDevice;
            Textures.Instance.Content = Content;

            systems = new List<ISystem>();
            systems.Add(new IsoECS.Systems.InputSystem()); // input system should update before any other system that needs to read the input
            systems.Add(new ControlSystem());
            systems.Add(new DateTimeSystem());
            systems.Add(new BehaviorSystem());
            systems.Add(new DebugSystem());
            systems.Add(new ProductionSystem());
            systems.Add(new ImmigrationSystem());
            systems.Add(new CityInformationSystem() { Graphics = GraphicsDevice });
            systems.Add(new HousingUpgradeSystem());

            renderers = new List<IRenderSystem>();
            renderers.Add(new IsometricMapSystem()
            {
                Graphics = GraphicsDevice
            });

            renderSystem = new RenderSystem()
            {
                Graphics = GraphicsDevice,
                ClearColor = Color.Black
            };
            renderers.Add(renderSystem);

            em = new EntityManager();
            em.UI = new Manager(this, "Pixel")
            {
                AutoCreateRenderTarget = false,
                AutoUnfocus = true,
                TargetFrames = 60
            };
            em.UI.Initialize();
            em.UI.RenderTarget = em.UI.CreateRenderTarget();

            Window w = new TomShane.Neoforce.Controls.Window(em.UI)
            {
                Text = "My Quick Test Window"
            };
            w.Init();
            w.Center();
            em.UI.Add(w);

            Table testTable = new Table(em.UI)
            {
                Top = 2,
                Left = 2,
                Width = w.ClientWidth - 4,
                Height = w.ClientHeight - 4,
                Anchor = Anchors.All
            };
            testTable.Init();
            testTable.SetTableSize(4, 5, new string[]{ "My Column" });
            testTable.AddAt(3, 3, "Hello Data!");
            testTable.AddAt(0, 0, "What?");
            w.Add(testTable);

            // Load the scenario
            // TODO: put this in a method somewhere
            string s = File.ReadAllText("Content/Data/Scenarios/alpha.json");
            Scenario scenario = JsonConvert.DeserializeObject<Scenario>(s);
            
            // Load textures from config
            Textures.Instance.LoadFromJson(scenario.Textures, true);

            // Load the drawables
            DrawableLibrary.Instance.LoadFromJson(scenario.Drawables, true);

            // Load in entities
            EntityLibrary.Instance.LoadFromJson(scenario.Entities, true);
                
            // load scenario data
            GameData.Instance.LoadItemsFromJson(scenario.Items, true);
            GameData.Instance.LoadRecipesFromJson(scenario.Recipes, true);

            foreach (JObject o in scenario.DefaultEntities)
            {
                Entity e = EntityLibrary.Instance.LoadEntity(o);

                em.AddEntity(e);
            }

            // start up the pathfinder thread
            PathfinderSystem pfs = new PathfinderSystem()
            {
                Map = em.Map,
                Collisions = em.Collisions
            };
            pathThread = new Thread(new ThreadStart(pfs.Run));
            pathThread.Start();

            pfs = new PathfinderSystem()
            {
                Map = em.Map,
                Collisions = em.Collisions
            };
            pathThread2 = new Thread(new ThreadStart(pfs.Run));
            //pathThread2.Start();

            // add some test entities to the map
            for (int j = 0; j < 3; j++)
            {
                Entity test = new Entity();

                test.AddComponent(new PositionComponent()
                {
                    X = Random.Next(GraphicsDevice.Viewport.Width),
                    Y = Random.Next(GraphicsDevice.Viewport.Height)
                });
                em.AddEntity(test);
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
            em.AddEntity(inputControlEntity);

            DrawableComponent diagDrawable = new DrawableComponent();
            diagDrawable.Add("Text", new DrawableText()
            {
                Text = "",
                Color = Color.White,
                Visible = true,
                Layer = "Text",
                Static = true
            });
            diagnosticEntity = new Entity();
            diagnosticEntity.AddComponent(new PositionComponent() { X = 0, Y = 50f });
            diagnosticEntity.AddComponent(diagDrawable);
            em.AddEntity(diagnosticEntity);

            // init the systems
            foreach (ISystem system in systems)
                system.Init(em);
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

        protected override void UnloadContent()
        {
            base.UnloadContent();

            pathThread.Abort();
            pathThread2.Abort();
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update the UI
            diagnostics.RestartTiming("UI");
            em.UI.Update(gameTime);
            diagnostics.StopTiming("UI");

            // update the game systems
            foreach (ISystem system in systems)
            {
                diagnostics.RestartTiming(system.GetType().Name);
                system.Update(em, gameTime.ElapsedGameTime.Milliseconds);
                diagnostics.StopTiming(system.GetType().Name);
            }

            _updateDiagnosticsCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            if (_updateDiagnosticsCountdown <= 0)
            {
                _updateDiagnosticsCountdown += _updateDiagnosticsRate;
                ((DrawableText)diagnosticEntity.Get<DrawableComponent>().Drawables["Text"][0]).Text = diagnostics.ShowTop(8, true);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            em.UI.BeginDraw(gameTime);

            foreach (IRenderSystem render in renderers)
            {
                diagnostics.RestartTiming(render.GetType().Name);
                render.Draw(em, spriteBatch, spriteFont);
                diagnostics.StopTiming(render.GetType().Name);
            }

            em.UI.EndDraw();

            base.Draw(gameTime);
        }
        
        private CollisionMapComponent CreateCollisionMap(IsometricMapComponent map)
        {
            CollisionMapComponent collisions = new CollisionMapComponent();

            for (int x = 0; x < map.TxWidth; x++)
            {
                for (int y = 0; y < map.TxHeight; y++)
                {
                    if (map.Terrain[0, y, x] == (int)Tiles.Grass)
                        collisions.Map.Add(new Point(x, y), PathTypes.UNDEFINED);
                    else
                        collisions.Map.Add(new Point(x, y), PathTypes.BLOCKED);
                }
            }

            return collisions;
        }
    }
}
