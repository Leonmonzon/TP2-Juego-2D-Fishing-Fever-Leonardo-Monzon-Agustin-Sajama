using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Fishing_Fever
{
    public class Intro : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Texture2D portada;
        Texture2D pixel; // textura blanca para dibujar botones
        SpriteFont fuente;

        Song musicaIntro;
        bool musicaIniciada;

        // botones
        Rectangle btnJugar;
        Rectangle btnConfig;
        Rectangle btnSalir;

        MouseState mouseAnterior;

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
            musicaIntro = Content.Load<Song>("Audio/musicaIntro");
            fuente = Content.Load<SpriteFont>("Fonts/fuente"); // asegurate de tener una fuente

            // textura blanca 1x1
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            int centroX = _graphics.PreferredBackBufferWidth / 2 - 100;
            int inicioY = 300;

            btnJugar  = new Rectangle(centroX, inicioY, 200, 50);
            btnConfig = new Rectangle(centroX, inicioY + 70, 200, 50);
            btnSalir  = new Rectangle(centroX, inicioY + 140, 200, 50);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!musicaIniciada)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(musicaIntro);
                musicaIniciada = true;
            }

            var mouse = Mouse.GetState();

            // click real (no mantener apretado)
            bool click = mouse.LeftButton == ButtonState.Pressed &&
                         mouseAnterior.LeftButton == ButtonState.Released;

            if (click)
            {
                if (btnJugar.Contains(mouse.Position))
                {
                    MediaPlayer.Stop();
                    Game1 juego = new Game1();
                    juego.Run();
                    Exit();
                }

                if (btnConfig.Contains(mouse.Position))
                {
                    // por ahora no hace nada
                    // despues aca metemos otra escena
                }

                if (btnSalir.Contains(mouse.Position))
                {
                    Exit();
                }
            }

            mouseAnterior = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            // portada
            _spriteBatch.Draw(
                portada,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.White
            );

            // botones
            dibujarBoton(btnJugar, "jugar");
            dibujarBoton(btnConfig, "configuracion");
            dibujarBoton(btnSalir, "salir");

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void dibujarBoton(Rectangle rect, string texto)
        {
            _spriteBatch.Draw(pixel, rect, Color.Black * 0.6f);

            Vector2 tamañoTexto = fuente.MeasureString(texto);
            Vector2 posTexto = new Vector2(
                rect.X + rect.Width / 2 - tamañoTexto.X / 2,
                rect.Y + rect.Height / 2 - tamañoTexto.Y / 2
            );

            _spriteBatch.DrawString(fuente, texto, posTexto, Color.White);
        }
    }
}
