namespace GameCore;

public class PlayerIntention
{
    //attribut
    public int Move {get;}// -1 = haut, 0 = neutre, 1 = bas : intention du joueur (lors de l'update)

    public PlayerIntention(int m)
    {
        //à remplir
    }
    /*
    // Constructeur : on passe directement l'intention du joueur
    public PlayerIntention(int move)
    {
        if (move < -1 || move > 1)
            throw new ArgumentOutOfRangeException(nameof(move), "Move doit être -1, 0 ou 1");
        
        Move = move;
    }

    // Méthode statique pour créer l'intention à partir d'une touche Godot
    public static PlayerIntention FromInput(bool upPressed, bool downPressed)
    {
        int move = 0;
        if (upPressed) move = -1;
        else if (downPressed) move = 1;

        return new PlayerIntention(move);
    }
     */
}