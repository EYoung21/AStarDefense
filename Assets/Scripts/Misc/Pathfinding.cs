using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance {  get; private set; }

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g,x,y));
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.calculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.calculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return (CalculatePath(endNode));
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) continue;
                if (!neighborNode.isWalkable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);

                if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.calculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }
        return null;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);

        PathNode currentNode = endNode;

        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        //left
        if ((currentNode.GetX() - 1) >= 0)
        {
            neighborList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));
        }
        
        //right
        if (currentNode.GetX() + 1 < grid.GetWidth())
        {
            neighborList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
        }
        
        //down
        if (currentNode.GetY() - 1 >= 0)
        {
            neighborList.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));
        }
        
        //up
        if (currentNode.GetY() + 1 < grid.GetHeight())
        {
            neighborList.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));
        }

        return neighborList;
    }

    private PathNode GetNode(int x, int y)
    {
        return(grid.GetGridObject(x, y));
    }

    


    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        
        //manhattan distance (no diagonals)
        return MOVE_STRAIGHT_COST * (xDistance + yDistance);
    }

    private PathNode GetLowestFCostNode(List<PathNode> list) {
        PathNode lowestFCostNode = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = list[i];
            }
        }
        return lowestFCostNode;
    }
    
    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        grid.GetXY(startPos, out int startX, out int startY);
        grid.GetXY(endPos, out int endX, out int endY);
        
        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null) return null;
        
        List<Vector3> vectorPath = new List<Vector3>();
        foreach (PathNode pathNode in path)
        {
            vectorPath.Add(new Vector3(pathNode.GetX(), pathNode.GetY()));
        }
        return vectorPath;
    }

    public List<Vector3> FindPathToCenter(Vector3 startPos)
    {
        //calculate center position
        int centerX = Mathf.FloorToInt(grid.GetWidth() / 2);
        int centerY = Mathf.FloorToInt(grid.GetHeight() / 2);
        Vector3 centerPos = new Vector3(centerX, centerY);
        
        return FindPath(startPos, centerPos);
    }

    //method to set unwalkable nodes (for blocks)
    public void SetIsWalkable(int x, int y, bool isWalkable)
    {
        if (x >= 0 && y >= 0 && x < grid.GetWidth() && y < grid.GetHeight())
        {
            grid.GetGridObject(x, y).SetIsWalkable(isWalkable);
        }
    }

    public void SetIsWalkable(Vector3 worldPosition, bool isWalkable)
    {
        grid.GetXY(worldPosition, out int x, out int y);
        SetIsWalkable(x, y, isWalkable);
    }
}
