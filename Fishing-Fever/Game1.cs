using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fishing_Fever;

public class Game1 : Game
{
    Texture2D fondo;


    Texture2D basePescador;
    Vector2 posPescador;





    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        //cargo el fondo
        fondo = Content.Load<Texture2D>("Images/fondo");
        //fin carga fondo


        //cargo la base del pescador
        basePescador = Content.Load<Texture2D>("Images/basePescador");


        posPescador = new Vector2(
    (_graphics.PreferredBackBufferWidth / 2) + 75 - basePescador.Width / 2,
    (_graphics.PreferredBackBufferHeight / 2) + 26 - basePescador.Height / 2
);



        //fin carga pescador
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        //pongo el fondo
        _spriteBatch.Begin();
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
        //fin fondo


        //dibujo la base del pescador
        _spriteBatch.Draw(
    basePescador,
    posPescador, 
    null, 
    Color.White,
    0f,
    Vector2.Zero,
    3f,
    SpriteEffects.None,
    0f
);
        //fin dibujo pescador

        _spriteBatch.End();
        base.Draw(gameTime);
    }


}
