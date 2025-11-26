using GameCore;
using Godot;
using System;


namespace Pong.Scripts;

public partial class Main : Node2D
{
	// GameCore
	
	private AudioStreamPlayer _bouncesound;
	private AudioStreamPlayer _goalsound;
	private GameState _gameState;
	
	// références aux nœuds visuels
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
	private PlayerInputReader _inputReader1;  // Player 1
	private PlayerInputReader _inputReader2;  // Player 2

	
	// pour conversion entre monde normalisé et pixels
	private Vector2 _windowSize;

	public override void _Ready()
	{
		// récupérer les nœuds de la scène
		_ballNode = GetNode<Area2D>("Ball");
		_paddle1Node = GetNode<Area2D>("Paddle1");
		_paddle2Node = GetNode<Area2D>("Paddle2");
		_ballSprite = _ballNode.GetNode<Sprite2D>("Sprite2D");
		_paddle1Sprite = _paddle1Node.GetNode<Sprite2D>("Sprite2D");
		_paddle2Sprite = _paddle2Node.GetNode<Sprite2D>("Sprite2D");
		
		
		//sound
		
		_bouncesound = GetNode<AudioStreamPlayer>("BounceSound");
		_goalsound = GetNode<AudioStreamPlayer>("GoalSound");
		// Taille de la fenêtre
		_windowSize = GetViewport().GetVisibleRect().Size;
		
		// initialisation du GameCore
		_gameState = new GameState();
		_inputReader1 = new PlayerInputReader();
		_inputReader2 = new PlayerInputReader();
		_gameLogger = new GameLogger(_gameState, _inputReader1, _inputReader2);
		
		_labelP1 = GetNode<Label>("Label1");
		_labelP2 = GetNode<Label>("Label2");
		
		// Game Over UI
		_gameOverUI = GetNode<CanvasLayer>("GameOverUI");
		_gameOverLabel = _gameOverUI.GetNode<Label>("VBoxContainer/GameOverLabel");
		_restartButton = _gameOverUI.GetNode<Button>("VBoxContainer/RestartButton");
		_restartButton.Pressed += OnRestartPressed;
		_gameOverUI.Visible = false;
		//_restartButton.PauseMode = PauseMode.Process;

		
		UpdateVisuals();
		
		/*var (p1X, p1Y) = _gameState.GetPaddle1Position();
		var (p2X, p2Y) = _gameState.GetPaddle2Position();
		_paddle1Node.Position = new Vector2(p1X, p1Y);
		_paddle2Node.Position = new Vector2(p2X, p2Y);*/
	}
	
	public override void _Process(double delta)
	{
		float deltaTime = (float)delta;

		// récupérer les intentions des joueurs
		var p1Intention = GetPlayerIntention1();
		var p2Intention = GetPlayerIntention2();
		
		// mettre à jour la logique du jeu
		_inputReader1.SetIntention(p1Intention.Move);
		_inputReader2.SetIntention(p2Intention.Move);
		_gameState.Update(deltaTime, p1Intention, p2Intention);
		
		if (_gameState.GetBallBounceSoundFlag())
		{
			_bouncesound.Play();
			_gameState.SetBallBounceSoundFlag(false);
		}

		if (_gameState.GetScoreGoalSoundFlag())
		{
			_goalsound.Play();
			_gameState.SetScoreGoalSoundFlag(false);
		}
		
		if (!_isGameOver && _gameState.IsGameOver())
		{
			var winnerId = _gameState.GetScore().Item1 > _gameState.GetScore().Item2 ? 0 : 1;
			string winner = winnerId == 0 ? "Player 1" : "Player 2";
			OnGameOver(winner);
		}



		// mettre à jour l'affichage
		UpdateVisuals();
	}
	
	
	private PlayerIntention GetPlayerIntention1()
	{
		int move = PlayerIntention.Neutral;

		if (Input.IsActionPressed("p1_up")) move = PlayerIntention.Up;
		else if (Input.IsActionPressed("p1_down")) move = PlayerIntention.Down;

		return new PlayerIntention(move);
	}

	private PlayerIntention GetPlayerIntention2()
	{
		int move = PlayerIntention.Neutral;

		if (Input.IsActionPressed("p2_up")) move = PlayerIntention.Up;
		else if (Input.IsActionPressed("p2_down")) move = PlayerIntention.Down;

		return new PlayerIntention(move);
	}
	
	private void UpdateVisuals()
	{
		Vector2 ToPixels(float normX, float normY)
		{
			// (-0.5, -0.5) → (0,0)
			// (+0.5, +0.5) → (windowWidth, windowHeight)
			float x = (normX + 0.5f) * _windowSize.X;
			float y = (normY + 0.5f) * _windowSize.Y;
			return new Vector2(x, y);
		}
		
		// balle
		var (ballX, ballY) = _gameState.GetBallPosition();
		_ballNode.Position = ToPixels(ballX, ballY);
		
		// mettre à l'échelle selon le monde normalisé
		var ballRadiusNorm = _gameState.GetBallRadius();
		float pixelDiameter = ballRadiusNorm * 2 * _windowSize.Y;
		_ballSprite.Scale = new Vector2(pixelDiameter / _ballSprite.Texture.GetSize().X, pixelDiameter / _ballSprite.Texture.GetSize().Y);

		// raquettes
		var (p1X, p1Y) = _gameState.GetPaddle1Position();
		var (p2X, p2Y) = _gameState.GetPaddle2Position();
		_paddle1Node.Position = ToPixels(p1X, p1Y);
		_paddle2Node.Position = ToPixels(p2X, p2Y);
		
		// mise à l'échelle selon le monde normalisé
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
		if (_isGameOver) return;

		_isGameOver = true;
		_gameOverLabel.Text = $"Game Over\n{winner} Wins!";
		_gameOverUI.Visible = true;
		GetTree().Paused = true;
		
		// Stop logger and save to desktop
		string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
		string csvPath = System.IO.Path.Combine(desktopPath, "pong_data_test.csv");
		_gameLogger.SaveCsv(csvPath);
		_gameLogger.Stop();
	}

	private void OnRestartPressed()
	{
		// Recreate both GameState and GameLogger
		_gameState = new GameState();
		_inputReader1 = new PlayerInputReader();
		_inputReader2 = new PlayerInputReader();
		_gameLogger = new GameLogger(_gameState, _inputReader1, _inputReader2);
		
		_isGameOver = false;
		_gameOverUI.Visible = false;
		GetTree().Paused = false;
		UpdateVisuals();
	}
}
