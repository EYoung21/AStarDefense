using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];
    }

    private Vector3 GetWorldPositon(int x,  int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    public void SetValue(int x, int y, int value) 
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;

        }
    }
}
