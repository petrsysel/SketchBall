using Godot;
using System;

public class Player : KinematicBody2D
{
    Vector2 velocity;
    public Vector2 Velocity{
        get{return velocity;}
        set{velocity = value;}
    }
    Vector2 acceleration;
    public Vector2 Acceleration{
        get{return acceleration;}
        set{acceleration = value;}
    }
    public Sprite BallSprite{
        get;
        set;
    }
    public Sprite StartAnimation{
        get;
        set;
    }
    AnimationPlayer animPlayer;
    public AnimationPlayer AnimationPlayer{
        get{return animPlayer;}
        set{animPlayer = value;}
    }
    public Label Label;
    [Export]
    float MaxSpeed = 200;

    Vector2 startPosition;
    public Vector2 StartPosition{
        get{return startPosition;}
        set{startPosition = value;}
    }

    TouchScreenButton startButton;
    public TouchScreenButton StartButton{
        get{return startButton;}
        // set{startButton = value;}
    }

    private Area2D area;
    public Area2D Area{
        get{return area;}
        set{area = value;}
    }

    GameManager gameManager;
    public GameManager GameManager{
        get{return gameManager;}
        set{gameManager = value;}
    }

    // private PlayerStateMachine stateMachine;
    public PlayerStateMachine StateMachine{
        get;
        set;
    }
    public PlayerStateGame StateGame;
    public PlayerStateStartButton StateStartButton;
    public PlayerStateDeath StateDeath;
    public PlayerStatePrepareToStart StatePrepareToStart;
    public PlayerStateMenu StateMenu;

    public ScoreManager ScoreManager{
        get;
        set;
    }
    public SaveManager SaveManager{
        get;
        set;
    }

    public override void _Ready()
    {
        velocity = new Vector2(0,0);
        acceleration = new Vector2(0,0);
        
        ScoreManager = new ScoreManager(this);
        SaveManager = new SaveManager(this);
        
        // GD.Print((ScoreManager == null) + " constructor");

        Label =             GetNode<Label>("Label");
        BallSprite =        GetNode<Sprite>("BallSprite");
        StartAnimation =    GetNode<Sprite>("StartAnimation");
        animPlayer =        GetNode<AnimationPlayer>("AnimationPlayer");
        area =              GetNode<Area2D>("Area2D");
        startButton =       GetNode<TouchScreenButton>("StartButton");
        gameManager =       GetParent<Node2D>().GetNode<GameManager>("GameManager");
        
        

        StateGame = new PlayerStateGame(this);
        StateDeath = new PlayerStateDeath(this);
        StateStartButton = new PlayerStateStartButton(this);
        StatePrepareToStart = new PlayerStatePrepareToStart(this);
        StateMenu = new PlayerStateMenu(this);
        
        StateMachine = new PlayerStateMachine(this);
        ChangeState(StateStartButton);
        
        GameManager.AddScoreOverviewHolder(new Vector2(40,StartPosition.y));

        SaveManager.LoadGame();
    }

    public void ChangeState(IPlayerState state){
        if(StateMachine.State != StateMachine.PreviousState){
            StateMachine.PreviousState = StateMachine.State;    //  POZOR na pointer
        }
        
        StateMachine.State = state;
        StateMachine.State.OnStateChanged();
    }

    public override void _Process(float delta)
    {
        StateMachine.Update(delta);
        // Label.Text = gameManager.Level.ToString();
    }
}

public class PlayerStateMachine{
    Player player;

    public IPlayerState State{
        get;
        set;
    }
    private IPlayerState previousState;
    public IPlayerState PreviousState{
        get{
            return previousState;
        }
        set{
            previousState = value;
            // player.Label.Text = previousState.GetType().ToString();
        }
    }

    // private IPlayerState gameState;
    public IPlayerState GameState{
        get;
        set;
    }

    public PlayerStateMachine(Player player){
        this.player = player;
    }

    

    public void Update(float delta){
        State.Update(delta);
    }
}

public interface IPlayerState{
    Player Player{
        get;
        set;
    }

    void Update(float delta);
    void OnStateChanged();
}

public class PlayerStateGame : Godot.Object, IPlayerState{
    private Player player;
    public Player Player{
        get{return player;}
        set{player = value;}
    }
    public PlayerStateGame(Player player){
        this.player = player;
    }

    public void OnStateChanged(){
        player.Area.Connect("area_entered", this, nameof(OnAreaEntered));
        player.AnimationPlayer.Play("Roll");
    }

    public void Update(float delta){

        Vector3 gravity = Input.GetGravity()*1f;
        player.Acceleration = new Vector2(gravity.x, -gravity.y);
        
        player.Velocity += player.Acceleration*0.99f;
        player.Velocity *= 0.99f;
        
        float animSpeed = player.Velocity.Length()/100f;
        player.AnimationPlayer.PlaybackSpeed = animSpeed;

        player.Velocity = player.MoveAndSlide(player.Velocity);
        float oneSize = (360f)/5f;
        int score = (int)((player.Position.y - player.StartPosition.y)/oneSize);
        player.ScoreManager.IncreseScore(score);
    }

    public void OnAreaEntered(Area2D area)
    {
        if(area is Hitbox){
            if(player.StateMachine.State is PlayerStateGame)
            Player.ChangeState(player.StateDeath);
        }

    }
}
public class PlayerStateStartButton : Godot.Object, IPlayerState{
    private Player player;
    public Player Player{
        get{return player;}
        set{player = value;}
    }
    public PlayerStateStartButton(Player player){
        this.player = player;
    }

    public void OnStateChanged(){
        player.BallSprite.Visible = false;
        player.StartAnimation.Visible = true;
        player.StartAnimation.Frame = 0;
        player.StartAnimation.Scale = new Vector2(0.5f, 0.5f);

        

        player.AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
        player.StartButton.Connect("pressed", this, nameof(OnStartPressed));
        
        player.Velocity = Vector2.Zero;
        player.Acceleration = Vector2.Zero;

        //  POZOR
        // player.StateMachine.SetState(new PlayerStateDeath(player));
    }

    public void Update(float delta){

    }

    public void OnStartPressed(){
        if(player.StateMachine.State is PlayerStateStartButton){
            player.AnimationPlayer.PlaybackSpeed = 1;
            player.AnimationPlayer.Play("StartButton");
            player.GameManager.AudioManager.PlayButtonPressed();
        }
    }

    public void OnAnimationFinished(string animationName){
        if(animationName == "StartButton"){
            player.StartAnimation.Visible = false;
            player.BallSprite.Visible = true;
            player.ChangeState(player.StateGame);
        }
    }
}

public class PlayerStateDeath : Godot.Object, IPlayerState{
    private Player player;
    public Player Player{
        get{return player;}
        set{player = value;}
    }
    public PlayerStateDeath(Player player){
        this.player = player;
    }

    public void OnStateChanged(){
        // player.BallSprite.Visible = false;
        // player.StartAnimation.Visible = true;
        // player.StartAnimation.Frame = 0;
        // player.AnimationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
        // player.StartButton.Connect("pressed", this, nameof(OnStartPressed));
        

        //  TADY BUDE MOŽNÁ ANIMACE SMRTI
        //  POKUD SE MI BUDE CHTÍT

        player.ScoreManager.ResetScore();

        player.ChangeState(player.StatePrepareToStart);
        player.GameManager.AudioManager.PlayDeath();
    }

    public void Update(float delta){

    }

    public void OnStartPressed(){
        player.AnimationPlayer.Play("StartButton");
    }

    public void OnAnimationFinished(string animationName){
        if(animationName == "StartButton"){
            player.StartAnimation.Visible = false;
            player.BallSprite.Visible = true;
            player.ChangeState(new PlayerStateGame(player));
        }
    }
}

public class PlayerStatePrepareToStart : Godot.Object, IPlayerState{
    private Player player;
    public Player Player{
        get{return player;}
        set{player = value;}
    }
    public PlayerStatePrepareToStart(Player player){
        this.player = player;
    }

    public void OnStateChanged(){
        
        
        player.GameManager.YPosition += 180;
        player.StartPosition = new Vector2(180, player.GameManager.YPosition + 180);
        player.GameManager.CreateStart();
        player.GameManager.AddScoreOverviewHolder(new Vector2(40,player.StartPosition.y - 180));
        player.BallSprite.Visible = false;
    }

    public void Update(float delta){
        if(player.Position.DistanceTo(player.StartPosition) > 1200f*delta + 1f) player.Position = player.Position.MoveToward(player.StartPosition, 1200f*delta);
            else{
                player.Position = player.StartPosition;
                player.ChangeState(player.StateStartButton);
            } 
    }

    public void OnStartPressed(){
        player.AnimationPlayer.Play("StartButton");
    }

    public void OnAnimationFinished(string animationName){
        if(animationName == "StartButton"){
            player.StartAnimation.Visible = false;
            player.BallSprite.Visible = true;
            player.ChangeState(new PlayerStateGame(player));
        }
    }
}

public class PlayerStateMenu : Godot.Object, IPlayerState{
    private Player player;
    public Player Player{
        get{return player;}
        set{player = value;}
    }
    public PlayerStateMenu(Player player){
        this.player = player;
    }

    public void OnStateChanged(){
        
    }

    public void Update(float delta){
        
    }
}