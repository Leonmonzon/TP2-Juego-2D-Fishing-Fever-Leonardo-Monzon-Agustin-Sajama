using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Fishing_Fever
{
    public class Intro : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Texture2D portada;
        Texture2D pixel; 
        SpriteFont fuente;

        Song musicaIntro;
        bool musicaIniciada;

        // Botones
        Rectangle btnJugar;
        Rectangle btnConfig;
        Rectangle btnSalir;

        MouseState mouseAnterior;

        public Intro()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
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

            int btnW = 280;
            int btnH = 55;
            int centroX = _graphics.PreferredBackBufferWidth / 2 - (btnW / 2);
            
            // --- CAMBIO AQUÍ: Bajamos el inicio de los botones ---
            int inicioY = 380; // Antes estaba en 300-320. Súbelo o bájalo según tu portada.

            btnJugar  = new Rectangle(centroX, inicioY, btnW, btnH);
            btnConfig = new Rectangle(centroX, inicioY + 70, btnW, btnH);
            btnSalir  = new Rectangle(centroX, inicioY + 140, btnW, btnH);
        }

        protected override void Update(GameTime gameTime)
        {
            if (!musicaIniciada)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(musicaIntro);
                musicaIniciada = true;
            }

            var mouse = Mouse.GetState();
            bool click = mouse.LeftButton == ButtonState.Pressed &&
                         mouseAnterior.LeftButton == ButtonState.Released;

            if (click)
            {
                if (btnJugar.Contains(mouse.Position))
                {
                    MediaPlayer.Stop();
                    using (Game1 juego = new Game1())
                    {
                        juego.Run();
                    }
                    Exit();
                }
                if (btnSalir.Contains(mouse.Position)) Exit();
            }

            mouseAnterior = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            _spriteBatch.Draw(portada, new Rectangle(0, 0, 800, 600), Color.White);

            MouseState ms = Mouse.GetState();

            // Dibujar botones con estética unificada
            DibujarBotonEstetico(btnJugar, "JUGAR", ms);
            DibujarBotonEstetico(btnConfig, "CONFIGURACION", ms);
            DibujarBotonEstetico(btnSalir, "SALIR", ms);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DibujarBotonEstetico(Rectangle rect, string texto, MouseState ms)
        {
            bool estaEncima = rect.Contains(ms.Position);
            
            // Lógica de escala (si está encima, crece un 5%)
            float escala = estaEncima ? 1.05f : 1.0f;
            
            Color colorBoton = estaEncima ? Color.DarkSlateBlue : Color.FromNonPremultiplied(40, 40, 40, 220);
            Color colorBorde = estaEncima ? Color.Gold : Color.White * 0.4f;

            // Calculamos el origen para que el botón crezca desde el centro y no desde la esquina
            Vector2 centroBoton = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            Vector2 tamañoBoton = new Vector2(rect.Width, rect.Height);

            // 1. Borde (un poco más grande que el botón)
            _spriteBatch.Draw(pixel, centroBoton, null, colorBorde, 0f, new Vector2(0.5f, 0.5f), tamañoBoton * escala + new Vector2(4, 4), SpriteEffects.None, 0f);
            
            // 2. Fondo
            _spriteBatch.Draw(pixel, centroBoton, null, colorBoton, 0f, new Vector2(0.5f, 0.5f), tamañoBoton * escala, SpriteEffects.None, 0f);

            // 3. Texto
            Vector2 textSize = fuente.MeasureString(texto);
            _spriteBatch.DrawString(fuente, texto, centroBoton + new Vector2(2, 2), Color.Black * 0.5f, 0f, textSize / 2, escala, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(fuente, texto, centroBoton, estaEncima ? Color.Gold : Color.White, 0f, textSize / 2, escala, SpriteEffects.None, 0f);
        }
    }
}