using GameCore;
using Godot;

namespace Pong.Scripts;

public partial class Main : Node2D
{
	//GameCore
	private GameState _gameState;
	
	//Références aux nœuds visuels
	private Area2D _ballNode;
	private Area2D _paddle1Node;
	private Area2D _paddle2Node;

	public override void _Ready()
	{
		// initialisation du GameCore
		_gameState = new GameState();
		
		// récupérer les nœuds de la scène
		_ballNode = GetNode<Area2D>("Ball");
		_paddle1Node = GetNode<Area2D>("Paddle1");
		_paddle2Node = GetNode<Area2D>("Paddle2");
		
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
		// balle
		var (ballX, ballY) = _gameState.GetBallPosition();
		_ballNode.Position = new Vector2(ballX, ballY);

		// raquettes
		var (p1Y, p2Y) = _gameState.GetPaddlesYPosition();
		_paddle1Node.Position = new Vector2(_paddle1Node.Position.X, p1Y);
		_paddle2Node.Position = new Vector2(_paddle2Node.Position.X, p2Y);
	}
}
