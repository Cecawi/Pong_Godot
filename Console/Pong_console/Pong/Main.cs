using System;
using GameCore;

namespace Pong;

public class MainConsole
{
    private static GameState _gameState;
    
    //monde dans GameCore :
    //en haut à gauche : (-0.5, -0.5)
    //en bas à droite : (+0.5, +0.5)
    //je veux afficher sur la console un "monde" 100 * 20
    private const int Width = 100;
    private const int Height = 20;
    
    public static void Main()
    {
        Console.Clear();
        
        _gameState = new GameState();

        UpdateVisuals();
    }

    private static void UpdateVisuals()
    {
        //crée une grille vide remplie d'espaces
        char[,] grid = new char[Height + 1, Width + 1];
        
        for(int i = 0 ; i <= Height ; i++)
        {
            for (int j = 0; j <= Width; j++)
            {
                grid[i, j] = ' ';
            }
        }

        //trace le cadre
        for (int i = 0 ; i <= Width ; i++)
        {
            grid[0, i] = '-';
            grid[Height, i] = '-';
        }
        for (int i = 0 ; i <= Height ; i++)
        {
            grid[i, 0] = '|';
            grid[i, Width] = '|';
        }
        
        //affichage dans la console
        //////////////Console.SetCursorPosition(0, 0);
        for(int i = 0 ; i <= Height ; i++)
        {
            for(int j = 0 ; j <= Width; j++)
            {
                Console.Write(grid[i, j]);
            }
            Console.WriteLine();
        }
    }
}