using Godot;
using System;
using Godot.Collections;

public class UI : CanvasLayer
{
    Player player;
    UIButton buttonPause;
    UIButton buttonContinue;
    UIButton buttonShutDown;
    UIButton buttonVolume;
    UIButton buttonNewGame;

    private Vector2 position;
    private Vector2 targetPosition;
    private delegate void SlideAnimationCallback();
    private delegate void PerformSlideAnimation(float delta, SlideAnimationCallback callback);

    private PerformSlideAnimation SlideState;
    private SlideAnimationCallback SlideCallback;

    private float timeInterpolation;
    private float interpolationDuration;

    Label ScoreHolder;
    Label ScoreOverview;

    Texture VolumeTexture;
    Texture MuteTexture;

    public override void _Ready()
    {
        player = GetParent<Camera2D>().GetParent<Node2D>().GetNode<Player>("Player");

        buttonPause = GetNode<UIButton>("PauseButton");
        buttonPause.Callback = OnPauseButtonPressed;
        buttonPause.GameManager = player.GameManager;

        buttonContinue = GetNode<UIButton>("PlayButton");
        buttonContinue.Callback = OnContinueButtonPressed;
        buttonContinue.GameManager = player.GameManager;

        buttonShutDown = GetNode<UIButton>("OffButton");
        buttonShutDown.Callback = OnOffButtonPressed;
        buttonShutDown.GameManager = player.GameManager;

        buttonNewGame = GetNode<UIButton>("NewGameButton");
        buttonNewGame.Callback = OnNewGameButtonPressed;
        buttonNewGame.GameManager = player.GameManager;

        buttonVolume = GetNode<UIButton>("VolumeButton");
        buttonVolume.Callback = OnVolumeButtonPressed;
        buttonVolume.GameManager = player.GameManager;

        position = new Vector2(0, 0);
        Offset = position;
        targetPosition = position;

        
    
        // targetPosition.y = 650;
        SlideState = SlideIdle;

        interpolationDuration = 10;
        timeInterpolation = 0;

        ScoreHolder = GetNode<Label>("ScoreHolder");
        ScoreOverview = GetNode<Label>("ScoreOverviewHolder");

        VolumeTexture = ResourceLoader.Load<Texture>("res://Sprites/buttonVolume.png");
        MuteTexture = ResourceLoader.Load<Texture>("res://Sprites/buttonVolumeMute.png");
        SetVolumeTexture(player.GameManager.AudioManager.IsMute);
    }

    private void Slide(float delta, SlideAnimationCallback callback){
        float distance = position.DistanceTo(targetPosition);
        timeInterpolation += delta;

        if(distance > 0.2f){
            player.Label.Text = distance.ToString();
            // position = position.MoveToward(targetPosition, 400*delta);
            position = position.LinearInterpolate(targetPosition,timeInterpolation/interpolationDuration);
        }
        else{
            position = targetPosition;
            callback();
            SlideCallback = () => {};
            SlideState = SlideIdle;
        }
        
        Offset = position;
    }
    private void SlideIdle(float delta, SlideAnimationCallback callback){

    }

    private void UIShow(SlideAnimationCallback callback){
        ScoreOverview.Text = player.ScoreManager.GetScoreOverviewLabel();
        targetPosition = new Vector2(0,650);
        SlideCallback = callback;
        timeInterpolation = 0;
        SlideState = Slide;
    }
    private void UIHide(SlideAnimationCallback callback){
        targetPosition = new Vector2(0,0);
        SlideCallback = callback;
        timeInterpolation = 0;
        SlideState = Slide;
    }

    public override void _Process(float delta)
    {
        SlideState(delta, SlideCallback);
        
        ScoreHolder.Text = player.ScoreManager.GetScoreLabel();
    }

    public void OnPauseButtonPressed(){
        if(player.StateMachine.State is PlayerStateMenu) return;
        // Offset = new Vector2(0,650);
        player.ChangeState(player.StateMenu);
        
        UIShow(() => {

        });
    }

    public void OnContinueButtonPressed(){
        if(!(player.StateMachine.State is PlayerStateMenu)) return;
        UIHide(() => {
            // player.Label.Text = player.StateMachine.PreviousState.GetType().ToString();
            player.ChangeState(player.StateMachine.PreviousState);
        });
    }

    public void OnOffButtonPressed(){
        GetTree().Quit();
    }
    public void OnNewGameButtonPressed(){
        // player.Label.Text = player.StateMachine.State.GetType().ToString();
        if(!(player.StateMachine.State is PlayerStateMenu)) return;
        
        UIHide(() => {
            if(player.StateMachine.PreviousState == player.StateGame) player.ChangeState(player.StateDeath);
            else player.ChangeState(player.StateMachine.PreviousState);
            
        });
    }
    public void OnVolumeButtonPressed(){
        player.GameManager.AudioManager.IsMute = !player.GameManager.AudioManager.IsMute;
        SetVolumeTexture(player.GameManager.AudioManager.IsMute);
        player.SaveManager.SaveGame();
    }

    private void SetVolumeTexture(bool isMute){
        if(isMute){
            buttonVolume.Sprite.Texture = MuteTexture;
        }
        else buttonVolume.Sprite.Texture = VolumeTexture;
    }
}
