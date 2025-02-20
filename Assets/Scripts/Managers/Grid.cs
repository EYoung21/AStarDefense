using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private int[,] gridArray;
    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridArray = new int[width, height];
    }

    private Vector3 GetWorldPositon(int x,  int y)
    {

    }
}
