using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fishing_Fever
{
    public class PescaBarra
    {
        // Texturas básicas (podís usar rectángulos de color si no tienes sprites)
        private Texture2D texturaFondo;
        private Texture2D texturaBarra;
        private Texture2D texturaPez;

        // Posiciones y tamaños
        private Rectangle rectFondo;
        private Rectangle rectBarra;
        private Rectangle rectPez;

        // Movimiento de la barra
        private float velocidadBarra = 200f;
        private float posicionBarra;
        private bool subiendo;

        // Movimiento del pez
        private float posicionPez;
        private float tiempoCambioPez;
        private float tiempoAcumulado;

        // Estado general
        private bool activo;
        private double progreso;  // cuánto llevamos pescando
        private double progresoObjetivo = 5; // segundos para "ganar"

        public bool PescaCompletada { get; private set; }

        public PescaBarra(GraphicsDevice graphicsDevice, Vector2 posicion)
        {
            // Crear texturas simples de color sólido
            texturaFondo = new Texture2D(graphicsDevice, 1, 1);
            texturaFondo.SetData(new[] { Color.Gray });

            texturaBarra = new Texture2D(graphicsDevice, 1, 1);
            texturaBarra.SetData(new[] { Color.Green });

            texturaPez = new Texture2D(graphicsDevice, 1, 1);
            texturaPez.SetData(new[] { Color.Yellow });

            rectFondo = new Rectangle((int)posicion.X, (int)posicion.Y, 40, 200);
            rectBarra = new Rectangle(rectFondo.X, rectFondo.Bottom - 50, 40, 50);
            rectPez = new Rectangle(rectFondo.X, rectFondo.Y + 100, 40, 20);

            posicionBarra = rectBarra.Y;
            posicionPez = rectPez.Y;

            activo = false;
        }

        // Activa la barra cuando se tira la caña
        public void Activar()
        {
            activo = true;
            PescaCompletada = false;
            progreso = 0;
            posicionBarra = rectFondo.Bottom - rectBarra.Height;
            subiendo = false;
            posicionPez = rectFondo.Y + rectFondo.Height / 2;
            tiempoCambioPez = 1f;
            tiempoAcumulado = 0;
        }

        // Actualiza la lógica de la barra y el pez
        public void Actualizar(GameTime gameTime)
        {
            if (!activo) return;

            var teclado = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Movimiento manual de la barra con espacio o clic sostenido
            var raton = Mouse.GetState();
            if (teclado.IsKeyDown(Keys.Space) || raton.LeftButton == ButtonState.Pressed)
                subiendo = true;
            else
                subiendo = false;

            // Movimiento de la barra verde
            if (subiendo)
                posicionBarra -= velocidadBarra * delta;
            else
                posicionBarra += velocidadBarra * delta;

            // Limita dentro del fondo
            posicionBarra = MathHelper.Clamp(posicionBarra, rectFondo.Y, rectFondo.Bottom - rectBarra.Height);
            rectBarra.Y = (int)posicionBarra;

            // Movimiento del pez (aleatorio cada cierto tiempo)
            tiempoAcumulado += delta;
            if (tiempoAcumulado >= tiempoCambioPez)
            {
                tiempoAcumulado = 0;
                tiempoCambioPez = 0.5f + (float)new System.Random().NextDouble() * 1.5f;
                posicionPez += (float)(new System.Random().NextDouble() * 100 - 50);
            }

            // Limitar al fondo
            posicionPez = MathHelper.Clamp(posicionPez, rectFondo.Y, rectFondo.Bottom - rectPez.Height);
            rectPez.Y = (int)posicionPez;

            // Si la barra verde está sobre el pez, sumamos progreso
            if (rectBarra.Intersects(rectPez))
                progreso += delta;
            else
                progreso -= delta * 0.5f; // castigo si no lo seguimos

            progreso = MathHelper.Clamp((float)progreso, 0, (float)progresoObjetivo);

            // Si llegamos al objetivo => pesca completada
            if (progreso >= progresoObjetivo)
            {
                PescaCompletada = true;
                activo = false;
            }
        }

        // Dibuja la barra y el pez
        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (!activo && !PescaCompletada) return;

            spriteBatch.Draw(texturaFondo, rectFondo, Color.DarkSlateGray);
            spriteBatch.Draw(texturaPez, rectPez, Color.Yellow);
            spriteBatch.Draw(texturaBarra, rectBarra, Color.Green);

            // Si completamos, mostrar barra dorada
            if (PescaCompletada)
                spriteBatch.Draw(texturaFondo, rectFondo, Color.Gold);
        }

        public bool EstaActiva() => activo;
    }
}
