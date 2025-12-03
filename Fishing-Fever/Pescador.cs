using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Fishing_Fever
{
    public class Pescador
    {
        private Texture2D rojoTirarCaña;    
        private Texture2D rojoEsfuerzoCaña; 
        
        public Vector2 Posicion { get; private set; } 

        public int FrameActual { get; set; } = 0;         
        private double tiempoFrame = 0;      
        private double duracionFrame = 0.15;  

        public bool AnimandoTiro { get; set; } = false;    
        public bool EnEsfuerzo { get; private set; } = false;      

        public Pescador(ContentManager content, Vector2 posicionInicial)
        {
            // Carga las texturas (asegúrate que Content/Images existe)
            rojoTirarCaña = content.Load<Texture2D>("Images/rojoTirarCaña");
            rojoEsfuerzoCaña = content.Load<Texture2D>("Images/rojoEsfuerzoCaña");
            Posicion = posicionInicial;
        }

        public void Lanzar()
        {
            if (!EnEsfuerzo && !AnimandoTiro)
            {
                AnimandoTiro = true;
                FrameActual = 0;
                tiempoFrame = 0;
            }
        }

        public void EmpezarEsfuerzo()
        {
            EnEsfuerzo = true;
            AnimandoTiro = false;
        }

        public void Resetear()
        {
            EnEsfuerzo = false;
            AnimandoTiro = false;
            FrameActual = 0;
        }

        public void Update(GameTime gameTime)
        {
            tiempoFrame += gameTime.ElapsedGameTime.TotalSeconds;

            if (AnimandoTiro)
            {
                if (tiempoFrame >= duracionFrame)
                {
                    FrameActual++;
                    tiempoFrame = 0;
                    if (FrameActual >= 3)
                    {
                        FrameActual = 3; // Se queda en la pose final de tiro
                    }
                }
            }
            else if (EnEsfuerzo)
            {
                // Animación de esfuerzo (bucle)
                if (tiempoFrame >= duracionFrame * 4)
                {
                    FrameActual++;
                    tiempoFrame = 0;
                    if (FrameActual > 2) FrameActual = 1; 
                }
            }
            else
            {
                FrameActual = 0; // Estado idle
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D spriteActual = EnEsfuerzo ? rojoEsfuerzoCaña : rojoTirarCaña;
            
            // Define el frame actual (asumiendo frames de 30x40 píxeles)
            Rectangle sourceRect = new Rectangle(FrameActual * 30, 0, 30, 40);

            // Escala 3x
            spriteBatch.Draw(spriteActual, Posicion, sourceRect, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
        }
    }
}