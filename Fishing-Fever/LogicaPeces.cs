using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Fishing_Fever
{
    public class LogicaPeces
    {
        private Texture2D spritePez;
        private Vector2 posicionPez;
        private bool mostrarPez;

        public LogicaPeces(ContentManager content)
        {
            // cargamos el sprite del pez capturado
            spritePez = content.Load<Texture2D>("Images/pezAlga");

            // posición donde va a aparecer el pez capturado
            posicionPez = new Vector2(100, 300);

            mostrarPez = false;
        }

        // este método lo llama Game1 cuando PescaBarra termina
        public void MostrarPez()
        {
            mostrarPez = true;
        }

        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (mostrarPez)
            {
                spriteBatch.Draw(spritePez, posicionPez, Color.White);
            }
        }
    }
}
