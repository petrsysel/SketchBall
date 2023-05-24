using Godot;
using System;

public class GyroTest : Label
{
    
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector3 gra = Input.GetGravity();
        string graLbl = $" Gravity x: {gra.x}, y: {gra.y}, z: {gra.z}";

        Text = $"{graLbl}";
    }   
}
