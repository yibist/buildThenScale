using Godot;
using System;

public partial class book : Area2D
{
	// Called when the node enters the scene tree for the first time.
	private AnimatedSprite2D bookSprite;

	private CharacterBody2D player;
	[Export]
	public string[] currentPieceColors;
	[Export]
	public string[] pieces;
	[Export]
	public Vector2I startingPostition;
	tile_map tileMap;

	bool used = false;
	public override void _Ready()
	{
		bookSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		tileMap = GetNode<tile_map>("TileMap");
	}

	public void onBodyEntered(CharacterBody2D player)
	{
		if (!player.IsInGroup("Player") || used)
		{
			return;
		}
		used = true;
		bookSprite.Play("opening");

		//tile_map
		tile_map.currentPieceColors = currentPieceColors;
		tile_map.pieceTypes = pieces;
		tile_map.startingPostition = tile_map.currentPosition = startingPostition;


		TetrisOn.Instance.On = true;
	}
	public void onBodyExited(CharacterBody2D player)
	{
		if (!player.IsInGroup("Player"))
		{
			return;
		}
		bookSprite.Play("closing");
	}
}
