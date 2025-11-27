using Fishing_Fever;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        using (var intro = new Intro())
        {
            intro.Run();
        }

        // Una vez cerrada la intro, arrancamos el juego principal
        using (var juego = new Game1())
        {
            juego.Run();
        }
    }
}
