namespace GameCore;

//objectif : représenter la physique et les règles de déplacement de la balle
//sans dépendre d’aucun moteur graphique

//il y a presque tout, faut regarder et vérifier les commentaires
public class Ball
{
    
    //attributs
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Radius { get; }///
                                /// ///////////////voir si on garde le rayon de la balle, normalement oui
                                ///
    public float VelocityX { get; private set; }
    public float VelocityY { get; private set; }
    //"taille" du terrain : 
    private readonly float _fieldWidth;//x (gauche : 0/droite : fieldWidth)
    private readonly float _fieldHeight;//y (haut : 0/bas : fieldHeight)
    //readonly : on doit lui assigner une valeur dans le constructeur
    
    //constructeur
    public Ball(float startX, float startY, float setOnceRadius, float velocityX, float velocityY, float fieldWidth, float fieldHeight)
    {
        X = startX;
        Y = startY;
        Radius = setOnceRadius;
        VelocityX = velocityX;
        VelocityY = velocityY;
        this._fieldWidth = fieldWidth;
        this._fieldHeight = fieldHeight;
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

        if (Y - Radius <= 0 || Y + Radius >= _fieldHeight)
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
        if(Y + Radius >= paddleY && Y - Radius <= paddleY + paddleHeight)
        {
            if(isLeftPaddle && X - Radius <= paddleX)
            {
                //rebond sur la raquette gauche
                VelocityX = Math.Abs(VelocityX);
                X = paddleX + Radius;
            }
            if(!isLeftPaddle && X + Radius >= paddleX)
            {
                //rebond sur la raquette droite
                VelocityX = -Math.Abs(VelocityX);
                X = paddleX - Radius;
            }

            //modifie légèrement la direction verticale selon le point d'impact
            float hitPos = (Y - (paddleY + paddleHeight / 2)) / (paddleHeight / 2);
            VelocityY = hitPos * Math.Abs(VelocityX);
        }
    }
    
    //hitPos : point d'impact avec la balle
    //hitPos : vaut entre -1 et 1 : valeur proportionnelle à l'endroit de l'impact
    //paddleY + (paddleHeight / 2) : centre vertical du paddle
    //Y - (paddleY + paddleHeight / 2) : distance entre la balle et le centre du paddle
    //on divise par paddleHeight / 2 pour etre entre -1 et 1
    //* Math.Abs(VelocityX) : valeur absolue  : la vitesse horizontale sans signe
    //* Math.Abs(VelocityX) : lie la puissance verticale à la vitesse horizontale : vitesse totale à peu près constante
    //plus la balle frappe loin du centre, plus l’angle du rebond est fort
    
    /*
    ↑ 0
    │
    │   balle.Y
    │     ↓
    │     ●  ← la balle
    │
    │ [paddleY]       ← haut de la raquette
    │ |             |
    │ |   RAQUETTE  | ← hauteur = paddleHeight
    │ |             |
    │ [paddleY + paddleHeight] ← bas de la raquette
    │
    ↓ fieldHeight
    */
    
    private void ClampY()//replace la balle à l'intérieur du terrain après un rebond
    {
        if (Y - Radius < 0)
        {
            Y = Radius;
        }

        if (Y + Radius > _fieldHeight)
        {
            Y = _fieldHeight - Radius;
        }
    }

    public bool IsOutLeft() => X + Radius < 0;//but : point pour joueur à droite
   
    public bool IsOutRight() => X - Radius > _fieldWidth;//but : point pour joueur à gauche
}