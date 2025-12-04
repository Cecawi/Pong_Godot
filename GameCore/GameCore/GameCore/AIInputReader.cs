using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GameCore
{
    public class AIInputReader : IPlayerInputReader
    {
        [DllImport("ClassifieurLineairePerceptronMultiClasses")]
        private static extern IntPtr create_perceptron_multiclasses(int inputSize, int numClasses, float lr);

        [DllImport("ClassifieurLineairePerceptronMultiClasses")]
        private static extern void train_perceptron_multiclasses(IntPtr model, float[] X, int[] Y, int rows, int cols, int epochs);

        [DllImport("ClassifieurLineairePerceptronMultiClasses")]
        private static extern int predict_perceptron_multiclasses(IntPtr model, float[] input);

        [DllImport("ClassifieurLineairePerceptronMultiClasses")]
        private static extern void destroy_perceptron_multiclasses(IntPtr model);

        //attributs
        private IntPtr _model;
        private readonly int _inputSize;
        private readonly int _numClasses;
        private float[] _currentInputs;
        private bool _isReady;

        public AIInputReader(int inputSize = 7, int numClasses = 3, float learningRate = 0.01f)
        {
            _inputSize = inputSize;
            _numClasses = numClasses;
            _currentInputs = new float[inputSize];
            _isReady = false;

            _model = create_perceptron_multiclasses(inputSize, numClasses, learningRate);
        }

        //charge le dataset CSV et entraîne le modèle
        //format CSV : ballX, ballY, ballVX, ballVY, playerY, enemyY, playerMove, enemyMove
        public void LoadAndTrain(string csvPath, int epochs = 1000)
        {
            if(!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Dataset non trouvé : " + csvPath);
            }

            List<float[]> inputsList = new List<float[]>();
            List<int> outputsList = new List<int>();

            string[] lines = File.ReadAllLines(csvPath);

            int compteur = 0;

            foreach(string line in lines)
            {
                if(string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(',');
                if(parts.Length < 8)
                {
                    continue;
                }
////////////////////////////////////////////////
                if(parts[6] == "0" && compteur < 1000)
                {
                    compteur++;
                }
                if(parts[6] == "0" && compteur >= 1000)
                {
                    continue;
                }
////////////////////////////////////////////////
                //entrées : indices 0, 1, 2, 3, 4, 5, 7
                float[] input = new float[7];
                input[0] = float.Parse(parts[0], CultureInfo.InvariantCulture);//ballX
                input[1] = float.Parse(parts[1], CultureInfo.InvariantCulture);//ballY
                input[2] = float.Parse(parts[2], CultureInfo.InvariantCulture);//ballVX
                input[3] = float.Parse(parts[3], CultureInfo.InvariantCulture);//ballVY
                input[4] = float.Parse(parts[4], CultureInfo.InvariantCulture);//playerY
                input[5] = float.Parse(parts[5], CultureInfo.InvariantCulture);//enemyY
                input[6] = float.Parse(parts[7], CultureInfo.InvariantCulture);//deltaTime (indice 7)

                //sortie : indice 6 (playerMove)
                float rawOutput = float.Parse(parts[6], CultureInfo.InvariantCulture);

                //conversion en classes (0, 1, 2)
                //rawOutput = 1.0 : haut (Up = -1) : classe 0
                //rawOutput = 0.0 : neutre (Neutral = 0) : classe 1
                //rawOutput = -1.0 : bas (Down = 1) : classe 2
                int classIndex = 1;//par défaut : neutre
                if (rawOutput > 0.5f)
                {
                    classIndex = 0;//haut
                }
                else if (rawOutput < -0.5f)
                {
                    classIndex = 2;//bas
                }

                inputsList.Add(input);
                outputsList.Add(classIndex);
            }

            int rows = inputsList.Count;
            int cols = _inputSize;

            //aplatir les données pour le C++
            float[] Xflat = new float[rows * cols];
            int[] Y = outputsList.ToArray();

            for(int i = 0 ; i < rows ; i++)
            {
                for(int j = 0 ; j < cols ; j++)
                {
                    Xflat[i * cols + j] = inputsList[i][j];
                }
            }

            //entraînement du modèle
            train_perceptron_multiclasses(_model, Xflat, Y, rows, cols, epochs);
            _isReady = true;
        }

        //met à jour l'état du jeu pour la prédiction
        public void SetGameState(float ballX, float ballY, float ballVX, float ballVY, 
                                  float playerY, float enemyY, float deltaTime)
        {
            _currentInputs[0] = ballX;
            _currentInputs[1] = ballY;
            _currentInputs[2] = ballVX;
            _currentInputs[3] = ballVY;
            _currentInputs[4] = playerY;
            _currentInputs[5] = enemyY;
            _currentInputs[6] = deltaTime;
        }

        public int ReadIntention()
        {
            if(!_isReady)
            {
                return 0;//neutre si pas encore entraîné
            }

            int predictedClass = predict_perceptron_multiclasses(_model, _currentInputs);

            //conversion classe en intention
            //classe 0 : haut (-1)
            //classe 1 : neutre (0)
            //classe 2 : bas (1)
            switch(predictedClass)
            {
                case 0:
                    return PlayerIntention.Up;//-1
                case 2:
                    return PlayerIntention.Down;//1
                default:
                    return PlayerIntention.Neutral;//0
            }
        }

        ~AIInputReader()
        {
            if(_model != IntPtr.Zero)
            {
                destroy_perceptron_multiclasses(_model);
                _model = IntPtr.Zero;
            }
        }
    }
}
