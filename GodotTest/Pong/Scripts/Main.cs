using Godot;
using System;
using GameCore;

public partial class Main : Node2D
{
	//GameCore
	private GameState _gameState;
	
	//Références aux noeuds visuels
	private Area2D ballNode;
	private Area2D paddle1Node;
	private Area2D paddle2Node;

	public override void _Ready()
	{
		// initialisation du GameCore
		_gameState = new GameState();
		
		// récupérer les nœuds de la scène
		ballNode = GetNode<Area2D>("Ball");
		paddle1Node = GetNode<Area2D>("Paddle1");
		paddle2Node = GetNode<Area2D>("Paddle2");
	}
	
	public override void _Process(float delta)
	{
		float deltaTime = delta;

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
		ballNode.Position = new Vector2(ballX, ballY);

		// raquettes
		var (p1Y, p2Y) = _gameState.GetPaddlePositions();
		paddle1Node.Position = new Vector2(paddle1Node.Position.x, p1Y);
		paddle2Node.Position = new Vector2(paddle2Node.Position.x, p2Y);
	}
}
