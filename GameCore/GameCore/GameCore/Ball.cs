namespace GameCore;

//objectif : représenter la physique et les règles de déplacement de la balle
//sans dépendre d’aucun moteur graphique
public class Ball
{
    
    //attributs
    private float X { get; set; }
    private float Y { get; set; }
    private float Radius { get; }///  ///////////////voir si on garde le rayon de la balle, normalement oui
    private float VelocityX { get; set; }

    private float VelocityY { get; set; }

    //"taille" du terrain : 
    private readonly float _fieldWidth;  //x (gauche : -fieldWidth/2 / droite : +fieldWidth/2)
    private readonly float _fieldHeight; //y (bas : -fieldHeight/2 / haut : +fieldHeight/2)

    //readonly : on doit lui assigner une valeur dans le constructeur
    
    //constructeur
    public Ball(float startX, float startY, float setOnceRadius, float velocityX, float velocityY, float fieldWidth, float fieldHeight)
    {
        X = startX;
        Y = startY;
        Radius = setOnceRadius;
        VelocityX = velocityX;
        VelocityY = velocityY;
        _fieldWidth = fieldWidth;
        _fieldHeight = fieldHeight;
    }
    
    public (float x, float y) GetPosition()
    {
        return (X, Y);
    }

    //méthodes
    public void Update(float deltaTime)//à chaque màj, on récupère deltaTime, le temps écoulé par rapport à la màj précédente
    {
        X += VelocityX * deltaTime;//deltaTime rend le mouvement indépendant du framerate
        Y += VelocityY * deltaTime;

        //rebond haut/bas
        if (Y + Radius >= _fieldHeight / 2f || Y - Radius <= -_fieldHeight / 2f)
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
    
    public void BounceFromPaddle(float paddleX, float paddleY, float paddleHeight, bool isLeftPaddle)
    {
        bool verticallyAligned = Y + Radius >= paddleY - paddleHeight / 2f && Y - Radius <= paddleY + paddleHeight / 2f;

        if (isLeftPaddle)
        {
            if (verticallyAligned && X - Radius <= paddleX + 0.01f) // marge min
            {
                //rebond sur la raquette gauche
                VelocityX = Math.Abs(VelocityX);
                X = paddleX + Radius;

                //modifie légèrement la direction verticale selon le point d'impact
                float hitPos = (Y - paddleY) / (paddleHeight / 2f);
                VelocityY = hitPos * Math.Abs(VelocityX);
            }
        }
        else
        {
            if (verticallyAligned && X + Radius >= paddleX - 0.01f)
            {
                //rebond sur la raquette droite
                VelocityX = -Math.Abs(VelocityX);
                X = paddleX - Radius;

                //modifie légèrement la direction verticale selon le point d'impact
                float hitPos = (Y - paddleY) / (paddleHeight / 2f);
                VelocityY = hitPos * Math.Abs(VelocityX);
            }
        }
    }
    
    private void ClampY()//replace la balle à l'intérieur du terrain après un rebond
    {
        if (Y - Radius < -_fieldHeight / 2f)
        {
            Y = -_fieldHeight / 2f + Radius;
        }

        if (Y + Radius > _fieldHeight / 2f)
        {
            Y = _fieldHeight / 2f - Radius;
        }
    }

    public bool IsOutLeft() => X + Radius < -_fieldWidth / 2f;   //but : point pour joueur à droite
   
    public bool IsOutRight() => X - Radius > _fieldWidth / 2f;   //but : point pour joueur à gauche
}