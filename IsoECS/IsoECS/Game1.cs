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
        Random random;

        RenderSystem renderSystem;

        IsoMap map;
        Entity mapEntity;
        Entity cameraEntity;


        Dictionary<String, Texture2D> textures;

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

            // Add the content to the textures singleton
            Textures.Instance.Graphics = GraphicsDevice;
            Textures.Instance.Content = Content;

            // Load textures from config
            Textures.Instance.LoadFromJson("Content/Data/textures.json", true);

            systems = new List<ISystem>();
            systems.Add(new InputSystem()); // input system should update before any other system that needs to read the input
            systems.Add(new CameraSystem());
            systems.Add(new ProductionSystem());

            entities = new List<Entity>();

            for (int i = 0; i < 10; i++)
            {
                Entity e = new Entity();

                e.AddComponent(new PositionComponent(new Vector2((float)random.NextDouble() * 300, (float)random.NextDouble() * 300)));
                e.AddComponent(new DrawableTextComponent() 
                    { 
                        Text = "I'm an entity with a drawable # " + e.ID,
                        Color = new Color(random.Next(256), random.Next(256), random.Next(256))
                    });

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

                node.AddComponent(new DrawableComponent()
                {
                    Texture = textures["Nodes"],
                    Source = new Rectangle(0, 0, textures["Nodes"].Width, textures["Nodes"].Height),
                    Color = Color.White,
                    Layer = 1,
                    Visible = true
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

            renderSystem = new RenderSystem()
            {
                Graphics = GraphicsDevice,
                ClearColor = Color.White
            };

            mapEntity = new Entity();
            mapEntity.AddComponent(new DrawableComponent() 
            {
                Visible = true,
                Color = Color.White
            });
            mapEntity.AddComponent(new PositionComponent());
            entities.Add(mapEntity);

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

            map = new IsoMap(GraphicsDevice);
            map.CreateMap("isometric_tiles", 32, 32, 16, 9);
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

            textures = new Dictionary<string, Texture2D>();
            textures.Add("Nodes", Content.Load<Texture2D>("Data/Sprites/Nodes"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // update the game systems
            foreach (ISystem system in systems)
            {
                system.Update(entities, gameTime.ElapsedGameTime.Milliseconds);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // render the map to texture
            map.RenderToTexture(spriteBatch, cameraEntity.Get<PositionComponent>().X, cameraEntity.Get<PositionComponent>().Y, MapRenderComplete);

            // render everything
            renderSystem.Draw(entities, spriteBatch, spriteFont);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Callback for when the map has completed rendering, update the map entity texture
        /// </summary>
        /// <param name="map"></param>
        private void MapRenderComplete(IsoMap map)
        {
            if (mapEntity.HasComponent<DrawableComponent>())
            {
                DrawableComponent drawable = mapEntity.Get<DrawableComponent>();

                drawable.Layer = 99;
                drawable.Texture = (Texture2D)map.Buffer;
                drawable.Source = new Rectangle(0, 0, map.Buffer.Width, map.Buffer.Height);
            }
        }
    }
}
