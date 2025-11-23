namespace GameCore;

public interface IGameState
{
    
        float BallX { get; }
        float BallY { get; }
        float BallVX { get; }
        float BallVY { get; }
        float PlayerY { get; }
        float EnemyY { get; }
    
}