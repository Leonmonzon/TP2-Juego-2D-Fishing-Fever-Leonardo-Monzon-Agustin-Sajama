using System;
using Fishing_Fever; // <-- Debe coincidir con el namespace de Game1 e Intro

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // 1. Ejecutar la introducción
        using (var intro = new Intro())
        {
            intro.Run();
        }

        // 2. Una vez que la intro se cierra (llama a Exit()), arrancamos el juego principal
        using (var juego = new Game1())
        {
            juego.Run();
        }
    }
}