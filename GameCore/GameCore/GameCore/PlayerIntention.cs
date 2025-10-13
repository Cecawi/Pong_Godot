namespace GameCore;

public class PlayerIntention
{
    //attributs
    public const int Up = -1;
    public const int Neutral = 0;
    public const int Down = 1;
    public int Move {get;}// -1 = haut, 0 = neutre, 1 = bas : intention du joueur (lors de l'update)
    
    public PlayerIntention(int m)
    {
        if (m < Up || m > Down)
        {
            throw new ArgumentOutOfRangeException(nameof(m), "La valeur doit être -1, 0 ou 1");
        }
        Move = m;
    }
}