using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Fishing_Fever
{
    // Usamos el mismo Enum para consistencia
    public enum EstadoMenu { Principal, Configuracion }

    public class Intro : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D portada;
        private Texture2D pixel; 
        private SpriteFont fuente;

        private Song musicaIntro;
        private bool musicaIniciada;

        private EstadoMenu estadoActual = EstadoMenu.Principal;

        // Botones
        private Rectangle btnJugar, btnConfig, btnSalir, btnVolumen, btnAtras;

        private MouseState mouseAnterior;

        public Intro()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Resolución estándar
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            portada = Content.Load<Texture2D>("Images/portada");
            musicaIntro = Content.Load<Song>("Audio/musicaIntro");
            fuente = Content.Load<SpriteFont>("Fonts/fuente"); 

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            ConfigurarBotones();
        }

        private void ConfigurarBotones()
        {
            // Tamaño de botones pequeños y posición baja para no tapar el título
            int btnW = 220; 
            int btnH = 45;  
            int centroX = _graphics.PreferredBackBufferWidth / 2 - (btnW / 2);
            int inicioY = 420; 

            // Rectángulos del Menú Principal
            btnJugar  = new Rectangle(centroX, inicioY, btnW, btnH);
            btnConfig = new Rectangle(centroX, inicioY + 55, btnW, btnH);
            btnSalir  = new Rectangle(centroX, inicioY + 110, btnW, btnH);

            // Rectángulos de Configuración (reutilizamos posiciones para limpieza visual)
            btnVolumen = new Rectangle(centroX, inicioY, btnW, btnH);
            btnAtras   = new Rectangle(centroX, inicioY + 55, btnW, btnH);
        }

        protected override void Update(GameTime gameTime)
        {
            // Iniciar música con el volumen guardado en Game1
            if (!musicaIniciada)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = Game1.NivelesVolumen[Game1.IndiceVolumen];
                MediaPlayer.Play(musicaIntro);
                musicaIniciada = true;
            }

            var mouse = Mouse.GetState();
            bool click = mouse.LeftButton == ButtonState.Pressed && mouseAnterior.LeftButton == ButtonState.Released;

            if (click)
            {
                if (estadoActual == EstadoMenu.Principal)
                {
                    if (btnJugar.Contains(mouse.Position))
                    {
                        MediaPlayer.Stop();
                        // Al lanzar Game1, este tomará el IndiceVolumen que hayamos elegido aquí
                        using (Game1 juego = new Game1()) { juego.Run(); }
                        Exit();
                    }
                    if (btnConfig.Contains(mouse.Position))
                    {
                        estadoActual = EstadoMenu.Configuracion;
                    }
                    if (btnSalir.Contains(mouse.Position)) Exit();
                }
                else if (estadoActual == EstadoMenu.Configuracion)
                {
                    if (btnVolumen.Contains(mouse.Position))
                    {
                        // Ciclo de volumen compartido con Game1
                        Game1.IndiceVolumen = (Game1.IndiceVolumen + 1) % Game1.NivelesVolumen.Length;
                        MediaPlayer.Volume = Game1.NivelesVolumen[Game1.IndiceVolumen];
                    }
                    if (btnAtras.Contains(mouse.Position))
                    {
                        estadoActual = EstadoMenu.Principal;
                    }
                }
            }

            mouseAnterior = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            // Dibujar la imagen de portada
            _spriteBatch.Draw(portada, new Rectangle(0, 0, 800, 600), Color.White);

            MouseState ms = Mouse.GetState();

            if (estadoActual == EstadoMenu.Principal)
            {
                DibujarBotonEstetico(btnJugar, "JUGAR", ms);
                DibujarBotonEstetico(btnConfig, "OPCIONES", ms);
                DibujarBotonEstetico(btnSalir, "SALIR", ms);
            }
            else
            {
                // Mostramos el nombre del volumen actual (SILENCIO, BAJO, etc.)
                string textoVolumen = "SONIDO: " + Game1.NombresVolumen[Game1.IndiceVolumen];
                DibujarBotonEstetico(btnVolumen, textoVolumen, ms);
                DibujarBotonEstetico(btnAtras, "VOLVER", ms);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DibujarBotonEstetico(Rectangle rect, string texto, MouseState ms)
        {
            bool estaEncima = rect.Contains(ms.Position);
            float escala = estaEncima ? 1.03f : 1.0f;
            
            // Colores unificados con el menú de pausa
            Color colorBoton = estaEncima ? Color.FromNonPremultiplied(50, 50, 80, 230) : Color.FromNonPremultiplied(30, 30, 30, 200);
            Color colorBorde = estaEncima ? Color.Gold : Color.White * 0.3f;

            Vector2 centroBoton = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            Vector2 tamañoBoton = new Vector2(rect.Width, rect.Height);

            // Dibujar Borde
            _spriteBatch.Draw(pixel, centroBoton, null, colorBorde, 0f, new Vector2(0.5f, 0.5f), tamañoBoton * escala + new Vector2(3, 3), SpriteEffects.None, 0f);
            
            // Dibujar Fondo
            _spriteBatch.Draw(pixel, centroBoton, null, colorBoton, 0f, new Vector2(0.5f, 0.5f), tamañoBoton * escala, SpriteEffects.None, 0f);

            // Dibujar Texto centrado con sombra
            Vector2 textSize = fuente.MeasureString(texto);
            float escalaTexto = escala * 0.85f; 

            _spriteBatch.DrawString(fuente, texto, centroBoton + new Vector2(1, 1), Color.Black * 0.5f, 0f, textSize / 2, escalaTexto, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(fuente, texto, centroBoton, estaEncima ? Color.Gold : Color.White, 0f, textSize / 2, escalaTexto, SpriteEffects.None, 0f);
        }
    }
}