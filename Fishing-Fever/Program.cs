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
    }
}