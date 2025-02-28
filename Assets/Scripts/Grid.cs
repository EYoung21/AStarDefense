using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    private int width;  
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPosition;
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        // Initialize Grid
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    private Vector3 GetWorldPositon(int x,  int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public int GetWidth() { return width; }

    public int GetHeight() { return height; }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }


    public void SetGridObject(int x, int y, TGridObject value) 
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });

        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x;
        int y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return (gridArray[x, y]);
        }
        else 
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x;
        int y;
        GetXY(worldPosition, out x, out y);

        return GetGridObject(x, y);
    }

    public override string ToString()
    {
        string result = "";
        for (int y = height - 1; y >= 0; y--) // Print from top to bottom
        {
            for (int x = 0; x < width; x++)
            {
                TGridObject value = gridArray[x, y];

                // Handle null values gracefully
                string cellValue = (value != null) ? value.ToString() : " . ";

                result += cellValue + " ";
            }
            result += "\n";
        }
        return result;
    }

}
