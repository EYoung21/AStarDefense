using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height)
    {
        grid = new Grid<PathNode>(width, height, 1f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g,x,y));
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {

    }
    
}
