using System.IO;
using System.Threading;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.DataStructures.Json.Converters;
using IsoECS.GamePlay;
using IsoECS.GamePlay.Map;
using IsoECS.RenderSystems;
using IsoECS.Systems;
using IsoECS.Systems.GamePlay;
using IsoECS.Systems.Threaded;
using IsoECS.Systems.UI;
using IsoECS.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TecsDotNet;
using TomShane.Neoforce.Controls;

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
        Thread pathThread;
        Thread pathThread2;

        DiagnosticInfo diagnostics;
        GameWorld world;

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

            world = new GameWorld();
            world.Systems.Add(world.Input);
            world.Systems.Add(new CameraSystem());
            world.Systems.Add(new InspectionSystem());
            world.Systems.Add(new ControlSystem());
            world.Systems.Add(new DateTimeSystem());
            world.Systems.Add(new BehaviorSystem());
            world.Systems.Add(new ImmigrationSystem());
            world.Systems.Add(new ProductionSystem());
            world.Systems.Add(new CityInformationSystem());
            world.Systems.Add(new HousingUpgradeSystem());
            world.Systems.Add(new OverlaySystem() { Graphics = GraphicsDevice });
            //world.Systems.Add(new DebugSystem());
            world.Systems.Add(new IsometricMapSystem()
            {
                Graphics = GraphicsDevice
            });

            world.Renderer = new DefaultRenderSystem()
            {
                Graphics = GraphicsDevice,
                ClearColor = Color.Black
            };
            world.Systems.Add(world.Renderer);
            
            world.UI = new Manager(this, "Pixel")
            {
                AutoCreateRenderTarget = false,
                AutoUnfocus = true,
                TargetFrames = 60
            };
            world.UI.Initialize();
            world.UI.RenderTarget = world.UI.CreateRenderTarget();

            Window w = new TomShane.Neoforce.Controls.Window(world.UI)
            {
                Text = "My Quick Test Window"
            };
            w.Init();
            w.Center();

            // Load the scenario
            // TODO: put this in a method somewhere
            string s = File.ReadAllText("Content/Data/Scenarios/alpha.json");
            Scenario scenario = JsonConvert.DeserializeObject<Scenario>(s);
            
            // Load textures from config
            Textures.Instance.LoadFromJson(scenario.Textures, true);

            // Load the drawables
            world.Prototypes.LoadFromFile(
                new DrawablesLoader(),
                scenario.Drawables,
                true);


            // Load in entities
            world.Prototypes.LoadFromFile(
                new CustomEntityLoader() { Converter = new CustomComponentConverter() }, 
                scenario.Entities, 
                true);
                
            // load scenario data
            world.Prototypes.LoadFromFile(
                new DataLoader<Item>(),
                scenario.Items,
                true);

            world.Prototypes.LoadFromFile(
                new DataLoader<Recipe>(),
                scenario.Recipes,
                true);
            //GameData.Instance.LoadItemsFromJson(scenario.Items, true);
            //GameData.Instance.LoadRecipesFromJson(scenario.Recipes, true);

            CustomEntityLoader cel = new CustomEntityLoader()
            {
                Library = world.Prototypes,
                Converter = new CustomComponentConverter()
            };
            foreach (JObject o in scenario.DefaultEntities)
            {
                Entity e = (Entity)cel.LoadPrototype(o);

                world.Entities.Add(e);
            }

            // start up the pathfinder thread
            PathfinderSystem pfs = new PathfinderSystem()
            {
                Map = world.Map,
                Collisions = world.Collisions
            };
            pathThread = new Thread(new ThreadStart(pfs.Run));
            pathThread.Start();

            pfs = new PathfinderSystem()
            {
                Map = world.Map,
                Collisions = world.Collisions
            };
            pathThread2 = new Thread(new ThreadStart(pfs.Run));
            //pathThread2.Start();

            // TODO: create a settings file to read any key bindings from
            inputControlEntity = new Entity();
            inputControlEntity.AddComponent(new PositionComponent());
            inputControlEntity.AddComponent(new CameraController());
            world.Entities.Add(inputControlEntity);

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
            world.Entities.Add(diagnosticEntity);
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
            // update the game systems
            double dt = (double)gameTime.ElapsedGameTime.Milliseconds * 0.001;
            world.Update(dt);

            // update the UI
            diagnostics.RestartTiming("UI");
            world.UI.Update(gameTime);
            diagnostics.StopTiming("UI");

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
            world.UI.BeginDraw(gameTime);

            foreach(TecsDotNet.System s in world.Systems)
            {
                if (s is RenderSystem)
                {
                    diagnostics.RestartTiming(s.GetType().Name);
                    ((RenderSystem)s).Draw(spriteBatch, spriteFont);
                    diagnostics.StopTiming(s.GetType().Name);
                }
            }

            world.UI.EndDraw();

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
