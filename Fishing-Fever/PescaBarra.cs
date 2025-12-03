using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Fishing_Fever
{
    // Clase que representa el minijuego de la barra de pesca
    public class PescaBarra
    {
        // Dimensiones de la barra (constantes)
        private const int ALTURA_BARRA = 300;
        private const int ANCHO_BARRA = 20;

        // Propiedades de la Barra
        private Texture2D texturaFondo;         // Fondo (Gris)
        private Texture2D texturaIndicador;     // Indicador del Jugador (Blanco/Rojo)
        private Texture2D texturaZonaObjetivo;  // Zona Verde del Pez

        private Vector2 posicion;
        private bool activa = false;

        // Lógica del Indicador del Jugador (Valores Ajustados)
        private float indicadorY; 
        private float velocidadIndicador;
        private float gravedad = 1100f; // Fuerza para caer (Ajustado: más firme)
        private float fuerzaImpulso = 300f; // Fuerza para subir con el clic (Ajustado: menos fuerza = menos salto)
        private int alturaIndicador = 30; // Altura del bloque del jugador

        // Lógica de la Zona Objetivo del Pez (Zona Verde)
        private float zonaObjetivoY; 
        private float velocidadZona = 100f; // Velocidad con la que se mueve la zona verde
        private int alturaZona = 60; // Altura del bloque verde del pez

        // Progreso del Minijuego
        private float progresoActual = 0f; // 0.0 a 1.0 (0% a 100%)
        private const float PROGRESO_GANAR = 1.0f;
        private const float PROGRESO_PERDER_FUERA_ZONA = 0.1f; // Tasa de pérdida si estás fuera de la zona

        public bool PescaCompletada { get; private set; } = false;

        private MouseState mouseAnterior;

        public PescaBarra(GraphicsDevice graphicsDevice, Vector2 pos)
        {
            posicion = pos;
            
            // 1. Crear Texturas
            // Fondo (Barra Gris)
            texturaFondo = new Texture2D(graphicsDevice, ANCHO_BARRA, ALTURA_BARRA);
            Color[] dataFondo = new Color[ANCHO_BARRA * ALTURA_BARRA];
            for (int i = 0; i < dataFondo.Length; ++i) dataFondo[i] = Color.Gray * 0.7f;
            texturaFondo.SetData(dataFondo);

            // Indicador del Jugador (Blanco)
            texturaIndicador = new Texture2D(graphicsDevice, ANCHO_BARRA, alturaIndicador);
            Color[] dataIndicador = new Color[ANCHO_BARRA * alturaIndicador];
            for (int i = 0; i < dataIndicador.Length; ++i) dataIndicador[i] = Color.White;
            texturaIndicador.SetData(dataIndicador);

            // Zona Objetivo (Verde)
            texturaZonaObjetivo = new Texture2D(graphicsDevice, ANCHO_BARRA, alturaZona);
            Color[] dataZona = new Color[ANCHO_BARRA * alturaZona];
            for (int i = 0; i < dataZona.Length; ++i) dataZona[i] = Color.LimeGreen * 0.8f;
            texturaZonaObjetivo.SetData(dataZona);
        }

        public void Activar()
        {
            activa = true;
            PescaCompletada = false;
            progresoActual = 0f;
            
            // Inicializar posición del jugador y de la zona objetivo
            indicadorY = ALTURA_BARRA / 2f; 
            zonaObjetivoY = ALTURA_BARRA / 2f; 
            velocidadIndicador = 0f;
        }

        public bool EstaActiva() => activa;

        public void Actualizar(GameTime gameTime)
        {
            if (!activa) return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mouseActual = Mouse.GetState();

            // --------------------
            // 1. Lógica del Indicador del Jugador (Gravedad y Control)
            // --------------------
            
            // Aplicar Impulso si se hace clic
            if (mouseActual.LeftButton == ButtonState.Pressed)
            {
                // Aplica el impulso reducido
                velocidadIndicador = -fuerzaImpulso; 
            }

            // Aplicar Gravedad
            velocidadIndicador += gravedad * delta;
            indicadorY += velocidadIndicador * delta;

            // Restringir el indicador a los límites de la barra
            if (indicadorY < 0)
            {
                indicadorY = 0;
                velocidadIndicador = 0;
            }
            if (indicadorY > ALTURA_BARRA - alturaIndicador)
            {
                indicadorY = ALTURA_BARRA - alturaIndicador;
                velocidadIndicador = 0;
            }
            
            // --------------------
            // 2. Lógica de la Zona Objetivo (Pez)
            // --------------------
            
            zonaObjetivoY += velocidadZona * delta;

            // Invertir dirección si la zona choca con los bordes
            if (zonaObjetivoY < 0 || zonaObjetivoY > ALTURA_BARRA - alturaZona)
            {
                velocidadZona *= -1;
                // Ajustar ligeramente la posición para evitar que se pegue al borde
                zonaObjetivoY = MathHelper.Clamp(zonaObjetivoY, 0, ALTURA_BARRA - alturaZona);
            }
            
            // --------------------
            // 3. Lógica del Progreso
            // --------------------
            
            // Calcular si el indicador del jugador está DENTRO de la zona objetivo
            bool estaEnZona = 
                indicadorY >= zonaObjetivoY && 
                indicadorY + alturaIndicador <= zonaObjetivoY + alturaZona;

            if (estaEnZona)
            {
                // Aumentar el progreso
                progresoActual += delta * 0.3f; 
            }
            else
            {
                // Disminuir el progreso
                progresoActual -= delta * PROGRESO_PERDER_FUERA_ZONA;
            }

            // Asegurar que el progreso se mantenga entre 0 y 1
            progresoActual = MathHelper.Clamp(progresoActual, 0f, PROGRESO_GANAR);

            // --------------------
            // 4. Chequeo de Victoria/Derrota
            // --------------------

            if (progresoActual >= PROGRESO_GANAR)
            {
                PescaCompletada = true;
                activa = false;
            }
            
            mouseAnterior = mouseActual;
        }

        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (!activa) return;

            // Posición base de la barra en la pantalla
            Vector2 posPantalla = posicion;
            
            // 1. Dibuja el Fondo de la Barra (Gris)
            spriteBatch.Draw(texturaFondo, posPantalla, Color.White);

            // 2. Dibuja la Zona Objetivo (Verde)
            Vector2 posZona = new Vector2(posPantalla.X, posPantalla.Y + zonaObjetivoY);
            spriteBatch.Draw(texturaZonaObjetivo, posZona, Color.White);

            // 3. Dibuja el Indicador del Jugador (Blanco)
            Vector2 posIndicador = new Vector2(posPantalla.X, posPantalla.Y + indicadorY);
            
            // Si está fuera de la zona, el indicador se pone rojo para feedback
            Color colorIndicador = (PescaCompletada || (indicadorY >= zonaObjetivoY && indicadorY + alturaIndicador <= zonaObjetivoY + alturaZona)) 
                                    ? Color.White 
                                    : Color.Red;

            spriteBatch.Draw(texturaIndicador, posIndicador, colorIndicador);

            // 4. Dibuja la Barra de Progreso (al lado de la barra principal)
            
            // Usaremos el fondo gris como "fondo" para el progreso
            Vector2 posProgresoBase = posPantalla + new Vector2(ANCHO_BARRA + 5, 0);
            spriteBatch.Draw(texturaFondo, posProgresoBase, Color.Black * 0.5f); // Fondo de progreso (Negro semi-transparente)

            // Indicador de Progreso (Amarillo)
            float alturaProgreso = ALTURA_BARRA * progresoActual;
            Rectangle rectProgreso = new Rectangle(
                (int)posProgresoBase.X, 
                (int)(posProgresoBase.Y + ALTURA_BARRA - alturaProgreso), // Dibuja desde abajo
                ANCHO_BARRA, 
                (int)alturaProgreso
            );
            // El color cambia a verde cuando está cerca de ganar
            Color colorProgreso = Color.Lerp(Color.Yellow, Color.ForestGreen, progresoActual);
            
            spriteBatch.Draw(texturaFondo, rectProgreso, colorProgreso);
        }
    }
}