namespace GameCore;

public class GameState
{
    private readonly Ball _b;
    private readonly Paddle _p1;
    private readonly Paddle _p2;
    private Score _s;
    private bool _status;//IsGameOver,running, ended...

    public GameState()
    {
        _b = new Ball();
        _p1 = new Paddle(1);
        _p2 = new Paddle(2);
        _s = new Score();
    }

    public void Update(float deltaTime, PlayerIntention p1Intention, PlayerIntention p2Intention)
    {
        //à def
    }

    public (float x, float y) GetBallPosition()//(float x, float y) : vecteur, position x, y
    {
        return _b.Position;
    }

    public (float y1, float y2) GetPaddlePositions()
    {
        return (_p1.Position, _p2.Position);
    }
    public (int p1, int p2) GetScore()
    {
        return _score.Values;
    }
}