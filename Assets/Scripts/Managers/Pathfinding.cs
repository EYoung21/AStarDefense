using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height)
    {
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

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList);

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
