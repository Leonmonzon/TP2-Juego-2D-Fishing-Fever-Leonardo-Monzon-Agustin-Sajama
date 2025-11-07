// librerias necesarias para monogame
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fishing_Fever
{
    // clase principal del juego
    public class Game1 : Game
    {
        PescaBarra barraPesca;
        // objeto que maneja configuracion de la ventana y resolucion
        private GraphicsDeviceManager _graphics;

        // objeto que permite dibujar cosas en pantalla
        private SpriteBatch _spriteBatch;

        // textura para el fondo del juego
        private Texture2D fondo;

        // objeto del pescador, que maneja su animacion
        private Pescador pescador;

        // constructor, inicializa cosas basicas
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; // carpeta donde estan las imagenes y sonidos
            IsMouseVisible = true; // muestra el cursor del mouse
        }

        // se ejecuta una sola vez al iniciar el juego, para cargar imagenes
        protected override void LoadContent()
        {

            barraPesca = new PescaBarra(GraphicsDevice, new Vector2(700, 200));

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // carga el fondo desde la carpeta Content/Images
            fondo = Content.Load<Texture2D>("Images/fondo");

            // crea el pescador, pasandole la posicion inicial
            pescador = new Pescador(
                Content,
                new Vector2(
                    (_graphics.PreferredBackBufferWidth / 2) + 75 - 15, // centrado un poco a la derecha
                    (_graphics.PreferredBackBufferHeight / 2) - 27       // un poco arriba
                )
            );
        }

        // se ejecuta cada frame, maneja la logica del juego
        protected override void Update(GameTime gameTime)
        {


            var mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed && !barraPesca.EstaActiva())
                barraPesca.Activar();

            barraPesca.Actualizar(gameTime);
            // si se presiona ESC, cierra el juego
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // actualiza la logica y animacion del pescador
            pescador.Update(gameTime);

            base.Update(gameTime);
        }

        // se ejecuta cada frame despues del Update, dibuja todo
        protected override void Draw(GameTime gameTime)
        {
            // limpia la pantalla con color celeste
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // comienza el proceso de dibujo
            _spriteBatch.Begin();

            // dibuja el fondo en toda la pantalla
            // se usa "FlipHorizontally" para que quede al reves (espejado)
            _spriteBatch.Draw(
                fondo,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.FlipHorizontally,
                0f
            );
            barraPesca.Dibujar(_spriteBatch);
            // dibuja al pescador
            pescador.Draw(_spriteBatch);

            // termina el proceso de dibujo
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
