namespace GameCore;

public class Score
{
    private int _paddle1Score = 0;////constructeur?
    private int _paddle2Score = 0;
    private int _paddle1Id = 0;
    private int _paddle2Id = 1;

    public int GetPaddle1Score()
    {
        return _paddle1Score;
    }

    public int GetPaddle2Score()
    {
        return _paddle2Score;
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

    public bool IsGameOver()
    {
        if(_paddle1Score == 11 || _paddle2Score == 11)/////ou >= si points en plus par erreur ou via debug
        {
            return true;
        }
        return false;
    }

    public int GetWinner()
    {
        return _paddle1Score > _paddle2Score ? _paddle1Id : _paddle2Id;
    }
}