using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fishing_Fever
{
    public class Intro : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D portada;
        private bool cambiarAScene = false;

        public Intro()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            portada = Content.Load<Texture2D>("Images/portada");
        }

        protected override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed)
                cambiarAScene = true;

            if (mouse.LeftButton == ButtonState.Pressed)
{
    Exit();
}


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(
                portada,
                new Rectangle(
                    0, 0, 
                    _graphics.PreferredBackBufferWidth, 
                    _graphics.PreferredBackBufferHeight
                ),
                Color.White
            );
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}