namespace GameCore;

//objectif : représenter la physique et les règles de déplacement de la balle
//sans dépendre d’aucun moteur graphique
public class Ball
{
    
    //attributs
    private float X { get; set; }
    private float Y { get; set; }
    internal float Radius { get; }///  ///////////////voir si on garde le rayon de la balle, normalement oui
    private float VelocityX { get; set; }

    private float VelocityY { get; set; }

    //"taille" du terrain : 
    private readonly float _fieldWidth;  //x (gauche : -fieldWidth/2 / droite : +fieldWidth/2)
    private readonly float _fieldHeight; //y (bas : -fieldHeight/2 / haut : +fieldHeight/2)
    
    private static readonly Random Rand = new Random();

    //readonly : on doit lui assigner une valeur dans le constructeur
    
    //constructeur
    public Ball(float startX, float startY, float setOnceRadius, float velocityX, float velocityY, float fieldWidth, float fieldHeight)
    {
        X = startX;
        Y = startY;
        Radius = setOnceRadius;
        _fieldWidth = fieldWidth;
        _fieldHeight = fieldHeight;
        // direction initiale aléatoire
        int dirX = Rand.Next(0, 2) == 0 ? -1 : 1;
        // angle vertical aléatoire
        float randomAngle = (float)(Rand.NextDouble() * 0.4 - 0.2); // entre -0.2 et +0.2
        VelocityX = velocityX * dirX;
        VelocityY = velocityY * randomAngle;
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
        int dirX = Rand.Next(0, 2) == 0 ? -1 : 1;
        float randomAngle = (float)(Rand.NextDouble() * 0.4 - 0.2);
        VelocityX = velocityX * dirX;
        VelocityY = velocityY * randomAngle;
    }
    
    public void BounceFromPaddle(float paddleX, float paddleY, float paddleWidth, float paddleHeight, bool isLeftPaddle)
    {
        // ajustement pour que la collision corresponde à l'affichage
        //////////////////////////////////////demander au prof
        float collisionRadius = Radius * 0.4f; // plus petit que le rayon
        
        // Vérifie si la balle est dans la plage verticale de la raquette
        bool verticallyAligned = Y + Radius >= paddleY - paddleHeight / 2f && Y - Radius <= paddleY + paddleHeight / 2f;
        
        if (!verticallyAligned) return;

        if (isLeftPaddle)
        {
            // contact réel : le bord droit de la raquette avec le bord gauche de la balle
            if (X - collisionRadius <= paddleX + paddleWidth / 2f)
            {
                VelocityX = Math.Abs(VelocityX);
                X = paddleX + paddleWidth / 2f + collisionRadius;
                ApplyBounceAngle(paddleY, paddleHeight);
                /*float hitPos = (Y - paddleY) / (paddleHeight / 2f);
                VelocityY = hitPos * Math.Abs(VelocityX);*/
                // Accélération légère à chaque rebond
                VelocityX *= 1.25f;
                VelocityY *= 1.25f;
                /////////////////EST CE QU'ON DEVRAIT AUSSI AUGMENTER LA VITESSE DES PADDLES ?
            }
        }
        else
        {
            // contact réel : le bord gauche de la raquette avec le bord droit de la balle
            if (X + collisionRadius >= paddleX - paddleWidth / 2f)
            {
                VelocityX = -Math.Abs(VelocityX);
                X = paddleX - paddleWidth / 2f - collisionRadius;
                ApplyBounceAngle(paddleY, paddleHeight);
                /*float hitPos = (Y - paddleY) / (paddleHeight / 2f);
                VelocityY = hitPos * Math.Abs(VelocityX);*/
                // Accélération légère à chaque rebond
                VelocityX *= 1.25f;
                VelocityY *= 1.25f;
            }
        }
    }
    
    private void ApplyBounceAngle(float paddleY, float paddleHeight)
    {
        // Détermine le point d’impact sur la raquette
        float hitPos = (Y - paddleY) / (paddleHeight / 2f);

        // Adapte la direction verticale selon le point d’impact
        VelocityY = hitPos * Math.Abs(VelocityX);

        // Clamp pour éviter les angles trop verticaux
        VelocityY = Math.Clamp(VelocityY, -0.5f, 0.5f);
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

    public bool IsOutLeft() => X - Radius * 0.4 < -_fieldWidth / 2f;   //but : point pour joueur à droite
   
    public bool IsOutRight() => X + Radius *0.4 > _fieldWidth / 2f;   //but : point pour joueur à gauche
}