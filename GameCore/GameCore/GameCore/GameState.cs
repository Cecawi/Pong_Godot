namespace GameCore;

public class GameState
{
    //attributs
    private readonly Ball _b;
    private readonly Paddle _p1;
    private readonly Paddle _p2;
    private Score _score;
    private bool _isGameOver;

    public GameState()
    {
        //_b = new Ball();/////PARAMETRES (à voir)
        _b = new Ball(400, 300, 3, 5, 5, 800, 600);
        _p1 = new Paddle(100, 250, 3, 100, 5, 600);
        _p2 = new Paddle(700, 250, 3, 100, 5, 600);
        _score = new Score();
        _isGameOver = false;
    }

    public void Update(float deltaTime, PlayerIntention p1Intention, PlayerIntention p2Intention)
    {
        if (_isGameOver)
        {
            return;// on ne met plus à jour si le jeu est terminé
        }

        //déplacement des raquettes selon l'intention
        _p1.Move(p1Intention.Move, deltaTime);
        _p2.Move(p2Intention.Move, deltaTime);

        //mise à jour de la balle
        _b.Update(deltaTime);

        //collision balle/raquette gauche
        _b.BounceFromPaddle(_p1.X + _p1.Width, _p1.Y, _p1.Height, isLeftPaddle: true);

        //collision balle/raquette droite
        _b.BounceFromPaddle(_p2.X, _p2.Y, _p2.Height, isLeftPaddle: false);

        //gestion des points
        if (_b.IsOutLeft())
        {
            _score.AddPoint(1); //point pour joueur 2
            ResetBall(directionToRight: true);
        }
        else if (_b.IsOutRight())
        {
            _score.AddPoint(0); //point pour joueur 1
            ResetBall(directionToRight: false);
        }

        //vérifie si la partie est terminée
        if (_score.IsGameOver())
        {
            _isGameOver = true;
        }
    }
    
    //réinitialise la balle après un point
    private void ResetBall(bool directionToRight)
    {
        float vx = directionToRight ? 5 : -5;
        _b.Reset(400, 300, vx, 5);
    }

    public (float x, float y) GetBallPosition()//(float x, float y) : vecteur, position x, y
    {
        return _b.GetPosition();
    }

    public (float y1, float y2) GetPaddlePositions()
    {
        return (_p1.Y, _p2.Y);
    }
    
    public (int p1, int p2) GetScore()
    {
        return _score.GetScore();
    }
    
    public bool IsGameOver()
    {
        return _isGameOver;
    }
}