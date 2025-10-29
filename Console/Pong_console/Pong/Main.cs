using System;
using GameCore;

namespace Pong;

public class MainConsole
{
    private static GameState _gameState;

    private const int Up = PlayerIntention.Up;
    private const int Neutral = PlayerIntention.Neutral;
    private const int Down = PlayerIntention.Down;
    
    private float ballX, ballY, paddle1X, paddle1Y, paddle2X, paddle2Y;

    private int gx, gy, p1gx, p1gy, p2gx, p2gy;
    
    //monde dans GameCore :
    //en haut à gauche : (-0.5, -0.5)
    //en bas à droite : (+0.5, +0.5)
    //je veux afficher sur la console un "monde" 100 * 20
    private const int Width = 100;
    private const int Height = 20;
    
    //vitesse d'affichage (ms)
    private const int FrameDelay = 100;
    
    public static void Main()
    {
        Console.Clear();
        
        _gameState = new GameState();
        
        while(true)
        {
            _gameState.Update(0.1f, new PlayerIntention(0), GetPlayer2Intention());
            UpdateVisuals();
        }
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
        
        //récupère les positions depuis GameCore
        var (ballX, ballY) = _gameState.GetBallPosition();
        var (paddle1X, paddle1Y) = _gameState.GetPaddle1Position();
        var (paddle2X, paddle2Y) = _gameState.GetPaddle2Position();
        
        //convertit les positions dans monde (GameCore) à la case correspondante dans le tableau (qu'on affiche)
        var (gx, gy) = ToGrid(ballX, ballY);
        var (p1gx, p1gy) = ToGrid(paddle1X, paddle1Y);
        var (p2gx, p2gy) = ToGrid(paddle2X, paddle2Y);
        
        //place les objets dans le tableau
        if (gy >= 0 && gy <= Height && gx >= 0 && gx <= Width) grid[gy, gx] = 'o';//balle
        if (p1gy >= 0 && p1gy <= Height && p1gx >= 0 && p1gx <= Width) grid[p1gy, p1gx] = '1';//paddle gauche
        if (p2gy >= 0 && p2gy <= Height && p2gx >= 0 && p2gx <= Width) grid[p2gy, p2gx] = '1';//paddle droit
        
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
///////////////////////////////////Console.SetCursorPosition(0, 0);
        for(int i = 0 ; i <= Height ; i++)
        {
            for(int j = 0 ; j <= Width; j++)
            {
                Console.Write(grid[i, j]);
            }
            Console.WriteLine();
        }
    }
    
    private static (int x, int y) ToGrid(float normX, float normY)//même idée que ToPixels dans le Main.cs dans Godot
    {
        //(-0.5, -0.5) → (0,0)
        //(+0.5, +0.5) → (Width, Height)
        int x = (int)((normX + 0.5f) * Width);
        int y = (int)((normY + 0.5f) * Height);
        return (x, y);
    }

    private static PlayerIntention GetPlayer2Intention()
    {
        int move = PlayerIntention.Neutral;
        
        var key = Console.ReadKey(true).Key;

        if(key == ConsoleKey.UpArrow)
        {
            move = PlayerIntention.Up;
        }
        
        else if(key == ConsoleKey.DownArrow)
        {
            move = PlayerIntention.Down;
        }

        return new PlayerIntention(move);
    }
}