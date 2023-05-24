using System;
using Godot;

public class SaveManager{
    Player player;
    public SaveManager(Player player){
        this.player = player;
    }

    public void LoadGame(){
        var saveGame = new File();
        if (!saveGame.FileExists("user://sketchBall.save")) return;

        saveGame.Open("user://sketchBall.save", File.ModeFlags.Read);

        while (saveGame.GetPosition() < saveGame.GetLen())
        {
            var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);

            player.ScoreManager.LastScore = int.Parse(nodeData["lastScore"].ToString());
            player.ScoreManager.HightScore = int.Parse(nodeData["hightScore"].ToString());
            if(nodeData.Keys.Contains("mute")){
                player.GameManager.AudioManager.IsMute = bool.Parse(nodeData["mute"].ToString());
            }
        }

        saveGame.Close();
    }
    public void SaveGame(){
        File saveGame = new File();
        saveGame.Open("user://sketchBall.save", File.ModeFlags.Write);

        Godot.Collections.Dictionary<string, object> data = new Godot.Collections.Dictionary<string, object>(){
            {"lastScore", player.ScoreManager.LastScore},
            {"hightScore", player.ScoreManager.HightScore},
            {"mute", player.GameManager.AudioManager.IsMute}
        };

        saveGame.StoreLine(JSON.Print(data));
        saveGame.Close();
    }
}