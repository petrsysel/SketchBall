using Godot;
using System;

public class Hole : Node2D
{
    Hitbox rotor;
    Sprite holeTexture;
    public override void _Ready()
    {
        GD.Randomize();
        rotor = GetNode<Hitbox>("Rotor");
        holeTexture = rotor.GetNode<Sprite>("Holes");

        holeTexture.Frame = (int)GD.RandRange(0, holeTexture.Hframes);
        holeTexture.Rotate((float)GD.RandRange(0f, Math.PI*2f));

        rotor.Rotate((float)((int)(GD.RandRange(0, 4))*Math.PI/2f));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
