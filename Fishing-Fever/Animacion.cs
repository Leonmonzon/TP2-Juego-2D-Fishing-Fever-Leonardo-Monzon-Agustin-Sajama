using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fishing_Fever
{
    // clase generica para manejar animaciones por frames
    public class Animacion
    {
        private Texture2D textura;       // sprite con todos los frames
        private int anchoFrame;          // ancho de cada frame
        private int altoFrame;           // alto de cada frame
        private int frameActual;         // frame que se esta mostrando
        private int totalFrames;         // cantidad total de frames
        private double tiempoFrame;      // tiempo acumulado desde el ultimo frame
        private double duracionFrame;    // cuanto dura cada frame
        private bool enBucle;            // si la animacion se repite o no

        // indica si la animacion llego al final
        public bool Terminada { get; private set; }

        // propiedad de solo lectura para saber que frame esta activo
        public int FrameActual => frameActual;

        // constructor: inicializa una animacion con sus datos basicos
        public Animacion(Texture2D textura, int ancho, int alto, double duracion, bool bucle = false)
        {
            this.textura = textura;
            anchoFrame = ancho;
            altoFrame = alto;
            duracionFrame = duracion;
            enBucle = bucle;

            // calcula cuantos frames tiene la animacion
            totalFrames = textura.Width / ancho;

            Reiniciar();
        }

        // reinicia la animacion al primer frame
        public void Reiniciar()
        {
            frameActual = 0;
            tiempoFrame = 0;
            Terminada = false;
        }

        // actualiza el frame segun el tiempo transcurrido
        public void Actualizar(GameTime gameTime)
        {
            if (Terminada) return;

            tiempoFrame += gameTime.ElapsedGameTime.TotalSeconds;
            if (tiempoFrame >= duracionFrame)
            {
                tiempoFrame = 0;
                frameActual++;

                if (frameActual >= totalFrames)
                {
                    if (enBucle)
                        frameActual = 0;
                    else
                    {
                        frameActual = totalFrames - 1;
                        Terminada = true;
                    }
                }
            }
        }

        // dibuja la animacion en la pantalla con una escala simple
        public void Dibujar(SpriteBatch spriteBatch, Vector2 posicion, float escala)
        {
            Rectangle src = new Rectangle(frameActual * anchoFrame, 0, anchoFrame, altoFrame);
            spriteBatch.Draw(textura, posicion, src, Color.White, 0f, Vector2.Zero, escala, SpriteEffects.None, 0f);
        }

        // version extendida del dibujo (permite voltear o rotar)
        public void Dibujar(SpriteBatch spriteBatch, Vector2 posicion, float escala, SpriteEffects efecto, float rotacion = 0f)
        {
            Rectangle src = new Rectangle(frameActual * anchoFrame, 0, anchoFrame, altoFrame);
            spriteBatch.Draw(textura, posicion, src, Color.White, rotacion, Vector2.Zero, escala, efecto, 0f);
        }

        // permite cambiar manualmente el frame actual
        public void SetFrame(int frame)
        {
            frameActual = MathHelper.Clamp(frame, 0, totalFrames - 1);
        }

        // devuelve el frame actual
        public int GetFrame() => frameActual;

        // opcional: clona la animacion sin compartir estado
        public Animacion Clone()
        {
            return new Animacion(textura, anchoFrame, altoFrame, duracionFrame, enBucle);
        }
    }
}
