using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Fishing_Fever
{
    // 1. Definicion de los posibles estados del anzuelo
    public enum EstadoAnzuelo
    {
        Listo,      // En mano, listo para lanzar
        Volando,    // En el aire
        Esperando,  // Flotando en el agua, esperando el pique
        Pico        // El pez ha picado (parpadeando)
    }

    // 2. Clase que gestiona el anzuelo y la boya
    public class Anzuelo
    {
        private Texture2D pixelTexture; // Para dibujar la boya
        private float nivelAgua;
        
        public EstadoAnzuelo Estado { get; private set; }
        public Vector2 Posicion { get; private set; }
        private Vector2 velocidad;
        private float gravedad = 900f; 

        // Temporizador para el estado de espera/pique
        private float temporizadorEspera = 0f;
        private const float TIEMPO_ESPERA_MIN = 2.0f; // Mínimo antes de un pique
        private const float TIEMPO_ESPERA_MAX = 5.0f; // Máximo antes de un pique
        
        // Control de parpadeo para el estado PICO
        private float temporizadorParpadeo = 0f;
        private bool esVisible = true;

        public Anzuelo(Texture2D pixel, float nivelAguaY)
        {
            pixelTexture = pixel;
            nivelAgua = nivelAguaY;
            Resetear();
        }

        public void Resetear()
        {
            Estado = EstadoAnzuelo.Listo;
            Posicion = Vector2.Zero; // La posicion real la gestiona el pescador en este estado
            velocidad = Vector2.Zero;
            temporizadorEspera = 0f;
            temporizadorParpadeo = 0f;
            esVisible = true;
        }

        public void Lanzar(Vector2 posInicial, Vector2 velInicial)
        {
            Estado = EstadoAnzuelo.Volando;
            Posicion = posInicial;
            velocidad = velInicial;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Estado == EstadoAnzuelo.Volando)
            {
                // Aplicar gravedad
                velocidad.Y += gravedad * delta;
                Posicion += velocidad * delta;

                // Chequeo de colisión con el agua
                if (Posicion.Y >= nivelAgua)
                {
                    Posicion = new Vector2(Posicion.X, nivelAgua);
                    Estado = EstadoAnzuelo.Esperando;
                    
                    // Iniciar el temporizador para el pique
                    temporizadorEspera = TIEMPO_ESPERA_MIN + (float)new Random().NextDouble() * (TIEMPO_ESPERA_MAX - TIEMPO_ESPERA_MIN);
                }
            }
            else if (Estado == EstadoAnzuelo.Esperando)
            {
                // Contar tiempo hasta el pique
                temporizadorEspera -= delta;
                if (temporizadorEspera <= 0)
                {
                    Estado = EstadoAnzuelo.Pico;
                    temporizadorParpadeo = 0f; // Reiniciar parpadeo
                }
            }
            else if (Estado == EstadoAnzuelo.Pico)
            {
                // Gestionar el parpadeo
                temporizadorParpadeo += delta;
                if (temporizadorParpadeo > 0.15f) // Cambia la visibilidad cada 0.15 segundos
                {
                    esVisible = !esVisible;
                    temporizadorParpadeo = 0f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Solo dibujar si no está en estado Listo y no está parpadeando invisible
            if (Estado != EstadoAnzuelo.Listo && (Estado != EstadoAnzuelo.Pico || esVisible))
            {
                // Dibuja la boya (un círculo o un cuadrado pequeño)
                Rectangle rect = new Rectangle((int)Posicion.X, (int)Posicion.Y, 10, 10);
                
                Color colorBoya = Color.White;
                if (Estado == EstadoAnzuelo.Esperando) colorBoya = Color.Yellow;
                if (Estado == EstadoAnzuelo.Pico) colorBoya = Color.Red;
                
                spriteBatch.Draw(pixelTexture, rect, colorBoya);
            }
        }
    }
}