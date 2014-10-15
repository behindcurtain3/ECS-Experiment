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

        RenderSystem renderSystem;
        DiagnosticInfo diagnostics;
        Entity diagnosticEntity;
        Entity inputControlEntity;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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
            systems.Add(new InputSystem()); // input system should update before any other system that needs to read the input
            systems.Add(new ControlSystem());
            systems.Add(new DateTimeSystem());
            systems.Add(new ProductionSystem());
            systems.Add(new ImmigrationSystem());
            systems.Add(new FindHomesSystem());

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

            // Load the scenario
            // TODO: put this in a method somewhere
            string s = File.ReadAllText("Content/Data/Scenarios/alpha.json");
            Scenario scenario = JsonConvert.DeserializeObject<Scenario>(s);
            
            // Load textures from config
            Textures.Instance.LoadFromJson(scenario.Textures, true);

            // Load in entities
            EntityLibrary.Instance.LoadFromJson(scenario.Library, true);

            // load scenario data
            GameData.Instance.LoadRecipesFromJson(scenario.Recipes, true);

            foreach (JObject o in scenario.DefaultEntities)
            {
                Entity e = EntityLibrary.Instance.LoadEntity(o);

                EntityHelper.ActivateEntity(em.Entities, e);
            }

            // add some test entities to the map
            for (int j = 0; j < 3; j++)
            {
                Entity test = new Entity();

                test.AddComponent(new PositionComponent()
                {
                    X = Random.Next(GraphicsDevice.Viewport.Width),
                    Y = Random.Next(GraphicsDevice.Viewport.Height)
                });

                EntityHelper.ActivateEntity(em.Entities, test);
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
                system.Update(em, gameTime.ElapsedGameTime.Milliseconds);
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
                render.Draw(em, spriteBatch, spriteFont);
                diagnostics.StopTiming(render.GetType().ToString());
            }

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
                        collisions.Collision.Add(new Point(x, y), 64);
                    else
                        collisions.Collision.Add(new Point(x, y), -1);
                }
            }

            return collisions;
        }
    }
}
