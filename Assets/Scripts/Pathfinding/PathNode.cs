using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;

    public List<PathNode> neighbourList = new List<PathNode>();

    public PathNode(Grid<PathNode> _grid, int _x, int _y) {
        grid = _grid;
        x = _x;
        y = _y;
        isWalkable = false;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
    }

    public override string ToString() {
        return x + "," + y;
    }

}
