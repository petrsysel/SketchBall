using Godot;
using System;

public class Line : StaticBody2D
{
    Sprite sprite;
    [Export]
    public int LineType = 5;
    [Export]
    public bool Horizontal = true;

    CollisionShape2D collider;
    RectangleShape2D collisionRect;
    public override void _Ready()
    {
        sprite = GetNode<Sprite>("Lines");

        GD.Randomize();        
        
        sprite.Frame = (int) GD.RandRange(0,6);
        sprite.FlipH = GD.Randf() < 0.5f;
        sprite.RegionEnabled = true;
        float width = ((sprite.Texture.GetWidth()/20)*LineType);
        float height = sprite.Texture.GetHeight();
        sprite.RegionRect = new Rect2(0,0,width,height);
        //float sizeRatio = 360f / (sprite.Texture.GetWidth()/4f);
        //GD.Print(sizeRatio);
        sprite.Scale = new Vector2(1, 0.5f);
        

        collider = new CollisionShape2D();
        collisionRect = new RectangleShape2D();
        collisionRect.Extents = new Vector2(width/2, 2);
        
        collider.Shape = collisionRect;
        collider.Position = new Vector2(width/2, collider.Position.y);
        AddChild(collider);

        if(!Horizontal) Rotate((float)Math.PI/2);
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
     
//  }
}
