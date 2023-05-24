using Godot;
using System;

public delegate void UIButtonCallback();

public class UIButton : Node2D
{
    [Export]
    public Texture Image;

    public UIButtonCallback Callback{
        get;
        set;
    }
    
    TouchScreenButton button;
    public TouchScreenButton Button{
        get{return button;}
    }

    public GameManager GameManager;
    public Sprite Sprite;

    public override void _Ready()
    {
        if(Image != null){
            Sprite = GetNode<Sprite>("Image");
            Sprite.Texture  = Image;
        }
        
        
        button = GetNode<TouchScreenButton>("Button");

        button.Connect("pressed", this, nameof(OnButtonTouched));
    }

    public void OnButtonTouched(){
        Callback();
        if(GameManager != null) GameManager.AudioManager.PlayButtonPressed();
    }
}
