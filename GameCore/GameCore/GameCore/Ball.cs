namespace GameCore;

//objectif : représenter la physique et les règles de déplacement de la balle
//sans dépendre d’aucun moteur graphique
public class Ball
{
    
    //calculer le point de collision et déduire l’angle de rebond
    //classe collisions/contacts???
    //Vitesse/angle balle (peut être encapsulé dans la position directement)
    
    //attributs
    public float X { get; private set; }/// <summary>
                                        /// // DEMANDER A CHATGPT SI FAUT METTRE EN PRIVE OU PUBLIQUE
                                        /// </summary>
    public float Y { get; private set; }
    public float VelocityX { get; private set; }
    public float VelocityY { get; private set; }
    //"taille" du terrain : 
    private readonly float fieldWidth;//x (gauche : 0/droite : fieldWidth)
    private readonly float fieldHeight;//y (haut : 0/bas : fieldHeight)

    //constructeur
    public Ball(float startX, float startY, float velocityX, float velocityY, float fieldWidth, float fieldHeight)
    {
        X = startX;
        Y = startY;
        VelocityX = velocityX;
        VelocityY = velocityY;
        this.fieldWidth = fieldWidth;
        this.fieldHeight = fieldHeight;
    }

    //méthodes
    public void Update(float deltaTime)//à chaque màj, on récupère deltaTime, le temps écoulé par rapport à la màj précédente
    {
        X += VelocityX * deltaTime;//deltaTime rend le mouvement indépendant du framerate
        Y += VelocityY * deltaTime;

        if(Y <= 0.1 || Y >= fieldHeight-0.1)//+/- 0.1 pour éviter que la balle "colle" le bord
        {
            VelocityY = -VelocityY;
            ClampY();
        }
    }

    public void Reset(float startX, float startY, float velocityX, float velocityY)
    {
        X = startX;
        Y = startY;
        VelocityX = velocityX;
        VelocityY = velocityY;
    }
    
    /*public void BounceFromPaddle(float paddleX, float paddleY, float paddleHeight, bool isLeftPaddle)
    {
        if (Y >= paddleY && Y <= paddleY + paddleHeight)
        {
            VelocityX = -VelocityX;

            // Modifie légèrement la direction verticale selon le point d'impact
            float hitPos = (Y - (paddleY + paddleHeight / 2)) / (paddleHeight / 2);
            VelocityY = hitPos * Math.Abs(VelocityX);

            // Décale légèrement la balle pour éviter qu'elle reste coincée
            X = isLeftPaddle ? paddleX + 1 : paddleX - 1;
        }
    }

    private void ClampY()
    {
        if (Y < 0) Y = 0;
        if (Y > fieldHeight) Y = fieldHeight;
    }

    public bool IsOutLeft() => X < 0;
    public bool IsOutRight() => X > fieldWidth;*/
    
    public void BounceFromPaddle(float paddleX, float paddleY, float paddleHeight, bool isLeftPaddle);//va faire "rebondir la balle d'une certaine manière par rapport à l'angle d'impacte
    private void ClampY();//si la balle touche le bord haut ou bas : balle renvoyée vers terrain
    public bool IsOutLeft();//but : point pour joueur à droite
    public bool IsOutRight();//but : point pour joueur à gauche
    
    public (float x, float y) GetPosition()
    {
        return (X, Y);
    }
    
}









// ici bas une conception du code















//         
//
//         
//
//         public void BounceFromPaddle(float paddleX, float paddleY, float paddleHeight, bool isLeftPaddle)
//         {
//             if (isLeftPaddle)
//             {
//                 if (X <= paddleX && Y >= paddleY && Y <= paddleY + paddleHeight)
//                 {
//                     VelocityX = -VelocityX;
//                     X = paddleX; 
//                 }
//             }
//             else
//             {
//                 if (X >= paddleX && Y >= paddleY && Y <= paddleY + paddleHeight)
//                 {
//                     VelocityX = -VelocityX;
//                     X = paddleX; 
//                 }
//             }
//         }
//
//         private void ClampY()
//         {
//             if (Y < 0) Y = 0;
//             if (Y > fieldHeight) Y = fieldHeight;
//         }
//
//         public bool IsOutLeft() => X < 0;
//         public bool IsOutRight() => X > fieldWidth;
//     }
// }

