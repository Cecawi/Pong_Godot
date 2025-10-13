namespace GameCore;

public class GameState
{
    private readonly Ball _b;
    private readonly Paddle _p1;
    private readonly Paddle _p2;
    private Score _score;
    private bool _status;//IsGameOver,running, ended...

    public GameState()
    {
        //_b = new Ball();/////PARAMETRES (à voir)
        _b = new Ball(400, 300, 3, 5, 5, 800, 600);
        _p1 = new Paddle(100, 250, 3, 100, 5, 600);
        _p2 = new Paddle(700, 250, 3, 100, 5, 600);
        _score = new Score();
    }

    public void Update(float deltaTime, PlayerIntention p1Intention, PlayerIntention p2Intention)
    {
        //à def
    }

    public (float x, float y) GetBallPosition()//(float x, float y) : vecteur, position x, y
    {
        return _b.GetPosition();
    }

    public (float y1, float y2) GetPaddlePositions()
    {
        if (_p1 != null && _p2 != null)
        {
            return (_p1.Y, _p2.Y);
        }
        // Valeur par défaut si jamais l’un des paddles n’existe pas
        return (0f, 0f);
    }
    public (int p1, int p2) GetScore()
    {
        return _score.GetScore();
    }
}