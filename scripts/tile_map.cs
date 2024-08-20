using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class tile_map : TileMap
{
	SortedDictionary<string, Vector2I[]> pieces;
	public static string[] pieceTypes = { };
	int rotation;
	int tileId;
	SortedDictionary<string, Vector2I> pieceColors;
	int baseLayer;
	int activeLayer;
	Vector2I[] clockwiseRotationMatrix;
	public static Vector2I startingPostition;
	public static Vector2I currentPosition;
	float steps;
	float reqSteps;
	public static string[] currentPieceColors;
	public Vector2I[] placedPieceLocations;

	public override void _Ready()
	{
		tileId = 1;
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
		pieceColors = new()
		{
			{ "B", new(0, 0) },{ "R", new(1, 0) },{ "Y", new(2, 0) },{ "G", new(0, 1) } };


		clockwiseRotationMatrix = new Vector2I[] { new(0, -1), new(1, 0) };
		currentPosition = startingPostition;
		steps = 0;
		reqSteps = 0.75f;
		placedPieceLocations = Array.Empty<Vector2I>();
	}

	public override void _Process(double delta)
	{
		if (!TetrisOn.Instance.On)
		{
			return;
		}
		getCurrentPiece();
		if (Input.IsActionJustPressed("up"))
		{
			rotatePiece(1);
		}
		movePiece();
		if (steps > reqSteps)
		{
			steps = 0;
		}
		if (Input.IsActionPressed("down"))
		{
			steps += (float)delta * 5;
		}
		steps += (float)delta;
	}

	public void rotatePiece(int direction)
	{
		if (!canRotate())
		{
			return;
		}
		clearPiece();
		Vector2I[] piece = pieces[pieceTypes[0]];
		for (int i = 0; i < piece.Length; i++)
		{
			Vector2I cell = piece[i];
			Vector2I newCoordinates = clockwiseRotationMatrix[0] * cell.X + clockwiseRotationMatrix[1] * cell.Y;
			piece[i] = newCoordinates;
		}
		drawPiece();
	}

	public void movePiece()
	{
		Vector2I direction = new(0, 0);
		if (Input.IsActionJustPressed("right"))
		{
			direction += Vector2I.Right;
		}
		if (Input.IsActionJustPressed("left"))
		{
			direction += Vector2I.Left;

		}
		clearPiece();
		if (willStick(direction))
		{
			placePiece();
		}
		if (canMove(direction))
		{
			currentPosition += direction;
		}
		if (steps > reqSteps && canMove(Vector2I.Down))
		{
			currentPosition += Vector2I.Down;
		}
		else if (steps > reqSteps && !canMove(Vector2I.Down))
		{
			placePiece();
		}
		drawPiece();
	}

	public void clearPiece()
	{
		foreach (Vector2I cell in getCurrentPiece())
		{
			EraseCell(activeLayer, currentPosition + cell);
		}
	}
	public void removePiece()
	{
		pieceTypes = pieceTypes.Skip(1).ToArray();
		currentPieceColors = currentPieceColors.Skip(1).ToArray();

		if (pieceTypes.Length == 0)
		{
			TetrisOn.Instance.On = false;
		}
	}

	public void clearOldPieces()
	{
		foreach (Vector2I cell in placedPieceLocations)
		{
			EraseCell(activeLayer, currentPosition + cell);
		}
	}
	public void drawPiece()
	{
		Vector2I[] currentPiece = getCurrentPiece();
		for (int i = 0; i < currentPiece.Length; i++)
		{
			Vector2I cell = currentPiece[i];
			string currentCellColor = currentPieceColors[0][i].ToString();
			EraseCell(activeLayer, currentPosition + cell);
			SetCell(activeLayer, currentPosition + cell, tileId, pieceColors[currentCellColor]);
			placedPieceLocations.Append(currentPosition + cell);
		}
	}
	public void placePiece()
	{
		Vector2I[] currentPiece = getCurrentPiece();
		for (int i = 0; i < currentPiece.Length; i++)
		{
			Vector2I cell = currentPiece[i];
			string currentCellColor = currentPieceColors[0][i].ToString();
			EraseCell(activeLayer, currentPosition + cell);
			SetCell(baseLayer, currentPosition + cell, tileId, pieceColors[currentCellColor]);
			placedPieceLocations.Append(currentPosition + cell);
		}
		removePiece();
		currentPosition = startingPostition;
	}
	public bool canMove(Vector2I dir)
	{
		foreach (Vector2I cell in getCurrentPiece())
		{
			if (!isTileFree(currentPosition + cell + dir))
				return false;
		}
		return true;
	}
	public bool canRotate()
	{
		foreach (Vector2I cell in getCurrentPiece())
		{
			Vector2I newCoordinates = clockwiseRotationMatrix[0] * cell.X + clockwiseRotationMatrix[1] * cell.Y;
			if (!isTileFree(newCoordinates + currentPosition))
				return false;
		}
		return true;
	}
	public bool willStick(Vector2I dir)
	{
		for (int i = 0; i < getCurrentPiece().Length; i++)
		{

			string currentCellColor = currentPieceColors[0][i].ToString();
			if (!isTileFree(currentPosition + getCurrentPiece()[i] + dir) && (currentCellColor == "R" || currentCellColor == "B"))
				return true;
		}

		return false;
	}
	public bool isTileFree(Vector2I pos)
	{
		return GetCellSourceId(baseLayer, pos) == -1;
	}
	public Vector2I[] getCurrentPiece()
	{
		return pieces[pieceTypes[0]];
	}
}
