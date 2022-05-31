using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem instance;
    public Grid<GridObject> buildingGrid;

    public int gridWidth;
    public int gridHeight;
    public float cellSize;

    [SerializeField] Color cannotBuildColor; // Maybe put this into every single PlacedObjectTypeSO for better customized use

    PlacedObjectTypeSO selectedPlacedObjectTypeSO;
    Transform selectedGameObjectTransform;

    // Conveyor Placing Variables
    Vector3 conveyorStartPosition;
    List<GameObject> placingConveyorBlueprints = new List<GameObject>();
    bool isPlacingConveyor = false;

    public class GridObject
    {
        int x, y;
        Grid<GridObject> grid;
        PlacedObject placedObject;
        public GridObject(Grid<GridObject> _grid, int _x, int _y) // FOR DEBUG TRUE
        {
            grid = _grid;
            x = _x;
            y = _y;
        }
        public void SetPlacedObject(PlacedObject newPlacedObject)
        {
            placedObject = newPlacedObject;
            Debug.Log("SetPlacedObject");
        }
        public PlacedObject GetPlacedObject() => placedObject;
        public void ClearPlacedObject() => placedObject = null;
        public bool CanBuild()
        {
            return placedObject == null;
        }
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        buildingGrid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }

    void Update()
    {
        if (selectedGameObjectTransform != null)
        {
            if (selectedPlacedObjectTypeSO.placedObjectCategory == PlacedObjectTypeSO.PlacedObjectCategory.BUILDING)
            {
                UpdateSelectedBuilding();
            }

            if (selectedPlacedObjectTypeSO.placedObjectCategory == PlacedObjectTypeSO.PlacedObjectCategory.CONVEYOR)
            {
                UpdateSelectedConveyor();
            }
        }
    }

    bool CanBuild(List<Vector2Int> gridPositionList)
    {
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!buildingGrid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                // Cannot build here
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    #region BuildingPlacing
    void UpdateSelectedBuilding()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        buildingGrid.GetXY(mousePosition, out int x, out int y);

        selectedGameObjectTransform.position = buildingGrid.GetWorldPosition(x, y);

        List<Vector2Int> gridPositionList = selectedPlacedObjectTypeSO.GetGridPositionList(new Vector2Int(x, y));

        if (CanBuild(gridPositionList))
        {
            selectedGameObjectTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            selectedGameObjectTransform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlacePlacedObjectTypeSO(selectedGameObjectTransform.position, selectedPlacedObjectTypeSO);
        }


        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectPlacedObjectTypeSO();
        }
    }
    #endregion

    #region ConveyorPlacing
    void UpdateSelectedConveyor()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        buildingGrid.GetXY(mousePosition, out int x, out int y);
        selectedGameObjectTransform.position = buildingGrid.GetWorldPosition(x, y);

        if (Input.GetMouseButtonDown(0))
        {
            if (isPlacingConveyor == false)
            {
                conveyorStartPosition = buildingGrid.GetWorldPosition(x, y);
                isPlacingConveyor = true;
            }
            else
            {
                if (ConveyorCanBuild())
                {
                    PlaceConveyorPath();
                    isPlacingConveyor = false;
                }
            }
        }

        DrawConveyorPath();

        if (ConveyorCanBuild())
        {
            RecolorConveyorPath(Color.white);
        }
        else
        {
            RecolorConveyorPath(cannotBuildColor);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ClearConveyorPath();
            DeselectPlacedObjectTypeSO();
        }
    }

    void PlaceConveyorPath()
    {
        if (placingConveyorBlueprints.Count > 0)
        {
            foreach (GameObject blueprint in placingConveyorBlueprints)
            {
                PlacePlacedObjectTypeSO(blueprint.transform.position, blueprint.gameObject.GetComponent<PlacedObject>().placedObjectTypeSO);
            }
            ClearConveyorPath();
        }
    }

    void ClearConveyorPath()
    {
        if (placingConveyorBlueprints.Count > 0)
        {
            foreach (GameObject blueprint in placingConveyorBlueprints)
            {
                Destroy(blueprint);
            }
            placingConveyorBlueprints.Clear();
        }
    }

    void DrawConveyorPath()
    {
        if (isPlacingConveyor == true)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            buildingGrid.GetXY(mousePosition, out int x, out int y);
            Vector3 endPosition = buildingGrid.GetWorldPosition(x, y);

            ClearConveyorPath();

            List<Vector2Int> pathPoints = VectorDrawing.FindVectorPath(conveyorStartPosition, endPosition);
            Debug.Log(pathPoints.Count);
            foreach (Vector2Int position in pathPoints)
            {
                Debug.Log("TEST");
                Debug.Log(position);
                GameObject conveyorBlueprint = Instantiate(selectedPlacedObjectTypeSO.prefab.gameObject, new Vector3(position.x, position.y), Quaternion.identity);
                placingConveyorBlueprints.Add(conveyorBlueprint);
            }

            /*
            foreach (Vector3 position in Pathfinding.instance.FindPath(conveyorStartPosition, endPosition, true))
            {
                buildingGrid.GetXY(position, out x, out y);

                GameObject conveyorBlueprint = Instantiate(selectedPlacedObjectTypeSO.prefab.gameObject, new Vector3(x, y), Quaternion.identity);
                placingConveyorBlueprints.Add(conveyorBlueprint);
            }*/

        }
    }

    void RecolorConveyorPath(Color color)
    {
        foreach (GameObject blueprint in placingConveyorBlueprints)
        {
            blueprint.GetComponent<SpriteRenderer>().color = color;
        }
    }
    bool ConveyorCanBuild()
    {
        bool canBuild = true;

        foreach (GameObject blueprint in placingConveyorBlueprints)
        {
            buildingGrid.GetXY(blueprint.transform.position, out int x, out int y);

            List<Vector2Int> gridPositionList = blueprint.GetComponent<PlacedObject>().placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, y));
            if (CanBuild(gridPositionList) == false)
            {
                canBuild = false;
                Debug.Log("Cannot build conveyor here");
            }
        }
        return canBuild;
    }
    #endregion

    #region PlacedObjectsTypeSO Methods
    public void DemolishPlacedObjectTypeSO(Vector3 position)
    {
        if (selectedPlacedObjectTypeSO != null)
            return;

        GridObject gridObject = buildingGrid.GetGridObject(position); // Camera.main.ScreenToWorldPoint(Input.mousePosition)
        PlacedObject placedObject = gridObject.GetPlacedObject();

        if (placedObject != null)
        {
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                buildingGrid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
        }
    }



    public GameObject PlacePlacedObjectTypeSO(Vector3 position, PlacedObjectTypeSO placedObjectTypeSO)
    {
        position = new Vector3(position.x, position.y);
        buildingGrid.GetXY(position, out int x, out int y);

        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, y));

        // DEBUG
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            Debug.Log(gridPosition);
        }

        if (CanBuild(gridPositionList))
        {
            PlacedObject placedObject = PlacedObject.Create(buildingGrid.GetWorldPosition(x, y), new Vector2Int(x, y), placedObjectTypeSO);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                buildingGrid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }
            return placedObject.gameObject;
        }
        else
        {
            Debug.Log("Cannot build here!" + " " + position);
        }
        return null;
    }

    public void SelectPlacedObjectTypeSO(PlacedObjectTypeSO placedObjectTypeSO)
    {

        // Delete all previous blueprints
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("PlacedObject"))
        {
            if (o.GetComponent<PlacedObject>().GetIsBlueprint())
            {
                Destroy(o);
            }
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectedGameObjectTransform = Instantiate(placedObjectTypeSO.prefab, mousePosition, Quaternion.identity);
        selectedPlacedObjectTypeSO = placedObjectTypeSO;
    }

    public void DeselectPlacedObjectTypeSO()
    {
        Destroy(selectedGameObjectTransform.gameObject);
        selectedPlacedObjectTypeSO = null;
        selectedGameObjectTransform = null;
        isPlacingConveyor = false;
        placingConveyorBlueprints.Clear();
    }
    #endregion
}
