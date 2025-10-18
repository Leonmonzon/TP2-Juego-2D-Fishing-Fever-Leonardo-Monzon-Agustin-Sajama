using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fishing_Fever;

public class Game1 : Game
{
    Texture2D fondo;

    // sprites de animacion
    Texture2D rojoTirarCaña;
    Texture2D rojoEsfuerzoCaña;

    Vector2 posPescador;

    // control de animacion
    int frameActual = 0;
    double tiempoFrame = 0;
    double duracionFrame = 0.15;

    // estados de animacion
    bool animandoTiro = false;
    bool animandoRetiro = false;
    bool enEsfuerzo = false;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // cargo las imagenes del juego
        fondo = Content.Load<Texture2D>("Images/fondo");
        rojoTirarCaña = Content.Load<Texture2D>("Images/rojoTirarCaña");
        rojoEsfuerzoCaña = Content.Load<Texture2D>("Images/rojoEsfuerzoCaña");

        // posicion del pescador, centrado y un poco a la derecha
        posPescador = new Vector2(
            (_graphics.PreferredBackBufferWidth / 2) + 75 - 15,
            (_graphics.PreferredBackBufferHeight / 2) - 27
        );
    }

    protected override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        // cerrar el juego con escape o boton de mando
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // si se hace click y no esta animando
        if (mouse.LeftButton == ButtonState.Pressed && !animandoTiro && !animandoRetiro)
        {
            // si no esta en esfuerzo, empieza la animacion de tirar
            if (!enEsfuerzo)
            {
                animandoTiro = true;
                frameActual = 0;
            }
            // si ya esta en esfuerzo, empieza la animacion de recoger
            else
            {
                animandoRetiro = true;
                frameActual = 2;
            }

            tiempoFrame = 0;
        }

        // acumula el tiempo del frame
        tiempoFrame += gameTime.ElapsedGameTime.TotalSeconds;

        // animacion de tirar la caña
        if (animandoTiro)
        {
            if (tiempoFrame >= duracionFrame)
            {
                frameActual++;
                tiempoFrame = 0;

                // cuando termina los frames pasa al estado de esfuerzo
                if (frameActual >= 3)
                {
                    frameActual = 0;
                    animandoTiro = false;
                    enEsfuerzo = true;
                }
            }
        }
        // animacion de recoger (va al reves)
        else if (animandoRetiro)
        {
            if (tiempoFrame >= duracionFrame)
            {
                frameActual--;
                tiempoFrame = 0;

                // si vuelve al frame base, termina y vuelve al inicio
                if (frameActual < 0)
                {
                    frameActual = 0;
                    animandoRetiro = false;
                    enEsfuerzo = false;
                }
            }
        }
        // animacion en bucle del esfuerzo
        else if (enEsfuerzo)
        {
            // se mueve mas lento entre los frames
            if (tiempoFrame >= duracionFrame * 4)
            {
                frameActual++;
                tiempoFrame = 0;

                // bucle entre frame 1 y 2
                if (frameActual > 2)
                    frameActual = 1;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        // dibujo el fondo
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

        // elijo que sprite usar segun el estado
        Texture2D spriteActual = enEsfuerzo ? rojoEsfuerzoCaña : rojoTirarCaña;

        // recorto el frame actual del sprite (cada frame mide 30x40)
        Rectangle sourceRect = new Rectangle(frameActual * 30, 0, 30, 40);

        // dibujo el pescador en su posicion con escala 3x
        _spriteBatch.Draw(spriteActual, posPescador, sourceRect, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
