using Godot;
using System;

public class Level : StaticBody2D
{
    PackedScene lineScene;
    PackedScene holeScene;
    PackedScene rectangleObstacleScene;
    PackedScene rotationObstacleScene;
    MazeGenerator mazeGenerator;
    [Export]
    bool Disable  = false;

    private float yPosition = 0;
    public float YPosition{
        get{
            return yPosition;
        }
        set{
            yPosition = value;
        }
    }

    private float levelHeight;
    public float LevelHeight{
        get{return levelHeight;}
    }

    public override void _Ready()
    {
        lineScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Line.tscn");
        holeScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Hole.tscn");
        rectangleObstacleScene = ResourceLoader.Load<PackedScene>("res://Prefabs/RectangleObstacle.tscn");
        rotationObstacleScene = ResourceLoader.Load<PackedScene>("res://Prefabs/RotationObstacle.tscn");

        mazeGenerator = new MazeGenerator();
    }

    public float CreateMaze(float yPosition, int level){
        YPosition = yPosition;
        
        Cell[,] maze = mazeGenerator.generateMaze(5,20);

        float oneSize = (360f)/5f;
        levelHeight = oneSize * 20;

        if(!Disable) DrawMaze(maze);
        if(level > 2){
            CreateRectangleObstacle((float)GD.RandRange(yPosition, yPosition + levelHeight));
        }
        if(level > 3){
            CreateRotationObstacle((float)GD.RandRange(yPosition, yPosition + levelHeight));
        }

        return levelHeight;
    }
    public float CreateCoridor(float yPosition){
        YPosition = yPosition;
        Cell[,] startLevel = new Cell[3, 11];
        for(int i = 0; i < startLevel.GetLength(0); i++){
            for(int j = 0; j < startLevel.GetLength(1); j++){
                startLevel[i,j] = new Cell(i,j,startLevel);
                if(j == 0 || j == 10) startLevel[i,j].Value = 1;
            }
        }
        float oneSize = (360f)/5f;
        levelHeight = oneSize * 1;
        if(!Disable) DrawMaze(startLevel);
        return levelHeight;
    }
    public float CreateStart(float yPosition){
        YPosition = yPosition;
        Cell[,] startLevel = new Cell[11, 11];
        for(int i = 0; i < startLevel.GetLength(0); i++){
            for(int j = 0; j < startLevel.GetLength(1); j++){
                startLevel[i,j] = new Cell(i,j,startLevel);
                if(j == 0 || j == 10 || i == 0) startLevel[i,j].Value = 1;
            }
        }
        float oneSize = (360f)/5f;
        levelHeight = oneSize * 5;
        if(!Disable) DrawMaze(startLevel);
        return levelHeight;
    }

    private void CreateLine(Vector2 position, int lineType, bool horizontal){
        
        Line l = (Line)lineScene.Instance();

        l.LineType = lineType;
        l.Position = position;
        l.Horizontal = horizontal;
        l.Visible = true;

        this.AddChild(l);
    }

    private void CreateHole(int i, int j){
        float oneSize = (360f)/5f;
        float xPos = (int)Math.Floor((j)/2f) * oneSize;
        float yPos = (int)Math.Floor((i)/2f) * oneSize + YPosition;
        
        Hole hole = holeScene.Instance<Hole>();
        hole.Position = new Vector2(xPos + oneSize/2f, yPos + oneSize/2f);
        // hole.Scale = new Vector2(0.25f,0.25f);
        AddChild(hole);
    }
    private void CreateRectangleObstacle(float yPos){
        RectangleObstacle obs = rectangleObstacleScene.Instance<RectangleObstacle>();
        obs.Position = new Vector2(180, yPos);
        AddChild(obs);
    }
    private void CreateRotationObstacle(float yPos){
        RotationObstacle obs = rotationObstacleScene.Instance<RotationObstacle>();
        obs.Position = new Vector2(180, yPos);
        AddChild(obs);
    }

    private void DrawMaze(Cell[,] maze){
        float oneSize = (360f)/5f;

        for(int i = 0; i  < maze.GetLength(0); i++){
            int wallCount = 0;
            for(int j = 0; j < maze.GetLength(1); j++){
                if(maze[i,j].Value == 1) wallCount ++;
                if(maze[i,j].Value == 0 || j == maze.GetLength(1) - 1){
                    if(wallCount > 1){
                        int startIndex = (int)Math.Floor((j - wallCount + 1)/2f);
                        int wallType = (wallCount - 1)/2;

                        int yPos = (int)Math.Floor((i)/2f);
                        CreateLine(new Vector2(startIndex*oneSize, YPosition+yPos*oneSize), wallType, true);
                    }
                    wallCount = 0;
                }

                if(maze[i,j].Value == 9){
                    CreateHole(i,j);
                }
            }
        }
        
        for(int j = 0; j  < maze.GetLength(1); j++){
            int wallCount = 0;
            for(int i = 0; i < maze.GetLength(0); i++){
                if(maze[i,j].Value == 1) wallCount ++;
                if(maze[i,j].Value == 0 || i == maze.GetLength(0) - 1){
                    if(wallCount > 1){
                        int startIndex = (int)Math.Floor((i - wallCount + 1)/2f);
                        int wallType = (wallCount - 1)/2;

                        int xPos = (int)Math.Floor((j)/2f);

                        CreateLine(new Vector2(xPos*oneSize, YPosition+startIndex*oneSize), wallType, false);
                    }
                    wallCount = 0;
                }
            }
        }
    }
}
