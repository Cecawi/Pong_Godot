using GameCore;
using Godot;
using System;

//TODO : si partie avec IA : pas de création de dataset ou enregistrement "mirroir"

namespace Pong.Scripts;

public partial class Main : Node2D
{
	//GameCore
	private GameState _gameState;
	
	//références aux nœuds visuels
	private Area2D _ballNode;
	private Area2D _paddle1Node;
	private Area2D _paddle2Node;
	private Label _labelP1;
	private Label _labelP2;

	private Sprite2D _ballSprite;
	private Sprite2D _paddle1Sprite;
	private Sprite2D _paddle2Sprite;
	
	private CanvasLayer _gameOverUI;
	private Label _gameOverLabel;
	private Button _restartButton;
	private bool _isGameOver = false;
	private GameLogger _gameLogger;
	private PlayerInputReader _inputReader1;//player 1
	private PlayerInputReader _inputReader2;//player 2

	private AudioStreamPlayer _bouncesound;
	private AudioStreamPlayer _goalsound;
	
	//agent IA pour le joueur 1
	private AIInputReader _aiInputReader;
	private bool _useAI = false;//true = IA contrôle joueur 1, false = clavier
	private float _lastDeltaTime = 0f;

	//UI Elements
	private Control _menuContainer;//conteneur global du menu
	private CheckButton _checkUseAI;
	private OptionButton _optAgentSelection;
	private Button _btnPlay;
	private string _agentsFolderPath = @"C:\Users\yecel\Desktop\ESGI - 4A\T1\Machine Learning\Pong_Joueur_Artificiel\AgentsIA";
	private System.Collections.Generic.List<string> _agentFiles = new System.Collections.Generic.List<string>();
	
	private bool _isGameStarted = false;
	private bool _hasGameStartedOnce = false;
	
	//état avant pause pour vérifier changements
	private bool _wasAIEnabledBeforePause;
	private int _agentIndexBeforePause;

	//gestion input espace
	private bool _spacePressedLastFrame = false;

	//pour conversion entre monde normalisé et pixels
	private Vector2 _windowSize;

	public override void _Ready()
	{
		//récupérer les nœuds de la scène
		_ballNode = GetNode<Area2D>("Ball");
		_paddle1Node = GetNode<Area2D>("Paddle1");
		_paddle2Node = GetNode<Area2D>("Paddle2");
		_ballSprite = _ballNode.GetNode<Sprite2D>("Sprite2D");
		_paddle1Sprite = _paddle1Node.GetNode<Sprite2D>("Sprite2D");
		_paddle2Sprite = _paddle2Node.GetNode<Sprite2D>("Sprite2D");
		
		//sound
		_bouncesound = GetNode<AudioStreamPlayer>("BounceSound");
		_goalsound = GetNode<AudioStreamPlayer>("GoalSound");
		
		//taille de la fenêtre
		_windowSize = GetViewport().GetVisibleRect().Size;
		
		//initialisation du GameCore
		_gameState = new GameState();
		_inputReader1 = new PlayerInputReader();
		_inputReader2 = new PlayerInputReader();
		_gameLogger = new GameLogger(_gameState, _inputReader1, _inputReader2);

		SetupUI();//ajout de l'interface menu

		_labelP1 = GetNode<Label>("Label1");
		_labelP2 = GetNode<Label>("Label2");
		
		//Game Over UI
		_gameOverUI = GetNode<CanvasLayer>("GameOverUI");
		_gameOverLabel = _gameOverUI.GetNode<Label>("VBoxContainer/GameOverLabel");
		_restartButton = _gameOverUI.GetNode<Button>("VBoxContainer/RestartButton");
		_restartButton.Pressed += OnRestartPressed;
		_gameOverUI.Visible = false;

		UpdateVisuals();
	}

	private void SetupUI()
	{
		//container pour l'UI en haut à gauche
		var uiContainer = new VBoxContainer();
		AddChild(uiContainer);
		uiContainer.Position = new Vector2(20, 20);
		_menuContainer = uiContainer;

		//titre menu
		var labelMenu = new Label();
		labelMenu.Text = "MENU";
		uiContainer.AddChild(labelMenu);

		//checkButton : activer ia
		_checkUseAI = new CheckButton();
		_checkUseAI.Text = "Activer IA Joueur 1";
		_checkUseAI.ButtonPressed = _useAI;
		_checkUseAI.Toggled += OnUseAIToggled;
		uiContainer.AddChild(_checkUseAI);

		//optionButton : sélection agent
		_optAgentSelection = new OptionButton();
		_optAgentSelection.ItemSelected += OnAgentSelected;
		uiContainer.AddChild(_optAgentSelection);
		
		//désactivé si ia pas cochée
		_optAgentSelection.Disabled = !_useAI;

		PopulateAgentList();

		//espace
		uiContainer.AddChild(new Control() { CustomMinimumSize = new Vector2(0, 10)});

		//bouton play
		_btnPlay = new Button();
		_btnPlay.Text = "JOUER";
		_btnPlay.Pressed += OnPlayPressed;
		_btnPlay.CustomMinimumSize = new Vector2(100, 40);
		uiContainer.AddChild(_btnPlay);
	}

	private void PopulateAgentList()
	{
		_optAgentSelection.Clear();
		_agentFiles.Clear();

		if( System.IO.Directory.Exists(_agentsFolderPath) )
		{
			string[] files = System.IO.Directory.GetFiles(_agentsFolderPath, "*.json");
			foreach( string file in files )
			{
				string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
				_agentFiles.Add(file);

				//formatage : 1ère lettre majuscule et _ en espace
				string formattedName = fileName.Replace("_", " ");
				if( formattedName.Length > 0 )
				{
					formattedName = char.ToUpper(formattedName[0]) + formattedName.Substring(1);
				}
				
				_optAgentSelection.AddItem(formattedName);
			}
		}

		if( _agentFiles.Count > 0 )
		{
			_optAgentSelection.Select(0);
		}
	}

	private void OnUseAIToggled(bool pressed)
	{
		_useAI = pressed;
		_optAgentSelection.Disabled = !_useAI;
		UpdatePlayButtonState();
	}

	private void OnAgentSelected(long index)
	{
		UpdatePlayButtonState();
	}

	//mise à jour du texte du bouton selon l'état
	private void UpdatePlayButtonState()
	{
		if( !_hasGameStartedOnce )
		{
			_btnPlay.Text = "JOUER";
			return;
		}

		//si paramètres changés pendant la pause -> recommencer
		bool paramsChanged = (_useAI != _wasAIEnabledBeforePause) || 
							 (_optAgentSelection.Selected != _agentIndexBeforePause);
		
		if( paramsChanged )
		{
			_btnPlay.Text = "RECOMMENCER";
		}
		else
		{
			_btnPlay.Text = "CONTINUER";
		}
	}

	//clic sur jouer
	private void OnPlayPressed()
	{
		//cas continuer
		if( _btnPlay.Text == "CONTINUER" )
		{
			_menuContainer.Visible = false;
			_isGameStarted = true;
			return;
		}

		//cas jouer ou recommencer
		GD.Print("Lancement de la partie...");
		
		_hasGameStartedOnce = true;

		//charger l'ia si demandée
		if( _useAI )
		{
			LoadSelectedAgent();
		}
		else
		{
			_aiInputReader = null;
		}

		//masquer le menu
		_menuContainer.Visible = false;
		
		//démarrer le jeu
		_isGameStarted = true;
		
		//réinitialiser le jeu pour être propre
		_gameState = new GameState();
		_inputReader1 = new PlayerInputReader();
		_inputReader2 = new PlayerInputReader();
		_gameLogger = new GameLogger(_gameState, _inputReader1, _inputReader2);
		UpdateVisuals();
	}

	//mettre le jeu en pause et afficher le menu
	private void PauseGame()
	{
		_isGameStarted = false;
		_menuContainer.Visible = true;
		
		//sauvegarder l'état actuel pour comparer
		_wasAIEnabledBeforePause = _useAI;
		_agentIndexBeforePause = _optAgentSelection.Selected;
		
		UpdatePlayButtonState();
	}

	private void LoadSelectedAgent()
	{
		//si la liste est vide, on ne fait rien
		if( _agentFiles.Count == 0 )
		{
			return;
		}

		int index = _optAgentSelection.Selected;
		
		if( index >= 0 && index < _agentFiles.Count )
		{
			string jsonPath = _agentFiles[index];
			
			//nettoyer ancienne ia si existe
			_aiInputReader = null; 
			
			//créer nouvelle instance
			_aiInputReader = new AIInputReader(inputSize : 5, numClasses : 3, learningRate : 0.01f);
			
			try 
			{
				_aiInputReader.LoadFromJSON(jsonPath);
				GD.Print("Agent IA chargé : " + jsonPath);
			}
			catch ( System.Exception e )
			{
				GD.PrintErr("Erreur chargement IA : " + e.Message);
			}
		}
	}
	
	public override void _Process(double delta)
	{
		//gestion pause (espace)
		bool spacePressed = Input.IsKeyPressed(Key.Space);
		if( spacePressed && !_spacePressedLastFrame )
		{
			if( _isGameStarted )
			{
				PauseGame();
			}
		}
		_spacePressedLastFrame = spacePressed;

		//si le jeu n'est pas lancé, on ne met rien à jour
		if( !_isGameStarted )
		{
			return;
		}
		
		float deltaTime = (float)delta;
		_lastDeltaTime = deltaTime;

		//mettre à jour l'état du jeu pour l'ia (5 entrées simplifiées)
		if( _useAI && _aiInputReader != null )
		{
			_aiInputReader.SetGameState
			(
				_gameState.BallX, _gameState.BallY,
				_gameState.BallVX, _gameState.BallVY,
				_gameState.PlayerY
			);
		}

		//récupérer les intentions des joueurs
		var p1Intention = GetPlayerIntention1();
		var p2Intention = GetPlayerIntention2();
		
		//mettre à jour la logique du jeu
		_inputReader1.SetIntention(p1Intention.Move);
		_inputReader2.SetIntention(p2Intention.Move);
		_gameState.Update(deltaTime, p1Intention, p2Intention);
		
		if( _gameState.GetBallBounceSoundFlag() )
		{
			_bouncesound.Play();
			_gameState.SetBallBounceSoundFlag(false);
		}

		if( _gameState.GetScoreGoalSoundFlag() )
		{
			_goalsound.Play();
			_gameState.SetScoreGoalSoundFlag(false);
		}
		
		if( !_isGameOver && _gameState.IsGameOver() )
		{
			var winnerId = _gameState.GetScore().Item1 > _gameState.GetScore().Item2 ? 0 : 1;
			string winner = winnerId == 0 ? "Player 1" : "Player 2";
			OnGameOver(winner);
		}

		//mettre à jour l'affichage
		UpdateVisuals();
	}
	
	
	private PlayerIntention GetPlayerIntention1()
	{
		//si l'ia est activée, utiliser la prédiction de l'agent
		if( _useAI && _aiInputReader != null )
		{
			int aiMove = _aiInputReader.ReadIntention();
			return new PlayerIntention(aiMove);
		}
		
		//sinon, utiliser le clavier
		int move = PlayerIntention.Neutral;

		if( Input.IsActionPressed("p1_up") )
		{
			move = PlayerIntention.Up;
		}
		else if( Input.IsActionPressed("p1_down") )
		{
			move = PlayerIntention.Down;
		}

		return new PlayerIntention(move);
	}

	private PlayerIntention GetPlayerIntention2()
	{
		int move = PlayerIntention.Neutral;

		if( Input.IsActionPressed("p2_up") )
		{
			move = PlayerIntention.Up;
		}
		else if( Input.IsActionPressed("p2_down") )
		{
			move = PlayerIntention.Down;
		}

		return new PlayerIntention(move);
	}
	
	private void UpdateVisuals()
	{
		Vector2 ToPixels(float normX, float normY)
		{
			//(-0.5, -0.5) → (0,0)
			//(+0.5, +0.5) → (windowWidth, windowHeight)
			float x = (normX + 0.5f) * _windowSize.X;
			float y = (normY + 0.5f) * _windowSize.Y;
			return new Vector2(x, y);
		}
		
		//balle
		var (ballX, ballY) = _gameState.GetBallPosition();
		_ballNode.Position = ToPixels(ballX, ballY);
		
		//mettre à l'échelle selon le monde normalisé
		var ballRadiusNorm = _gameState.GetBallRadius();
		float pixelDiameter = ballRadiusNorm * 2 * _windowSize.Y;
		_ballSprite.Scale = new Vector2(pixelDiameter / _ballSprite.Texture.GetSize().X, pixelDiameter / _ballSprite.Texture.GetSize().Y);

		//raquettes
		var (p1X, p1Y) = _gameState.GetPaddle1Position();
		var (p2X, p2Y) = _gameState.GetPaddle2Position();
		_paddle1Node.Position = ToPixels(p1X, p1Y);
		_paddle2Node.Position = ToPixels(p2X, p2Y);
		
		//mise à l'échelle selon le monde normalisé
		var (p1W, p1H) = _gameState.GetPaddle1Size();
		var (p2W, p2H) = _gameState.GetPaddle2Size();
		_paddle1Sprite.Scale = new Vector2(p1W * _windowSize.X / _paddle1Sprite.Texture.GetSize().X, p1H * _windowSize.Y / _paddle1Sprite.Texture.GetSize().Y);
		_paddle2Sprite.Scale = new Vector2(p2W * _windowSize.X / _paddle2Sprite.Texture.GetSize().X, p2H * _windowSize.Y / _paddle2Sprite.Texture.GetSize().Y);
		
		var (s1, s2) = _gameState.GetScore();
		_labelP1.Text = s1.ToString();
		_labelP2.Text = s2.ToString();
	}
	
	private void OnGameOver(string winner)
	{
		if( _isGameOver )
		{
			return;
		}

		_isGameOver = true;
		_gameOverLabel.Text = $"Game Over\n{winner} Wins!";
		_gameOverUI.Visible = true;
		GetTree().Paused = true;
		
		//stop logger and save to desktop
		string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		string csvPath = System.IO.Path.Combine(desktopPath, "pong_data_test.csv");
		_gameLogger.SaveCsv(csvPath);
		_gameLogger.Stop();
	}

	private void OnRestartPressed()
	{
		//recreate both GameState and GameLogger
		_gameState = new GameState();
		_inputReader1 = new PlayerInputReader();
		_inputReader2 = new PlayerInputReader();
		_gameLogger = new GameLogger(_gameState, _inputReader1, _inputReader2);
		
		_isGameOver = false;
		_gameOverUI.Visible = false;
		GetTree().Paused = false;

		//réafficher le menu
		_isGameStarted = false;
		_menuContainer.Visible = true;

		UpdateVisuals();
	}
}