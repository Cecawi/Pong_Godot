namespace GameCore
{
    // objectif : représenter une raquette (paddle) pour un joueur
    // gère le déplacement vertical et empêche de sortir du terrain
    public class Paddle
    {
        // attributs
        public float X { get; private set; }
        // already defined
        public float Y { get; private set; }
        public float Width { get; }   // largeur de la raquette
        public float Height { get; }  // hauteur de la raquette
        public float Speed { get; }   // vitesse de déplacement verticale

        private readonly float _fieldHeight; // limite haute et basse du terrain

        // constructeur
        public Paddle(float startX, float startY, float width, float height, float speed, float fieldHeight)
        {
            X = startX;
            Y = startY;
            Width = width;
            Height = height;
            Speed = speed;
            _fieldHeight = fieldHeight;
        }

        // méthode pour récupérer la position
        public (float x, float y) GetPosition()
        {
            return (X, Y);
        }

        // déplace la raquette verticalement selon la direction donnée (-1 = haut, 0 = neutre, 1 = bas)
        public void Move(int intention, float deltaTime)
        {
            Y += intention * Speed * deltaTime;
            ClampY();
        }
        
        /*// méthode pour déplacer la raquette vers le haut
        public void MoveUp(float deltaTime)
        {
            Y -= Speed * deltaTime;
            ClampY();
        }

        // méthode pour déplacer la raquette vers le bas
        public void MoveDown(float deltaTime)
        {
            Y += Speed * deltaTime;
            ClampY();
        }*/

        // méthode pour rester dans le terrain
        private void ClampY()
        {
            if (Y < 0)
            {
                Y = 0;
            }

            if (Y + Height > _fieldHeight)
            {
                Y = _fieldHeight - Height;
            }
        }
    }
}