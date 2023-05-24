using System;
using Godot;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator{
    Cell[,] latestMaze;

    public MazeGenerator(){
        GD.Randomize();
    }

    public Cell[,] generateMaze(int w, int h){
        latestMaze = new Cell[h*2+1,w*2+1];
        prepareMaze();
        prepareWalls();
        /*
        1 - wall
        0 - unvisited cell
        2 - visited cell
        3 - removed wall
        */
        
        Stack<Cell> cellStack = new Stack<Cell>();

        Cell initCell = PickRandomCell();
        initCell.Value = 2;
        cellStack.Push(initCell);

        while(cellStack.Count > 0){
            Cell current = cellStack.Pop();
            List<Cell> unvisitedNeigboars = current.GetNeigboars(0);

            if(unvisitedNeigboars.Count > 0){
                cellStack.Push(current);
                int randIndex = (int)Math.Floor(GD.Randf()*(unvisitedNeigboars.Count));
                Cell chosen = unvisitedNeigboars[randIndex];

                current.RemoveWall(chosen); //  Potentionaly problematic
                chosen.Value = 2;
                cellStack.Push(chosen);
            }
        }

        

        createGates();

        pretify();
        createHoles(12);

        return latestMaze;
    }

    private void createHoles(int count){
        int k = count;
        // 9 - hole 
        while(k > 0){
            // int i = (int)GD.RandRange(0, latestMaze.GetLength(0) - 1);
            // int j = (int)GD.RandRange(0, latestMaze.GetLength(1) - 1);
            // Cell c = latestMaze[i,j];

            Cell c = PickRandomCell();
            if(c.Value == 0 && c.GetNeigboars(9).Count == 0){
                //if()
                c.Value = 9;
                k--;
            }
        }
        
    }

    private void createGates(){
        int j = 5;
        int i = 0;
        latestMaze[i,j].Value = 0;
        j = 5;
        i = latestMaze.GetLength(0) - 1;
        latestMaze[i,j].Value = 0;
    }

    private void pretify(){
        for(int i = 0; i  < latestMaze.GetLength(0); i++){
            for(int j = 0; j < latestMaze.GetLength(1); j++){
                int value = latestMaze[i,j].Value;
                if(value != 1) latestMaze[i,j].Value = 0;
            }
        }
    }

    private void prepareMaze(){
        for(int i = 0; i  < latestMaze.GetLength(0); i++){
            for(int j = 0; j < latestMaze.GetLength(1); j++){
                latestMaze[i,j] = new Cell(i,j,latestMaze);
            }
        }
    }
    private void prepareWalls(){
        for(int i = 0; i  < latestMaze.GetLength(0); i++){
            for(int j = 0; j < latestMaze.GetLength(1); j++){
                if(i%2 == 0 || j%2 == 0) latestMaze[i,j].Value = 1;
            }
        }
    }

    public void Print(){
        for(int i = 0; i  < latestMaze.GetLength(0); i++){
            string row = "";
            for(int j = 0; j < latestMaze.GetLength(1); j++){
                row += latestMaze[i,j].Value + " ";
            }
        }
    }

    private Cell PickRandomCell(){
        int i = (int)Math.Floor(GD.RandRange(0f, (latestMaze.GetLength(0)-3f)/2));
        i *= 2;
        i++;
        int j = (int)Math.Floor(GD.RandRange(0f, (latestMaze.GetLength(1)-3f)/2));
        j *= 2;
        j++;
        Cell c = latestMaze[i,j];
        return c;
    }
}

public class Cell{
    public int i;
    public int j;
    private int val;
    public int Value{
        get{
            return val;
        }
        set{
            this.val = value;
        }
    }
    private Cell[,] maze;

    public Cell(int i, int j, Cell[,] maze){
        this.i = i;
        this.j = j;
        this.val = 0;
        this.maze = maze;
    }

    public List<Cell> GetNeigboars(int filterValue){
        List<Cell> neighboars = new List<Cell>();

        if(i != 1) neighboars.Add(maze[i-2, j]);
        if(i != maze.GetLength(0)-2) neighboars.Add(maze[i+2, j]);
        if(j != 1) neighboars.Add(maze[i, j-2]);
        if(j != maze.GetLength(1)-2) neighboars.Add(maze[i, j+2]);

        List<Cell> filtred = new List<Cell>();
        foreach(Cell c in neighboars){
            if(c.Value == filterValue) filtred.Add(c);
        }
        
        return filtred;
    }

    public bool RemoveWall(Cell cell){
        int iDirection = cell.i - i;
        int jDirection = cell.j - j;
        if(Math.Abs(iDirection + jDirection) != 2) return false;

        if(iDirection == 0){
            maze[i, j + (jDirection/2)].Value = 3;
        }
        else{
            maze[i + (iDirection/2), j].Value = 3;
        }
        return true;
    }
}