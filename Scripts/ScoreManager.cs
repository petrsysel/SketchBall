using System;
using Godot;

public class ScoreManager{
    private int score;
    public int Score{
        get{return score;}
        set {
            score = value;
            if(score > HightScore){
                HightScore = score;
            }
        }
    }

    private int lastScore;
    public int LastScore{
        get{return lastScore;}
        set{
            lastScore = value;
        }
    }

    private int hightScore;
    public int HightScore{
        get{
            return hightScore;
        }
        set{
            hightScore = value;
        }
    }

    PackedScene scoreOverviewScene;
    Player player;


    public ScoreManager(Player player){
        this.player = player;
        scoreOverviewScene = ResourceLoader.Load<PackedScene>("res://Prefabs/ScoreOverviewHolder.tscn"); 
        Score = 0;
        LastScore = 0;
        HightScore = 0;
        
    }

    public void ResetScore(){
        LastScore = Score;
        Score = 0;
        SaveScore();
    }

    public Label CreateScoreOverviewHolder(Vector2 position){
        Label l = scoreOverviewScene.Instance<Label>();
        l.Align = Label.AlignEnum.Center;
        l.Valign = Label.VAlign.Center;
        l.SetPosition(position, false);

        return l;
    }

    private void LoadScore(){
        player.SaveManager.LoadGame();
    }

    public void SaveScore(){
        player.SaveManager.SaveGame();
    }

    public void IncreseScore(int score){
        if(score > Score){
            Score = score;
        }
    }
    public string GetScoreLabel(){
        return $"SCORE: {Score}";
    }
    public string GetLastScoreLabel(){
        return $"LAST SCORE: {LastScore}";
    }
    public string GetHightScoreLabel(){
        return $"HIGHT SCORE: {HightScore}";
    }
    public string GetScoreOverviewLabel(bool withScore = true){
        string score = withScore?GetScoreLabel()+"\n":"";
        string overview = $"{score}{GetLastScoreLabel()}\n{GetHightScoreLabel()}";
        return overview;
    }
}