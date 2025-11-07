// librerias necesarias para trabajar con monogame
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Fishing_Fever
{
    // clase que maneja todo lo del pescador y sus animaciones
    public class Pescador
    {
        // texturas de las animaciones
        private Texture2D rojoTirarCaña;    // animacion de tirar la caña
        private Texture2D rojoEsfuerzoCaña; // animacion de esfuerzo

        // posicion del pescador en pantalla
        private Vector2 posicion;

        // variables de control de frames y tiempo
        private int frameActual = 0;          // frame que se esta mostrando
        private double tiempoFrame = 0;       // tiempo acumulado desde el ultimo frame
        private double duracionFrame = 0.15;  // cuanto dura cada frame

        // banderas de estado
        private bool animandoTiro = false;    // si esta tirando la caña
        private bool animandoRetiro = false;  // si esta recogiendo
        private bool enEsfuerzo = false;      // si esta en animacion de esfuerzo

        // constructor del pescador
        public Pescador(ContentManager content, Vector2 posicionInicial)
        {
            // carga las dos imagenes desde la carpeta Content/Images
            rojoTirarCaña = content.Load<Texture2D>("Images/rojoTirarCaña");
            rojoEsfuerzoCaña = content.Load<Texture2D>("Images/rojoEsfuerzoCaña");

            // guarda la posicion
            posicion = posicionInicial;
        }

        // actualiza el estado del pescador cada frame
        public void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            // si el jugador hace click y no esta animando
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

                // reinicia el tiempo del frame
                tiempoFrame = 0;
            }

            // acumula el tiempo desde el ultimo frame
            tiempoFrame += gameTime.ElapsedGameTime.TotalSeconds;

            // animacion de tirar la caña
            if (animandoTiro)
            {
                // si paso suficiente tiempo, avanza al siguiente frame
                if (tiempoFrame >= duracionFrame)
                {
                    frameActual++;
                    tiempoFrame = 0;

                    // si llego al final, cambia al estado de esfuerzo
                    if (frameActual >= 3)
                    {
                        frameActual = 0;
                        animandoTiro = false;
                        enEsfuerzo = true;
                    }
                }
            }
            // animacion de recoger (va en orden inverso)
            else if (animandoRetiro)
            {
                if (tiempoFrame >= duracionFrame)
                {
                    frameActual--;
                    tiempoFrame = 0;

                    // si llego al primer frame, vuelve al estado base
                    if (frameActual < 0)
                    {
                        frameActual = 0;
                        animandoRetiro = false;
                        enEsfuerzo = false;
                    }
                }
            }
            // animacion de esfuerzo (bucle)
            else if (enEsfuerzo)
            {
                // se mueve mas lento que las otras
                if (tiempoFrame >= duracionFrame * 4)
                {
                    frameActual++;
                    tiempoFrame = 0;

                    // cicla entre frame 1 y 2
                    if (frameActual > 2)
                        frameActual = 1;
                }
            }
        }

        // dibuja al pescador en pantalla
        public void Draw(SpriteBatch spriteBatch)
        {
            // elige el sprite segun el estado
            Texture2D spriteActual = enEsfuerzo ? rojoEsfuerzoCaña : rojoTirarCaña;

            // define el area de la imagen que se va a mostrar (cada frame mide 30x40)
            Rectangle sourceRect = new Rectangle(frameActual * 30, 0, 30, 40);

            // dibuja el sprite en su posicion, con escala 3x
            spriteBatch.Draw(spriteActual, posicion, sourceRect, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
        }
    }
}
