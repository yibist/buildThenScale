using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using Godot;

public partial class player : CharacterBody2D
{
	public const float Speed = 200.0f;
	public const float JumpVelocity = -400.0f;

	float coyoteTimeReq = 0.1f;
	float coyoteTime = 0;

	float bounceLimit = 0;
	float bounceLimitReq = 0.2f;
	private AnimatedSprite2D playerSprite;
	public override void _Ready()
	{
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		if (TetrisOn.Instance != null && TetrisOn.Instance.On)
		{
			return;
		}
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && (IsOnFloor() || coyoteTime < coyoteTimeReq))
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}
		if (IsOnFloor())
		{
			coyoteTime = 0;
		}
		else
		{
			coyoteTime += (float)delta;
		}
		if (Input.IsActionPressed("left") || Input.IsActionPressed("right") || Input.IsActionPressed("up") || Input.IsActionPressed("down"))
		{
			playerSprite.Play("move");
		}
		else
		{
			playerSprite.Play("idle");
		}
		if (direction.X > 0)
		{
			playerSprite.FlipH = true;
		}
		else if (direction.X < 0)
		{
			playerSprite.FlipH = false;
		}
		bounceLimit += (float)delta;
		Velocity = velocity;
		MoveAndSlide();
	}
	public void onArea2dBodyEntered(Rid rid, TileMap body, int bodyshapeindex, int localshapeindex)
	{
		Vector2 positionOfTile = body.GetCoordsForBodyRid(rid) * 16 + body.Position;

		Vector2 top = positionOfTile + new Vector2(8, 0);
		Vector2 bottom = positionOfTile + new Vector2(8, 16);
		Vector2 right = positionOfTile + new Vector2(16, 8);
		Vector2 left = positionOfTile + new Vector2(0, 8);

		List<Vector2> distances = new()
		{
			Position - top,
			Position - bottom,
			Position - right,
			Position - left,
		};
		float smallest = float.MaxValue;
		int small = 5;
		for (int i = 0; i < 4; i++)
		{
			Vector2 distance = distances[i];
			if (smallest > Math.Abs(distance.X) + Math.Abs(distance.Y))
			{
				small = i;
				smallest = Math.Abs(distance.X) + Math.Abs(distance.Y);
			}
		}
		foreach (Vector2 distance in distances)
		{

		}
		if (bounceLimit > bounceLimitReq)
		{
			var velocity = GetRealVelocity();
			if (small < 2)
			{
				velocity = new Vector2(velocity.X, velocity.Y * -2);
			}
			else
			{
				velocity = new Vector2(velocity.X * -2, velocity.Y);
			}
			bounceLimit = 0;

			if (velocity.Y > 600)
			{
				velocity.Y = 600;
			}
			if (velocity.Y < -600)
			{
				velocity.Y = -600;
			}
			if (velocity.X > 600)
			{
				velocity.X = 600;
			}
			if (velocity.X < -600)
			{
				velocity.X = -600;
			}
			Velocity = velocity;
		}
	}
}
