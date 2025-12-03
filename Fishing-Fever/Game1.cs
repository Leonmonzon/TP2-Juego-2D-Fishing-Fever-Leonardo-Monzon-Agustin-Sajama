using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System; 
// Asegúrate de que todas las clases (Pescador, Anzuelo, PescaBarra, LogicaPeces)
// usan el mismo namespace 'Fishing_Fever'

namespace Fishing_Fever
{
    // Clase principal del juego
    public class Game1 : Game
    {
        // AJUSTA ESTE VALOR: Coordenada Y donde el anzuelo debe flotar (Nivel del Agua)
        private const float NIVEL_AGUA = 380f; 

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Declaración de variables necesarias
        private LogicaPeces logicaPeces;
        private PescaBarra barraPesca;
        private Texture2D fondo;
        private Pescador pescador;
        private Anzuelo anzuelo; 

        // Textura auxiliar: un pixel blanco para dibujar la línea de pesca y la boya
        private Texture2D pixelTexture; 

        private MouseState mouseAnterior; // Necesario para detectar un solo click

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Configuración de la ventana (Ejemplo: 800x600)
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // 1. CREACIÓN DE TEXTURA AUXILIAR (Pixel Blanco)
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            // 2. Carga de Assets y Clases
            // Asegúrate que 'fondo.png' existe en Content/Images/
            fondo = Content.Load<Texture2D>("Images/fondo");
            
            // Inicialización de las CLASES (Objetos)
            logicaPeces = new LogicaPeces(Content); 
            // Posición de la barra de pesca
            barraPesca = new PescaBarra(GraphicsDevice, new Vector2(700, 200));

            // Posicion del pescador
            // Usamos la posición ajustada
            Vector2 posPescador = new Vector2(
                (_graphics.PreferredBackBufferWidth / 2) + 75 - 15, 
                (_graphics.PreferredBackBufferHeight / 2) - 27
            );
            // NOTA: ASUMIMOS QUE LA CLASE PESCADOR.CS EXISTE
            pescador = new Pescador(Content, posPescador);

            // 3. Inicializamos el anzuelo (Ahora la clase Anzuelo existe)
            anzuelo = new Anzuelo(pixelTexture, NIVEL_AGUA);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            MouseState mouseActual = Mouse.GetState();

            // [CRÍTICO] El temporizador del pez DEBE correr siempre.
            logicaPeces.Update(gameTime); 

            // Detecta un nuevo clic (presionado ahora, pero no en el frame anterior)
            bool clickIzquierdo = mouseActual.LeftButton == ButtonState.Pressed && 
                                  mouseAnterior.LeftButton == ButtonState.Released;

            // 1. Lógica del Minijuego (tiene prioridad)
            if (barraPesca.EstaActiva())
            {
                barraPesca.Actualizar(gameTime);
                
                if (barraPesca.PescaCompletada)
                {
                    // ¡Pesca completada! Muestra el pez y resetea el ciclo de pesca
                    logicaPeces.MostrarPez();
                    // ASUMIMOS QUE ESTOS MÉTODOS EXISTEN EN PESCADOR Y ANZUELO
                    pescador.Resetear(); 
                    anzuelo.Resetear(); 
                }
            }
            // 2. Lógica de Pesca Normal (Solo si el minijuego NO está activo)
            else
            {
                // ASUMIMOS QUE ESTOS MÉTODOS EXISTEN EN PESCADOR Y ANZUELO
                pescador.Update(gameTime);
                anzuelo.Update(gameTime);

                // Caso A: Anzuelo en mano (Listo) y Clic -> Iniciar Lanzamiento
                if (anzuelo.Estado == EstadoAnzuelo.Listo && clickIzquierdo)
                {
                    pescador.Lanzar();
                    // Escondemos el pez anterior al iniciar una nueva pesca
                    logicaPeces.EsconderPez(); 
                }

                // Caso B: Ejecutar Lanzamiento físico (Cuando la animación del pescador lo permite)
                if (anzuelo.Estado == EstadoAnzuelo.Listo && pescador.AnimandoTiro && pescador.FrameActual >= 2)
                {
                    Vector2 puntaCaña = pescador.Posicion + new Vector2(80, 20); 
                    anzuelo.Lanzar(puntaCaña, new Vector2(-250f, -300f));
                    pescador.AnimandoTiro = false; 
                }

                // Caso C: El pez PICÓ (Anzuelo.Estado == Pico) y Clic -> EMPEZAR MINIJUEGO
                else if (anzuelo.Estado == EstadoAnzuelo.Pico && clickIzquierdo)
                {
                    barraPesca.Activar(); 
                    pescador.EmpezarEsfuerzo(); 
                }

                // Caso D: Recoger línea (si está volando o esperando) y Clic
                else if ((anzuelo.Estado == EstadoAnzuelo.Volando || anzuelo.Estado == EstadoAnzuelo.Esperando) && clickIzquierdo)
                {
                    // Reinicia todo el ciclo de pesca
                    anzuelo.Resetear();
                    pescador.Resetear();
                    logicaPeces.EsconderPez(); 
                }
            }

            mouseAnterior = mouseActual;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // 1. Fondo (Usando tu configuracion de FlipHorizontally)
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

            // 2. Dibujar la línea de pesca si está lanzada
            if (anzuelo.Estado != EstadoAnzuelo.Listo)
            {
                Vector2 origenLinea = pescador.Posicion + new Vector2(80, 20);
                DrawLine(_spriteBatch, origenLinea, anzuelo.Posicion, Color.Black, 1);
            }

            // 3. Entidades
            pescador.Draw(_spriteBatch);
            anzuelo.Draw(_spriteBatch, gameTime); // Pasamos gameTime para el parpadeo del anzuelo

            // 4. UI y Minijuego
            barraPesca.Dibujar(_spriteBatch);
            logicaPeces.Dibujar(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        
        // Función auxiliar para dibujar una línea simple (con el pixelTexture)
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 difference = end - start;
            float distance = difference.Length();
            float rotation = (float)Math.Atan2(difference.Y, difference.X);

            spriteBatch.Draw(
                pixelTexture, 
                start, 
                null, 
                color, 
                rotation, 
                Vector2.Zero, 
                new Vector2(distance, thickness), 
                SpriteEffects.None, 
                0
            );
        }
    }
}