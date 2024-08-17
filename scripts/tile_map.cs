using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class tile_map : TileMap
{
	// pieces
	SortedDictionary<string, Vector2I[]> pieces;
	// pieces used for this level
	string[] pieceTypes;
	int rotation;
	int tileId;
	Vector2I[] pieceAtlases;
	int baseLayer;
	int activeLayer;
	Vector2I[] clockwiseRotationMatrix;
	Vector2I startPosition;
	Vector2I currentPosition;

	float steps;
	int reqSteps;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileId = 0;
		baseLayer = 0;
		activeLayer = 1;
		pieces = new()
		{
			{ "I", new Vector2I[] { new(-1, 0), new(0, 0), new(1, 0), new(2, 0) } },
			{ "J", new Vector2I[] { new(-1, 1), new(-1, 0), new(0, 0), new(1, 0) } },
			{ "L", new Vector2I[] { new(1, 1), new(-1, 0), new(0, 0), new(1, 0) } },
			{ "O", new Vector2I[] { new(0, 1), new(1, 1), new(0, 0), new(1, 0) } },
			{ "S", new Vector2I[] { new(0, 1), new(1, 1), new(-1, 0), new(0, 0) } },
			{ "T", new Vector2I[] { new(0, 1), new(-1, 0), new(0, 0), new(1, 0) } },
			{ "Z", new Vector2I[] { new(-1, 1), new(0, 1), new(0, 0), new(1, 0) } },
		};
		pieceAtlases = new Vector2I[] { new(0, 0), new(1, 0), new(2, 0) };
		clockwiseRotationMatrix = new Vector2I[] { new(0, -1), new(1, 0) };
		pieceTypes = new string[] { "T", "L" };
		startPosition = currentPosition = new Vector2I(5, -5);

		steps = 0;
		reqSteps = 1;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_up"))
		{
			rotatePiece(1);
		}

		movePiece();
		if (steps > reqSteps)
		{
			steps = 0;
		}
		steps += (float)delta;
	}


	public void rotatePiece(int direction)
	{
		clearPiece(pieces[pieceTypes[0]], currentPosition);
		Vector2I[] piece = pieces[pieceTypes[0]];
		for (int i = 0; i < piece.Length; i++)
		{
			Vector2I cell = piece[i];
			Vector2I newCoordinates = clockwiseRotationMatrix[0] * cell.X + clockwiseRotationMatrix[1] * cell.Y;
			piece[i] = newCoordinates;
		}
		drawPiece(pieceAtlases[0]);
	}

	public void movePiece()
	{
		Vector2I direction = new(0, 0);
		if (Input.IsActionJustPressed("ui_right"))
		{
			direction += new Vector2I(1, 0);
		}
		if (Input.IsActionJustPressed("ui_left"))
		{
			direction += new Vector2I(-1, 0);

		}
		clearPiece(pieces[pieceTypes[0]], currentPosition);
		currentPosition += direction;
		if (steps > reqSteps)
		{
			currentPosition += new Vector2I(0, 1);
		}
		drawPiece(pieceAtlases[0]);
	}

	public void clearPiece(Vector2I[] piece, Vector2I pos)
	{
		foreach (Vector2I cell in piece)
		{
			EraseCell(activeLayer, pos + cell);
		}
	}
	public void removePieve()
	{
		pieceTypes = pieceTypes.Skip(1).ToArray();
	}
	public void drawPiece(Vector2I atlas)
	{
		foreach (Vector2I cell in pieces[pieceTypes[0]])
		{
			SetCell(activeLayer, currentPosition + cell, tileId, atlas);
		}
	}
}
