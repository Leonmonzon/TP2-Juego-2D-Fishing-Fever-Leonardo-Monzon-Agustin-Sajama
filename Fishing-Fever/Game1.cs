using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;

namespace Fishing_Fever
{
    public enum GameState { Jugando, Pausado }

    public class Game1 : Game
    {
        private GameState estadoActual = GameState.Jugando;
        
        // SISTEMA DE VOLUMEN ESTÁTICO (Para compartir con la Intro)
        public static float[] NivelesVolumen = { 0.0f, 0.2f, 0.5f, 1.0f };
        public static string[] NombresVolumen = { "SILENCIO", "BAJO", "MEDIO", "ALTO" };
        public static int IndiceVolumen = 2; // Por defecto MEDIO

        private Song musicaJuego;
        private bool musicaIniciada = false;

        private SoundEffect sonidoCaptura;
        private SoundEffect sonidoTiro;

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
        private const float NIVEL_AGUA = 475f;

        private Rectangle btnReanudar, btnVolumen, btnMenuPrincipal, btnSalir;

        private bool esperandoDespuesCaptura = false;
        private float tiempoEsperaCaptura = 0f;
        private const float DELAY_CAPTURA = 2.0f; // 2 seconds delay

        private int score = 0;

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

            sonidoCaptura = Content.Load<SoundEffect>("Audio/sonidoCapturado");
            sonidoTiro = Content.Load<SoundEffect>("Audio/sonidoTiro");

            logicaPeces = new LogicaPeces(Content);
            barraPesca = new PescaBarra(GraphicsDevice, new Vector2(700, 200));
            
            Vector2 posPescador = new Vector2((_graphics.PreferredBackBufferWidth / 2) + 70, (_graphics.PreferredBackBufferHeight / 2) + 5);
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
            int startY = h / 2 - 120;

            btnReanudar = new Rectangle(centerX, startY, btnW, btnH);
            btnVolumen = new Rectangle(centerX, startY + 75, btnW, btnH);
            btnMenuPrincipal = new Rectangle(centerX, startY + 150, btnW, btnH);
            btnSalir = new Rectangle(centerX, startY + 225, btnW, btnH);
        }

        protected override void Update(GameTime gameTime)
        {
            estadoTecladoActual = Keyboard.GetState();
            MouseState mouseActual = Mouse.GetState();
            bool click = mouseActual.LeftButton == ButtonState.Pressed && mouseAnterior.LeftButton == ButtonState.Released;

            if (estadoTecladoActual.IsKeyDown(Keys.Escape) && estadoTecladoAnterior.IsKeyUp(Keys.Escape))
                estadoActual = (estadoActual == GameState.Jugando) ? GameState.Pausado : GameState.Jugando;

            if (estadoActual == GameState.Jugando)
                ActualizarLogicaJuego(gameTime, mouseActual, click);
            else
                ActualizarMenuPausa(mouseActual, click);

            estadoTecladoAnterior = estadoTecladoActual;
            mouseAnterior = mouseActual;
            base.Update(gameTime);
        }

        private void ActualizarLogicaJuego(GameTime gameTime, MouseState mouseActual, bool click)
        {
            if (!musicaIniciada)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = NivelesVolumen[IndiceVolumen];
                MediaPlayer.Play(musicaJuego);
                musicaIniciada = true;
            }

            MediaPlayer.Volume = NivelesVolumen[IndiceVolumen];

            logicaPeces.Update(gameTime);
            if (barraPesca.EstaActiva())
            {
                barraPesca.Actualizar(gameTime);
                if (barraPesca.PescaCompletada && !esperandoDespuesCaptura)
                {
                    logicaPeces.MostrarPez();
                    score += logicaPeces.CurrentScore;
                    sonidoCaptura.Play(); // sonido al capturar
                    esperandoDespuesCaptura = true;
                    tiempoEsperaCaptura = 0f;
                }
            }
            else if (esperandoDespuesCaptura)
            {
                tiempoEsperaCaptura += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (tiempoEsperaCaptura >= DELAY_CAPTURA)
                {
                    pescador.Resetear();
                    anzuelo.Resetear();
                    esperandoDespuesCaptura = false;
                }
            }
            else
            {
                pescador.Update(gameTime);
                anzuelo.Update(gameTime);

                if (!barraPesca.EstaActiva() && anzuelo.Position.Y >= NIVEL_AGUA)
                {
                    barraPesca.Activar();
                    pescador.EmpezarEsfuerzo();
                }

                if (anzuelo.Estado == EstadoAnzuelo.Listo && click)
                {
                    pescador.Lanzar();
                    logicaPeces.EsconderPez();
                    sonidoTiro.Play(1.0f, 1.0f, 0.0f); // sonido al tirar la caña con pitch aún más alto
                }

                if (anzuelo.Estado == EstadoAnzuelo.Listo && pescador.AnimandoTiro && pescador.FrameActual >= 3)
                {
                    anzuelo.Lanzar(pescador.Posicion + new Vector2(17, 13), new Vector2(-500f, 0f));
                    pescador.AnimandoTiro = false;
                }
            }
        }

        private void ActualizarMenuPausa(MouseState mouse, bool click)
        {
            if (click)
            {
                if (btnReanudar.Contains(mouse.Position)) estadoActual = GameState.Jugando;
                
                if (btnVolumen.Contains(mouse.Position))
                {
                    IndiceVolumen = (IndiceVolumen + 1) % NivelesVolumen.Length;
                    MediaPlayer.Volume = NivelesVolumen[IndiceVolumen];
                }

                if (btnMenuPrincipal.Contains(mouse.Position)) { Process.Start(Process.GetCurrentProcess().MainModule.FileName); Exit(); }
                if (btnSalir.Contains(mouse.Position)) Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(fondo, new Rectangle(0, 0, 800, 600), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            
            pescador.Draw(_spriteBatch);
            anzuelo.Draw(_spriteBatch, gameTime);
            barraPesca.Dibujar(_spriteBatch);
            logicaPeces.Dibujar(_spriteBatch);

            string scoreText = "Score: " + score;
            Vector2 scorePos = new Vector2(800 - fuente.MeasureString(scoreText).X - 10, 10);
            _spriteBatch.DrawString(fuente, scoreText, scorePos, Color.White);

            if (estadoActual == GameState.Pausado)
            {
                _spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, 800, 600), Color.Black * 0.5f);
                MouseState ms = Mouse.GetState();
                DibujarBotonEstetico(btnReanudar, "REANUDAR", ms);
                DibujarBotonEstetico(btnVolumen, "SONIDO: " + NombresVolumen[IndiceVolumen], ms);
                DibujarBotonEstetico(btnMenuPrincipal, "MENU PRINCIPAL", ms);
                DibujarBotonEstetico(btnSalir, "SALIR", ms);
            }
            _spriteBatch.End();
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
    }
}
