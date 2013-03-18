using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ParaProtector
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager Input;
        public static Game1 Instance;
        SpriteFont font;
        public int myPoints;
        bool gameOver, paused;
        public int Lives;
        public int ScoreToSpawn;
        public int AmountToSpawn;
        public List<FallDude> spawnedDudes;
        public List<int> Randoms;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Instance = this;

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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Input = new InputManager();
            this.Services.AddService(Input.GetType(), Input);

            Input.Bind("Pause", Keys.Space);

            font = Content.Load<SpriteFont>("SpriteFont1");

            Randoms = new List<int>();
            FillRandomList();

            spawnedDudes = new List<FallDude>();

            paused = false;
            gameOver = false;
            myPoints = 10;
            Lives = 50;

            ScoreToSpawn = 10;
            AmountToSpawn = 1;



            // TODO: use this.Content to load your game content here
        }

        public void FillRandomList()
        {
            for(int i = 0; i < 1000; i++)
            {
                Randoms.Add(new Random().Next(1, 700));
            }
        }


        public void SpawnMore()
        {
            for (int i = 0; i <= AmountToSpawn; i++)
            {
                FallDude newDude = new FallDude();

                spawnedDudes.Add(newDude);
            }
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

            Input.UpdateInput();

            float dt;

            if(!paused)
                dt = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            else
                dt = 0;

            if (myPoints >= ScoreToSpawn)
            {
                SpawnMore();
                ScoreToSpawn *= 2;
                AmountToSpawn *= 2;
            }

            CollisionDelegateReference clickCDR = null;
            Point clickPoint;

            if (Input.CheckMouseLeftPressed(InputManager.MasterAuthKey))
            {
                clickPoint = new Point(Input.GetMousePosition());
                clickCDR = CollisionWorld.Instance.AddDelegate(CollisionWorld.CollisionGroupIdentifier.MouseClick, new CollisionDelegate(null, clickPoint));
            }
            try
            {
                foreach (FallDude fd in spawnedDudes)
                {
                    if (fd.Destroy)
                    {
                        spawnedDudes.Remove(fd);
                        spawnedDudes.Add(new FallDude());
                    }
                    else
                        fd.Update(dt);
                }
            }
            catch { }
            clickPoint = null;

            if(clickCDR != null)
            CollisionWorld.Instance.RemoveDelegate(clickCDR);

            


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (FallDude fd in spawnedDudes)
            {
                fd.Draw(spriteBatch);
            }
            spriteBatch.DrawString(font, "Lives: " + Lives, new Vector2(400, 20), Color.White);
            spriteBatch.DrawString(font, "Points: " + myPoints, new Vector2(700, 20), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
