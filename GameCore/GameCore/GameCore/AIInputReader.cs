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

        [DllImport("ClassifieurLineairePerceptronMultiClasses")]
        private static extern void set_weights(IntPtr model, float[] inWeights);

        [DllImport("ClassifieurLineairePerceptronMultiClasses")]
        private static extern void set_bias(IntPtr model, float[] inBias);

        //attributs
        private IntPtr _model;
        private readonly int _inputSize;
        private readonly int _numClasses;
        private float[] _currentInputs;
        private bool _isReady;

        public AIInputReader(int inputSize = 5, int numClasses = 3, float learningRate = 0.01f)
        {
            _inputSize = inputSize;
            _numClasses = numClasses;
            _currentInputs = new float[inputSize];
            _isReady = false;

            _model = create_perceptron_multiclasses(inputSize, numClasses, learningRate);
        }

        //charge un agent depuis un fichier JSON (parsing manuel sans bibliothèque externe)
        public void LoadFromJSON(string jsonPath)
        {
            if(!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("Agent JSON non trouvé : " + jsonPath);
            }

            string contenu = File.ReadAllText(jsonPath);

            //extraction des poids
            List<float> poidsFlat = new List<float>();
            int posPoids = contenu.IndexOf("\"poids\":");
            if(posPoids != -1)
            {
                int debutTableauExt = contenu.IndexOf("[", posPoids);
                int niveau = 0;
                int finTableauExt = debutTableauExt;
                for(int i = debutTableauExt ; i < contenu.Length ; ++i)
                {
                    if(contenu[i] == '[')
                    {
                        niveau++;
                    }
                    else if(contenu[i] == ']')
                    {
                        niveau--;
                        if(niveau == 0)
                        {
                            finTableauExt = i;
                            break;
                        }
                    }
                }

                string poidsStr = contenu.Substring(debutTableauExt + 1, finTableauExt - debutTableauExt - 1);
                
                //extraire tous les nombres
                string nombreCourant = "";
                for(int i = 0 ; i < poidsStr.Length ; ++i)
                {
                    char c = poidsStr[i];
                    if(char.IsDigit(c) || c == '.' || c == '-' || c == 'e' || c == 'E' || c == '+')
                    {
                        nombreCourant += c;
                    }
                    else if(nombreCourant.Length > 0)
                    {
                        poidsFlat.Add(float.Parse(nombreCourant, CultureInfo.InvariantCulture));
                        nombreCourant = "";
                    }
                }
                if(nombreCourant.Length > 0)
                {
                    poidsFlat.Add(float.Parse(nombreCourant, CultureInfo.InvariantCulture));
                }
            }

            //extraction des biais
            List<float> biais = new List<float>();
            int posBiais = contenu.IndexOf("\"biais\":");
            if(posBiais != -1)
            {
                int debutTableau = contenu.IndexOf("[", posBiais);
                int finTableau = contenu.IndexOf("]", debutTableau);
                string biaisStr = contenu.Substring(debutTableau + 1, finTableau - debutTableau - 1);
                
                string nombreCourant = "";
                for(int i = 0 ; i < biaisStr.Length ; ++i)
                {
                    char c = biaisStr[i];
                    if(char.IsDigit(c) || c == '.' || c == '-' || c == 'e' || c == 'E' || c == '+')
                    {
                        nombreCourant += c;
                    }
                    else if(nombreCourant.Length > 0)
                    {
                        biais.Add(float.Parse(nombreCourant, CultureInfo.InvariantCulture));
                        nombreCourant = "";
                    }
                }
                if(nombreCourant.Length > 0)
                {
                    biais.Add(float.Parse(nombreCourant, CultureInfo.InvariantCulture));
                }
            }

            //appliquer les poids et biais au modèle
            if(poidsFlat.Count > 0)
            {
                set_weights(_model, poidsFlat.ToArray());
            }
            if(biais.Count > 0)
            {
                set_bias(_model, biais.ToArray());
            }

            _isReady = true;
        }

        /*
        //ancienne méthode : charge le dataset CSV et entraîne le modèle
        //format CSV : ballX, ballY, ballVX, ballVY, playerY, enemyY, playerMove, enemyMove
        public void LoadAndTrain(string csvPath, int epochs = 1000)
        {
            ...
        }
        */

        //met à jour l'état du jeu pour la prédiction (5 entrées : ballX, ballY, ballVX, ballVY, playerY)
        public void SetGameState(float ballX, float ballY, float ballVX, float ballVY, float playerY)
        {
            _currentInputs[0] = ballX;
            _currentInputs[1] = ballY;
            _currentInputs[2] = ballVX;
            _currentInputs[3] = ballVY;
            _currentInputs[4] = playerY;
        }

        public int ReadIntention()
        {
            if(!_isReady)
            {
                return 0;//neutre si pas encore entraîné
            }

            int predictedClass = predict_perceptron_multiclasses(_model, _currentInputs);

            //conversion classe en intention (mapping uniforme)
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