using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Fishing_Fever
{
    // Clase que maneja el pez atrapado y lo muestra
    public class LogicaPeces
    {
        // Colección de todas las texturas de peces disponibles
        private List<Texture2D> texturasDisponibles;
        
        // Textura del pez atrapado actualmente
        private Texture2D pezActual; 
        
        private bool pezAtrapado = false;
        private Vector2 posicionPez;
        private Random random;

        // Temporizador para esconder el pez después de un tiempo
        private float tiempoVisible = 0f;
        private const float DURACION_VISIBILIDAD = 3.0f; // Pez visible por 3 segundos

        private List<int> scoresDisponibles;
        public int CurrentScore { get; private set; } = 0;

        public LogicaPeces(ContentManager content)
        {
            random = new Random();
            texturasDisponibles = new List<Texture2D>();

            // --- LISTA DE PECES A CARGAR ---
            // IMPORTANTE: Asegúrate de que estos archivos existan en Content/Images/
            string[] nombresDePeces = { "Pez alga", "Pez burbuja", "Pez luna", "Pez cuchillo", "Pez fantasma", "Pez piedra", "Pez rayo", "Tiburon martillo" }; 
            
            // Cargar cada textura y añadirla a la lista
            foreach (string nombre in nombresDePeces)
            {
                try
                {
                    Texture2D pez = content.Load<Texture2D>("Images/" + nombre);
                    texturasDisponibles.Add(pez);
                    Console.WriteLine($"Pez cargado: {nombre}");
                }
                catch (ContentLoadException)
                {
                    Console.WriteLine($"ERROR: No se pudo cargar 'Images/{nombre}'. ¿Existe el archivo?");
                }
            }

            scoresDisponibles = new List<int> { 10, 20, 30, 40, 50, 60, 70, 100 }; // Scores corresponding to fish names
            posicionPez = new Vector2(100, 100);
        }

        public void MostrarPez()
        {
            if (texturasDisponibles.Count > 0)
            {
                // 1. Selecciona un pez al azar
                int indice = random.Next(texturasDisponibles.Count);
                pezActual = texturasDisponibles[indice];
                CurrentScore = scoresDisponibles[indice];
                
                // 2. Activa el estado y el temporizador
                pezAtrapado = true;
                tiempoVisible = DURACION_VISIBILIDAD; 
            }
        }
        
        // Método para esconder el pez al iniciar una nueva pesca o manualmente
        public void EsconderPez()
        {
            pezAtrapado = false;
            tiempoVisible = 0f;
            pezActual = null; // Opcional: Liberar la referencia
        }

        public void Update(GameTime gameTime)
        {
            if (pezAtrapado)
            {
                tiempoVisible -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (tiempoVisible <= 0)
                {
                    EsconderPez(); // Oculta el pez cuando el tiempo se agota
                }
            }
        }

        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (pezAtrapado && pezActual != null)
            {
                // Escala de visualización (la misma que antes)
                float escala = 4f; 
                
                // Dibuja el pez seleccionado
                spriteBatch.Draw(pezActual, posicionPez, null, Color.White, 0f, Vector2.Zero, escala, SpriteEffects.None, 0f);
            }
        }
    }
}