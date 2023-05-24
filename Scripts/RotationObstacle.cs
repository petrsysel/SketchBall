using Godot;
using System;

public class RotationObstacle : Node2D
{
    private float angle;
    float speed;
    Vector2 CenterVector;
    Sprite Sprite;
    public override void _Ready()
    {
        GD.Randomize();
        CenterVector = Position;
        angle = 0f;
        speed = (float)GD.RandRange(0.5f, 1.2f);
        Sprite = GetNode<Sprite>("Sprite");
        Sprite.Frame = (int)GD.RandRange(0, Sprite.Hframes);
    }

    public override void _Process(float delta)
    {
        angle += speed*delta; 
        Vector2 rotationVector = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        Position = CenterVector + (rotationVector*140);
    }
}
