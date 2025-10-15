namespace GameCore;

public class GameState
{
    //attributs
    private readonly Ball _b;
    private readonly Paddle _p1;
    private readonly Paddle _p2;
    private readonly Score _score;
    private bool _isGameOver;

    // monde normalisé (-0.5 à +0.5)
    private readonly float _fieldWidth = 1.0f;
    private readonly float _fieldHeight = 1.0f;
    private readonly float _margeGaucheDroite = 0.4f; // en unité normalisée

    public GameState()
    {
        // Générer le sens initial de la balle aléatoire (-1 ou +1)
        int direction = new Random().Next(0, 2) == 0 ? -1 : 1;
        
        // on considère que le centre est (0,0), donc la balle démarre au milieu
        // les tailles et vitesses sont exprimées dans le monde normalisé
        _b = new Ball(0f, 0f, 0.015f, 0.2f * direction, 0.2f, _fieldWidth, _fieldHeight);

        // paddles centrés verticalement
        _p1 = new Paddle(-_margeGaucheDroite, 0f, 0.02f, 0.15f, 0.5f, _fieldHeight);
        _p2 = new Paddle(_margeGaucheDroite, 0f, 0.02f, 0.15f, 0.5f, _fieldHeight);

        _score = new Score();
        _isGameOver = false;
    }

    public void Update(float deltaTime, PlayerIntention p1Intention, PlayerIntention p2Intention)
    {
        if (_isGameOver)
        {
            return; // on ne met plus à jour si le jeu est terminé
        }

        // déplacement des raquettes selon l'intention
        _p1.Move(p1Intention.Move, deltaTime);
        _p2.Move(p2Intention.Move, deltaTime);

        // mise à jour de la balle
        _b.Update(deltaTime);
    
        // collision balle/raquette gauche
        _b.BounceFromPaddle(_p1.X, _p1.Y, _p1.Width, _p1.Height, isLeftPaddle : true);

        // collision balle/raquette droite
        _b.BounceFromPaddle(_p2.X, _p2.Y, _p2.Width, _p2.Height, isLeftPaddle : false);

        // gestion des points
        if (_b.IsOutLeft())
        {
            _score.AddPoint(1); // point pour joueur 2
            ResetBall(directionToRight: true);///////////////direction vers le gagnant ? perdant ? aléatoire ?
        }
        else if (_b.IsOutRight())
        {
            _score.AddPoint(0); // point pour joueur 1
            ResetBall(directionToRight: false);///////////////direction vers le gagnant ? perdant ? aléatoire ?
        }

        // vérifie si la partie est terminée
        if (_score.IsGameOver())
        {
            _isGameOver = true;
        }
    }
    
    // réinitialise la balle après un point
    private void ResetBall(bool directionToRight)
    {
        int randomDir = new Random().Next(0, 2) == 0 ? -1 : 1;
        float vx = directionToRight ? 0.2f : -0.2f;
        vx *= randomDir;
        float vy = (float)(new Random().NextDouble() * 0.4f - 0.2f); // -0.2 à +0.2
        _b.Reset(0f, 0f, vx, vy);
    }

    // accesseurs pour l'affichage
    public (float x, float y) GetBallPosition() // (x, y) : vecteur, position x, y
    {
        return _b.GetPosition();
    }
    
    public float GetBallRadius() => _b.Radius;

    public (float y1, float y2) GetPaddlesYPosition()///////////////
    {
        return (_p1.Y, _p2.Y);
    }
    
    public (float x1, float y1) GetPaddle1Position()
    {
        return (_p1.X, _p1.Y);
    }
    
    public (float x1, float y1) GetPaddle2Position()
    {
        return (_p2.X, _p2.Y);
    }

    public (float width, float height) GetPaddle1Size()
    {
        return (_p1.Width, _p1.Height);
    }
    
    public (float width, float height) GetPaddle2Size()
    {
        return (_p2.Width, _p2.Height);
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