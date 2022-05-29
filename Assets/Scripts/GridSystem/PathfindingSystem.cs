using UnityEngine;

public class PathfindingSystem : MonoBehaviour
{
    public static PathfindingSystem instance { get; private set; }
    public Pathfinding pathfinding;



    void Start()
    {
        instance = this;

        int gridWidth = GridBuildingSystem.instance.gridWidth;
        int gridHeight = GridBuildingSystem.instance.gridHeight;
        float cellSize = GridBuildingSystem.instance.cellSize;

        pathfinding = new Pathfinding(gridWidth, gridHeight, cellSize);
    }

    void Update()
    {
    }
}
