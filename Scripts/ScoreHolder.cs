using Godot;
using System;

public class ScoreHolder : Label
{
    Player player;
    public override void _Ready()
    {
        player = GetParent<CanvasLayer>().GetParent<Camera2D>().GetParent<Node2D>().GetNode<Player>("Player");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        int score = player.ScoreManager.Score;
        Text = $" SCORE: {score}";
    }
}
