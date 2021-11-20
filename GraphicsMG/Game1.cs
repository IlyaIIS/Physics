using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyTools;
using Physics;

namespace GraphicsMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Camera Camera = new Camera();
        public Field Field;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Camera.Pos = new Vector2(0f,0f);
            Camera.Zoom = 3;
            Controller.Cam = Camera;

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Field = new Field(100, 100);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Printer.TileSp = Content.Load<Texture2D>("Tile");

            Printer.Font = Content.Load<SpriteFont>("defaultFont");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Controller.CheckKeyActions(Field);
            MouseState mouse = Mouse.GetState();

            if (gameTime.TotalGameTime.Ticks % 3 == 0)
            {
                Field.Update();

                /*
                Tile tile = Field.Tiles[Field.Width / 2, Field.Height / 2];
                Particle particle;
                if (tile.Particle == null)
                {
                    if (MyRandom.CheckChance(50))
                    {
                        particle = new Water(tile);
                        particle.SpeedX = -5;
                        particle.SpeedY = -5;
                    }
                    else
                    {
                        particle = new Sand(tile);
                        particle.SpeedX = 5;
                        particle.SpeedY = -5;
                    }
                    tile.Particle = particle;
                }
                //*/
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.get_transformation(GraphicsDevice));

            Printer.DrawLevel(_spriteBatch, Field);

            _spriteBatch.End();


            _spriteBatch.Begin();

            Printer.DrawTechInf(Field);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
