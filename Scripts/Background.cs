using Godot;
using System;

public class Background : Node2D
{
    Sprite Paper1;
    Sprite Paper2;
    float paperHeight;

    Camera2D camera;

    public override void _Ready()
    {
        Paper1 = GetNode<Sprite>("Paper1");
        Paper2 = GetNode<Sprite>("Paper2");

        camera = GetParent<Node2D>().GetNode<Camera2D>("Camera2D");

        paperHeight = Paper1.Texture.GetHeight()/2f;

        Paper1.Position = new Vector2(180, camera.Position.y);
        Paper2.Position = new Vector2(180, Paper1.Position.y - paperHeight);
    }

    public override void _Process(float delta)
    {
        float toPaper1 = camera.Position.y - Paper1.Position.y;
        float toPaper2 = camera.Position.y - Paper2.Position.y;

        if(Math.Abs(toPaper1) < Math.Abs(toPaper2)){
            if(toPaper1 < 0) Paper2.Position = new Vector2(180, Paper1.Position.y - paperHeight);
            else Paper2.Position = new Vector2(180, Paper1.Position.y + paperHeight);
        }
        else{
            if(toPaper2 < 0) Paper1.Position = new Vector2(180, Paper2.Position.y - paperHeight);
            else Paper1.Position = new Vector2(180, Paper2.Position.y + paperHeight);
        }
    }
}
