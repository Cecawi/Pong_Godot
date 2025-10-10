namespace GameCore;

public class PlayerIntention
{
    //attribut
    public int Move {get;}// -1 = haut, 0 = neutre, 1 = bas : intention du joueur (lors de l'update)
    
    

    public PlayerIntention(int m)
    {
        if (m < -1 || m > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(m), "La valeur doit être -1, 0 ou 1");
        }
        Move = m;
    }
    
    /*
     normalement, on en n'a pas besoin
     A ENLEVER
    // Méthode statique pour créer l'intention à partir d'une touche_clavier Godot
    public static PlayerIntention FromInput(bool upPressed, bool downPressed)
    {
        int move = 0;
        if (upPressed) move = -1;
        else if (downPressed) move = 1;

        return new PlayerIntention(move);
    }*/
}