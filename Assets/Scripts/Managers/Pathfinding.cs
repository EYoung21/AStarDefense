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

    private  List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode > neighborList = new List<PathNode>();

        if ((currentNode.GetX() - 1) >= 0)
        {
            // Left
            neighborList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));

            // Left Down
            if (currentNode.GetY() - 1 >= 0)
            {
                neighborList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() - 1));
            }
            // Left Up
            if (currentNode.GetY() + 1 < grid.GetHeight())
            {  
                neighborList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() + 1));
            }
        }
        // Right
        if (currentNode.GetX() + 1 < grid.GetWidth())
        {
            neighborList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
            // Right Down
            if (currentNode.GetY() - 1 >= 0)
            {
                neighborList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY() - 1));
            }
            // Right Up
            if (currentNode.GetY() + 1 < grid.GetHeight())
            {
                neighborList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY() + 1));
            }
        }
        // Down
        if (currentNode.GetY() - 1 >= 0)
        {
            neighborList.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));
        }
        // Up
        if (currentNode.GetY() + 1 < grid.GetHeight())
        {
            neighborList.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));
        }

        return(neighborList);
    }

    private PathNode GetNode(int x, int y)
    {
        return(grid.GetGridObject(x, y));
    }

    


    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetX());
        int remaining = Mathf.Abs(xDistance - yDistance);

        return (MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining);
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
    
}
