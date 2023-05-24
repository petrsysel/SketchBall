using Godot;
using System;
using System.Collections.Generic;

public class GameManager : Node2D
{
    Player player;
    Queue<Level> levels;

    public Queue<Level> Levels{
        get{return levels;}
    }

    PackedScene levelScene;

    public float YPosition = 0;

    AudioManager audioManager;
    public AudioManager AudioManager{
        get{return audioManager;}
    }

    public int Level{
        get;
        set;
    }

    public override void _Ready()
    {
        player = GetParent<Node2D>().GetNode<Player>("Player");
        levelScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Level.tscn");
        YPosition = 0f;
        levels = new Queue<Level>();

        audioManager = new AudioManager(this);

        CreateStart();
        Level = 1;
    }

    public void ResetLevel(){
        Level = 1;
    }
    public void IncreaseLevel(){
        Level += 1;
    }

    public void CreateStart(){
        AddStart();
        AddLevel();
    }

    public override void _Process(float delta)
    {
        if(player.StateMachine.State is PlayerStateGame){
            if(player.Position.y > YPosition - (360f/5f*20f)/2f){
                AddLevel();
                // AddCoridor();
            }
        }
        if(levels.Count > 5) RemoveLevel();
    }

    private void AddLevel(){
        IncreaseLevel();
        Level firstLevel = levelScene.Instance() as Level;
        this.AddChild(firstLevel);
        YPosition += firstLevel.CreateMaze(YPosition, Level);
        
        levels.Enqueue(firstLevel);
    }

    private void RemoveLevel(){
        Level l = levels.Dequeue();
        // this.RemoveChild(l);
        l.QueueFree();
    }

    private void AddCoridor(){
        Level firstLevel = levelScene.Instance() as Level;
        this.AddChild(firstLevel);
        YPosition += firstLevel.CreateCoridor(YPosition);
        
        levels.Enqueue(firstLevel);
    }
    private void AddStart(){
        // GD.Print(player.ScoreManager == null);
        Level firstLevel = levelScene.Instance() as Level;
        this.AddChild(firstLevel);
        YPosition += firstLevel.CreateStart(YPosition);
        
        levels.Enqueue(firstLevel);

        ResetLevel();
    }

    public void AddScoreOverviewHolder(Vector2 position){
        Label overviewHolder = player.ScoreManager.CreateScoreOverviewHolder(position);
        overviewHolder.Text = player.ScoreManager.GetScoreOverviewLabel(false);
        
        this.AddChild(overviewHolder);
    }
}
