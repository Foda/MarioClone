using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Runtime.InteropServices;

namespace SFML_Test
{
    public class MainWindow : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private long _timerFreq;
        private long _startTime, _endTime;
        private int _fps;
        private float _deltatime;

        Texture2D blobTexture;
        Texture2D notmarioTexture;

        Level level = new Level();
        Player ply = new Player();
        Camera camera;

        Sprite background;
        Sprite backTrees;
        Sprite backMts;
        Sprite backClouds;
        List<System.Drawing.Color> colorizer;

        Effect crtEffect;

        public MainWindow()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 640;

            this.IsFixedTimeStep = true;
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera();
            camera.Pos = new Vector2(320, 240);
            camera.Zoom = 0.5f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            crtEffect = Content.Load<Effect>(System.IO.Path.GetFullPath("Bend.fx"));

            ConstHelper.content = this.Content;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blobTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\blocks.png"));
            notmarioTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\notmario.png"));
            ply.mSpriteTexture = notmarioTexture;
            ply.Source = new Rectangle(0, 0, 16, 16);
            ply.LoadContent(Content);
            ply.vPosition = new Vector2(300, 300);

            level.blockSprite = new Sprite();
            level.blockSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\block_set.png"));
            level.blockSprite.Source = new Rectangle(0, 0, 16, 16);

            level.blockFrostingSprite = new Sprite();
            level.blockFrostingSprite.mSpriteTexture = 
                Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\block_set_frosting.png"));
            level.blockFrostingSprite.Source = new Rectangle(16, 0, 16, 16);

            level.stoneSprite = new Sprite();
            level.stoneSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\block_stone.png"));
            level.stoneSprite.Source = new Rectangle(0, 0, 16, 16);

            level.slopeSprite = new Sprite();
            level.slopeSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\block_slope.png"));
            level.slopeSprite.Source = new Rectangle(0, 0, 16, 16);

            level.litSprite = new Sprite();
            level.litSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\blob.png"));
            level.litSprite.Source = new Rectangle(0, 0, 16, 16);

            level.itemBlockSprite = new Sprite();
            level.itemBlockSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\itemblock.png"));
            level.itemBlockSprite.Source = new Rectangle(0, 0, 16, 16);

            level.mushSprite = new Sprite();
            level.mushSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\mushroom.png"));
            level.mushSprite.Source = new Rectangle(0, 0, 16, 16);

            level.bubbleSprite = new Sprite();
            level.bubbleSprite.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\respawn_bubble.png"));
            level.bubbleSprite.Source = new Rectangle(0, 0, 32, 32);

            backTrees = new Sprite();
            backTrees.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\back_trees.png"));
            backTrees.Source = new Rectangle(0, 0, 512, 170);

            backClouds = new Sprite();
            backClouds.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\back_clouds.png"));
            backClouds.Source = new Rectangle(0, 0, 512, 300);

            backMts = new Sprite();
            backMts.mSpriteTexture = Content.Load<Texture2D>(System.IO.Path.GetFullPath("Assets\\back_mts.png"));
            backMts.Source = new Rectangle(0, 0, 511, 341);
            backMts.Position.Y = 110;

            level.AddTestItem();

            //Colorizer!
            //247, 223, 175
            colorizer = Colorizer.GenerateColors_Offset(2, System.Drawing.Color.FromArgb(255, 70, 145, 113), 0.25f);

            //colorizer = Colorizer.GenerateColors_Harmony(3, 0, 360, 360, 360, 360, 0.5f, 0.5f);
            System.Drawing.Color blockcolor =
                Colorizer.GenerateColors_RandomAdd(1, System.Drawing.Color.FromArgb(255, 200, 152, 88),
                System.Drawing.Color.FromArgb(255, 112, 80, 48), System.Drawing.Color.FromArgb(255, 96, 168, 0), 1)[0];

            System.Drawing.Color blockfrost =
                Colorizer.GenerateColors_RandomAdd(1, System.Drawing.Color.FromArgb(255, 0, 200, 0),
                System.Drawing.Color.FromArgb(255, 177, 159, 88), System.Drawing.Color.FromArgb(255, 232, 192, 160), 1)[0];

            backTrees.color = new Color(colorizer[0].R, colorizer[0].G, colorizer[0].B);
            backMts.color = new Color(colorizer[1].R, colorizer[1].G, colorizer[1].B);

            level.blockSprite.color = new Color(blockcolor.R, blockcolor.G, blockcolor.B);
            level.blockFrostingSprite.color = new Color(blockfrost.R, blockfrost.G, blockfrost.B);

            //Test NPCs
            level.npcMan.CreateNPC(ConstHelper.NPCType.GOOMBA, new Vector2(16 * 33, 200), Vector2.Zero);
            level.npcMan.CreateNPC(ConstHelper.NPCType.TURTLE, new Vector2(16 * 30, 200), Vector2.Zero);

            AudioManager.LoadContent(Content);
        }

        static Random rand = new Random();
        private static float RandVec()
        {
            return (float)(rand.NextDouble() * 2 - 1);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                level.bubble.Pop();
            }
          
            level.TestCeiling(ref ply);
            level.WallTest(ref ply);
            level.TestGround(ref ply);

            level.Update(gameTime);

            ply.Input(Keyboard.GetState());
            ply.Update(gameTime, camera);

            camera.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(247, 223, 175));

            spriteBatch.Begin();
            backClouds.Position = new Vector2(0, 100);
            backClouds.Draw(spriteBatch);

            backClouds.Position = new Vector2(512, 100);
            backClouds.Draw(spriteBatch);

            backMts.Position = new Vector2(-camera.Pos.X / 20, 120 - camera.Pos.Y / 4);
            backMts.Draw(spriteBatch);

            backMts.Position = new Vector2(511 - camera.Pos.X / 20, 120 - camera.Pos.Y / 4);
            backMts.Draw(spriteBatch);
           
            backTrees.Position = new Vector2((float)128 - camera.Pos.X / 2, (float)400 - camera.Pos.Y / 2);
            backTrees.Draw(spriteBatch);

            backTrees.Position = new Vector2((float)640 - camera.Pos.X / 2, (float)400 - camera.Pos.Y / 2);
            backTrees.Draw(spriteBatch);

            backTrees.Position = new Vector2((float)1152 - camera.Pos.X / 2, (float)400 - camera.Pos.Y / 2);
            backTrees.Draw(spriteBatch);

            spriteBatch.End();

            camera.Zoom = 1f;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));
            level.Draw(spriteBatch);
            ply.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
