using Godot;
using System;

public class RectangleObstacle : Node2D
{
    float speed;
    int direction;
    Vector2 MovementVector;
    Sprite Sprite;

    public override void _Ready()
    {
        GD.Randomize();
        speed = 200;
        direction = -1;
        MovementVector = new Vector2();
        Sprite = GetNode<Sprite>("Sprite");
        Sprite.Frame = (int)GD.RandRange(0, Sprite.Hframes);
        speed = (int)GD.RandRange(80, 200);
    }

    public override void _Process(float delta)
    {
        MovementVector = Position;
        MovementVector.x += speed * direction * delta;
        if(MovementVector.x > 320 || MovementVector.x < 40){
            direction *= -1;
        }
        else{
            Position = MovementVector;
        }
        
    }
}
