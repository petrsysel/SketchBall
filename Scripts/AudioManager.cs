using System;
using Godot;

public class AudioManager{
    GameManager Parent;
    AudioStreamPlayer AudioPlayer;
    AudioStream deathSound;
    AudioStream buttonPressSound;

    public bool IsMute;

    public AudioManager(GameManager parent){
        Parent = parent;
        AudioPlayer = Parent.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        deathSound = ResourceLoader.Load<AudioStream>("res://Sounds/error.wav");
        buttonPressSound = ResourceLoader.Load<AudioStream>("res://Sounds/playcard.wav");
        IsMute = false;
    }

    public void PlayDeath(){
        if(IsMute) return;
        AudioPlayer.Stream = deathSound;
        AudioPlayer.Play();
    }

    public void PlayButtonPressed(){
        if(IsMute) return;
        AudioPlayer.Stream = buttonPressSound;
        AudioPlayer.Play();
    }
}