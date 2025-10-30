namespace GameCore;

public class Score
{
    // attributs
    private int _paddle1Score = 0;
    private int _paddle2Score = 0;
    private  int _paddle1Id = 0;
    private  int _paddle2Id = 1;
    public bool GoalSoundFlag;
    
    // les getters et setters
    public int GetPaddle1Score()
    {
        return _paddle1Score;
    }

    public int GetPaddle2Score()
    {
        return _paddle2Score;
    }

    public (int sP1, int sP2) GetScore()
    {
        return (_paddle1Score, _paddle2Score);
    }

    public void AddPoint(int paddleId)
    {
        if(paddleId == _paddle1Id)
        {
            _paddle1Score++;
        }
        if(paddleId == _paddle2Id)
        {
            _paddle2Score++;
        }
    }
    
    public bool IsGameOver() => _paddle1Score >= 11 || _paddle2Score >= 11;//>= si ajout par erreur ou via debug

    public int GetWinner()
    {
        return _paddle1Score > _paddle2Score ? _paddle1Id : _paddle2Id;//si le score du joueur 1 > au score du joueur 2, en renvoie l'identifiant du joueur 1, sinon, on renvoie celui du joueur 2
    }
    
    public void Reset()
    {
        _paddle1Score = 0;
        _paddle2Score = 0;
    }
}