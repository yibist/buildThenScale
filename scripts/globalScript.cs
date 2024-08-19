using Godot;
using System;
public partial class TetrisOn : Node
{
    public static TetrisOn Instance { get; set; }

    public bool On { get; set; } = false;

    public override void _Ready()
    {
        Instance = this;
    }
}