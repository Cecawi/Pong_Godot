using GameCore;
using Godot;

namespace Pong.Scripts;

public partial class Main : Node2D
{
	// GameCore
	private GameState _gameState;
	
	// références aux nœuds visuels
	private Area2D _ballNode;
	private Area2D _paddle1Node;
	private Area2D _paddle2Node;
	private Label _labelP1;
	private Label _labelP2;

	
	
	// pour conversion entre monde normalisé et pixels
	private Vector2 _windowSize;

	public override void _Ready()
	{
		// récupérer les nœuds de la scène
		_ballNode = GetNode<Area2D>("Ball");
		_paddle1Node = GetNode<Area2D>("Paddle1");
		_paddle2Node = GetNode<Area2D>("Paddle2");
		
		// Taille de la fenêtre
		_windowSize = GetViewport().GetVisibleRect().Size;
		
		// initialisation du GameCore
		_gameState = new GameState();
		_labelP1 = GetNode<Label>("Label1");
		_labelP2 = GetNode<Label>("Label2");
		
		UpdateVisuals();
		
		var (p1X, p1Y) = _gameState.GetPaddle1Position();
		var (p2X, p2Y) = _gameState.GetPaddle2Position();
		_paddle1Node.Position = new Vector2(p1X, p1Y);
		_paddle2Node.Position = new Vector2(p2X, p2Y);
		

	}
	
	public override void _Process(double delta)
	{
		float deltaTime = (float)delta;

		// récupérer les intentions des joueurs
		var p1Intention = GetPlayerIntention1();
		var p2Intention = GetPlayerIntention2();

		// mettre à jour la logique du jeu
		_gameState.Update(deltaTime, p1Intention, p2Intention);

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

		// raquettes
		var (p1X, p1Y) = _gameState.GetPaddle1Position();
		var (p2X, p2Y) = _gameState.GetPaddle2Position();
		_paddle1Node.Position = ToPixels(p1X, p1Y);
		_paddle2Node.Position = ToPixels(p2X, p2Y);
		
		
		var (s1, s2) = _gameState.GetScore();
		_labelP1.Text = s1.ToString();
		_labelP2.Text = s2.ToString();

	}
}
