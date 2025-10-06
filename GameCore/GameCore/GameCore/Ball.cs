//calculer le point de collision et déduire l’angle de rebond
    //classe collisions/contacts???
    //Vitesse/angle balle (peut être encapsulé dans la position directement)
    
    
    




namespace GameCore
{
    public class Ball
    {
         // Ducoup les attributs ici
        public float X { get; private set; }
        public float Y { get; private set; }
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        private readonly float fieldWidth;
        private readonly float fieldHeight;

        // un constructeur
        public Ball(float startX, float startY, float velocityX, float velocityY, float fieldWidth, float fieldHeight);

        // les méthodes
        public void Update(float deltaTime);
        public void Reset(float startX, float startY, float velocityX, float velocityY);
        public void BounceFromPaddle(float paddleX, float paddleY, float paddleHeight, bool isLeftPaddle);
        private void ClampY();
        public bool IsOutLeft();
        public bool IsOutRight();
    }
}








// ici bas une conception du code












// namespace GameCore
// {
//     public class Ball
//     {
//         public float X { get; private set; }
//         public float Y { get; private set; }
//         public float VelocityX { get; private set; }
//         public float VelocityY { get; private set; }
//
//         private readonly float fieldWidth;
//         private readonly float fieldHeight;
//
//         public Ball(float startX, float startY, float velocityX, float velocityY, float fieldWidth, float fieldHeight)
//         {
//             X = startX;
//             Y = startY;
//             VelocityX = velocityX;
//             VelocityY = velocityY;
//             this.fieldWidth = fieldWidth;
//             this.fieldHeight = fieldHeight;
//         }
//
//         public void Update(float deltaTime)
//         {
//             X += VelocityX * deltaTime;
//             Y += VelocityY * deltaTime;
//
//             if (Y <= 0 || Y >= fieldHeight)
//             {
//                 VelocityY = -VelocityY;
//                 ClampY();
//             }
//         }
//
//         public void Reset(float startX, float startY, float velocityX, float velocityY)
//         {
//             X = startX;
//             Y = startY;
//             VelocityX = velocityX;
//             VelocityY = velocityY;
//         }
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

