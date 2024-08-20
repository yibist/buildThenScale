using Godot;
using System;

public partial class portal : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public void onBodyEntered(CharacterBody2D player)
	{
		if (!player.IsInGroup("Player"))
		{
			return;
		}
		string currentSceneFile = GetTree().CurrentScene.SceneFilePath;
		currentSceneFile = currentSceneFile.Remove(currentSceneFile.Length - 5);
		int nextLevel = currentSceneFile[currentSceneFile.Length - 1].ToString().ToInt() + 1;
		string nextLevelPath = "res://levels/level" + nextLevel + ".tscn";
		GetTree().ChangeSceneToFile(nextLevelPath);
	}

}
