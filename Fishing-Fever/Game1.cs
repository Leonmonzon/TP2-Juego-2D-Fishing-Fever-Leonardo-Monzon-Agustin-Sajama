using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;

namespace Fishing_Fever
{
    public enum GameState { Jugando, Pausado }

    public class Game1 : Game
    {
        private GameState estadoActual = GameState.Jugando;
        private bool musicaSilenciada = false;
        private Song musicaJuego;
        private bool musicaIniciada = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private KeyboardState estadoTecladoActual;
        private KeyboardState estadoTecladoAnterior;
        private MouseState mouseAnterior;

        private LogicaPeces logicaPeces;
        private PescaBarra barraPesca;
        private Texture2D fondo;
        private Pescador pescador;
        private Anzuelo anzuelo;
        private Texture2D pixelTexture;
        private SpriteFont fuente;
        
        // POSICIÓN ORIGINAL
        private const float NIVEL_AGUA = 475f;

        private Rectangle btnReanudar, btnSilenciar, btnMenuPrincipal, btnSalir;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            ConfigurarBotonesPausa();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            fondo = Content.Load<Texture2D>("Images/fondo");
            musicaJuego = Content.Load<Song>("Audio/musicaJuego");
            fuente = Content.Load<SpriteFont>("Fonts/fuente"); 

            logicaPeces = new LogicaPeces(Content);
            
            // Posición de la barra de pesca (Original)
            barraPesca = new PescaBarra(GraphicsDevice, new Vector2(700, 200));
            
            // POSICIÓN DEL PESCADOR (Original de tu código)
            Vector2 posPescador = new Vector2(
                (_graphics.PreferredBackBufferWidth / 2) + 75 - 5,
                (_graphics.PreferredBackBufferHeight / 2) - 27 + 25f
            );
            
            pescador = new Pescador(Content, posPescador);
            anzuelo = new Anzuelo(pixelTexture, NIVEL_AGUA);
        }

        private void ConfigurarBotonesPausa()
        {
            int w = _graphics.PreferredBackBufferWidth;
            int h = _graphics.PreferredBackBufferHeight;
            int btnW = 280;
            int btnH = 55;
            int centerX = (w / 2) - (btnW / 2);
            int startY = h / 2 - 120; // Ajustado para 4 botones

            btnReanudar = new Rectangle(centerX, startY, btnW, btnH);
            btnSilenciar = new Rectangle(centerX, startY + 75, btnW, btnH);
            btnMenuPrincipal = new Rectangle(centerX, startY + 150, btnW, btnH);
            btnSalir = new Rectangle(centerX, startY + 225, btnW, btnH);
        }

        protected override void Update(GameTime gameTime)
        {
            estadoTecladoActual = Keyboard.GetState();
            MouseState mouseActual = Mouse.GetState();
            bool clickIzquierdo = mouseActual.LeftButton == ButtonState.Pressed && mouseAnterior.LeftButton == ButtonState.Released;

            if (estadoTecladoActual.IsKeyDown(Keys.Escape) && estadoTecladoAnterior.IsKeyUp(Keys.Escape))
            {
                estadoActual = (estadoActual == GameState.Jugando) ? GameState.Pausado : GameState.Jugando;
            }

            if (estadoActual == GameState.Jugando)
            {
                ActualizarLogicaJuego(gameTime, mouseActual, clickIzquierdo);
            }
            else
            {
                ActualizarMenuPausa(mouseActual, clickIzquierdo);
            }

            estadoTecladoAnterior = estadoTecladoActual;
            mouseAnterior = mouseActual;
            base.Update(gameTime);
        }

        private void ActualizarLogicaJuego(GameTime gameTime, MouseState mouseActual, bool clickIzquierdo)
        {
            if (!musicaIniciada)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(musicaJuego);
                musicaIniciada = true;
            }

            logicaPeces.Update(gameTime);

            if (barraPesca.EstaActiva())
            {
                barraPesca.Actualizar(gameTime);
                if (barraPesca.PescaCompletada)
                {
                    logicaPeces.MostrarPez();
                    pescador.Resetear();
                    anzuelo.Resetear();
                }
            }
            else
            {
                pescador.Update(gameTime);
                anzuelo.Update(gameTime);

                if (anzuelo.Estado == EstadoAnzuelo.Listo && clickIzquierdo)
                {
                    pescador.Lanzar();
                    logicaPeces.EsconderPez();
                }

                if (anzuelo.Estado == EstadoAnzuelo.Listo && pescador.AnimandoTiro && pescador.FrameActual >= 2)
                {
                    Vector2 puntaCaña = pescador.Posicion + new Vector2(80, 60);
                    anzuelo.Lanzar(puntaCaña, new Vector2(-300f, -300f));
                    pescador.AnimandoTiro = false;
                }
                else if (anzuelo.Estado == EstadoAnzuelo.Pico && clickIzquierdo)
                {
                    barraPesca.Activar();
                    pescador.EmpezarEsfuerzo();
                }
                else if ((anzuelo.Estado == EstadoAnzuelo.Volando || anzuelo.Estado == EstadoAnzuelo.Esperando) && clickIzquierdo)
                {
                    anzuelo.Resetear();
                    pescador.Resetear();
                    logicaPeces.EsconderPez();
                }
            }
        }

        private void ActualizarMenuPausa(MouseState mouse, bool click)
        {
            if (click)
            {
                if (btnReanudar.Contains(mouse.Position)) estadoActual = GameState.Jugando;
                if (btnSilenciar.Contains(mouse.Position)) { musicaSilenciada = !musicaSilenciada; MediaPlayer.IsMuted = musicaSilenciada; }
                if (btnMenuPrincipal.Contains(mouse.Position)) { Process.Start(Process.GetCurrentProcess().MainModule.FileName); Exit(); }
                if (btnSalir.Contains(mouse.Position)) Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            // FONDO ORIGINAL (Con FlipHorizontally como lo tenías)
            _spriteBatch.Draw(fondo, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            
            if (anzuelo.Estado != EstadoAnzuelo.Listo)
            {
                Vector2 origenLinea = barraPesca.EstaActiva() ? pescador.Posicion + new Vector2(15, 10) : pescador.Posicion + new Vector2(10, 50);
                DrawLine(_spriteBatch, origenLinea, anzuelo.Posicion, Color.Black, 1);
            }

            pescador.Draw(_spriteBatch);
            anzuelo.Draw(_spriteBatch, gameTime);
            barraPesca.Dibujar(_spriteBatch);
            logicaPeces.Dibujar(_spriteBatch);

            if (estadoActual == GameState.Pausado)
            {
                // Overlay oscuro suave
                _spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.Black * 0.5f);
                
                MouseState ms = Mouse.GetState();
                DibujarBotonEstetico(btnReanudar, "REANUDAR", ms);
                DibujarBotonEstetico(btnSilenciar, musicaSilenciada ? "SONIDO: OFF" : "SONIDO: ON", ms);
                DibujarBotonEstetico(btnMenuPrincipal, "MENU PRINCIPAL", ms);
                DibujarBotonEstetico(btnSalir, "SALIR", ms);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DibujarBotonEstetico(Rectangle rect, string texto, MouseState ms)
        {
            bool estaEncima = rect.Contains(ms.Position);
            Color colorBoton = estaEncima ? Color.DarkSlateBlue : Color.FromNonPremultiplied(40, 40, 40, 220);
            Color colorBorde = estaEncima ? Color.Gold : Color.White * 0.4f;

            _spriteBatch.Draw(pixelTexture, new Rectangle(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4), colorBorde);
            _spriteBatch.Draw(pixelTexture, rect, colorBoton);

            Vector2 textSize = fuente.MeasureString(texto);
            Vector2 textPos = new Vector2(rect.X + (rect.Width / 2) - (textSize.X / 2), rect.Y + (rect.Height / 2) - (textSize.Y / 2));
            _spriteBatch.DrawString(fuente, texto, textPos, estaEncima ? Color.Gold : Color.White);
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 diff = end - start;
            float dist = diff.Length();
            float rot = (float)Math.Atan2(diff.Y, diff.X);
            spriteBatch.Draw(pixelTexture, start, null, color, rot, Vector2.Zero, new Vector2(dist, thickness), SpriteEffects.None, 0);
        }
    }
}