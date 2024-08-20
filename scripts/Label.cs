using Godot;
using System;

public partial class Label : Godot.Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void onEnterWand(CharacterBody2D player)
	{
		if (!player.IsInGroup("Player"))
		{
			return;
		}
		this.Visible = true;
	}
}
