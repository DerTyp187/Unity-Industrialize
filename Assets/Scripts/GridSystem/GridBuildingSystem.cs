using System.Collections.Generic;
using UnityEngine;

// Written with https://www.youtube.com/watch?v=dulosHPl82A
public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem instance;
    public Grid<GridObject> buildingGrid;

    public int gridWidth;
    public int gridHeight;
    public float cellSize;

    PlacedObjectTypeSO selectedPlacedObjectTypeSO;
    Transform selectedGameObjectTransform;

    public List<PlacedObjectTypeSO> DEBUG_OBJS = new List<PlacedObjectTypeSO>();

    public class GridObject
    {
        int x, y;
        bool isAccessable; // if true, can be placed on -> To limit the building area in the future
        Grid<GridObject> grid;
        PlacedObject placedObject;
        public GridObject(Grid<GridObject> _grid, int _x, int _y, bool _isAccessable = true) // FOR DEBUG TRUE
        {
            grid = _grid;
            x = _x;
            y = _y;
            isAccessable = _isAccessable;
        }
        public void SetPlacedObject(PlacedObject newPlacedObject)
        {
            placedObject = newPlacedObject;
            Debug.Log("SetPlacedObject");
        }
        public PlacedObject GetPlacedObject() => placedObject;
        public void ClearPlacedObject() => placedObject = null;
        public void SetIsAccessable(bool _isAccessable) => isAccessable = _isAccessable;
        public void SwitchIsAccessable() => isAccessable = !isAccessable;
        public bool IsAccessable() => isAccessable;
        public bool CanBuild()
        {
            return placedObject == null && isAccessable;
        }
    }
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        buildingGrid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
    }



    void Update()
    {
        if (selectedGameObjectTransform != null)
        {
            UpdateSelectedGameObject();
        }

        // DEBUG
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBuilding(DEBUG_OBJS[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectBuilding(DEBUG_OBJS[1]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectBuilding(DEBUG_OBJS[2]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectBuilding(DEBUG_OBJS[3]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectBuilding(DEBUG_OBJS[4]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectBuilding(DEBUG_OBJS[5]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectBuilding(DEBUG_OBJS[6]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectBuilding(DEBUG_OBJS[7]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectBuilding(DEBUG_OBJS[8]);
        }

    }



    void UpdateSelectedGameObject()
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
            PlaceBuilding(selectedGameObjectTransform.position);
        }


        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectBuilding();
        }


    }

    public void DemolishBuilding(Vector3 position)
    {
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

    public GameObject PlaceBuilding(Vector3 position)
    {
        position = new Vector3(position.x, position.y);
        buildingGrid.GetXY(position, out int x, out int y);

        List<Vector2Int> gridPositionList = selectedPlacedObjectTypeSO.GetGridPositionList(new Vector2Int(x, y));

        // DEBUG
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            Debug.Log(gridPosition);
        }

        if (CanBuild(gridPositionList))
        {
            PlacedObject placedObject = PlacedObject.Create(buildingGrid.GetWorldPosition(x, y), new Vector2Int(x, y), selectedPlacedObjectTypeSO);

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

    public void SelectBuilding(PlacedObjectTypeSO placedObjectTypeSO)
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

    public void DeselectBuilding()
    {
        Destroy(selectedGameObjectTransform.gameObject);
        selectedPlacedObjectTypeSO = null;
        selectedGameObjectTransform = null;
    }


}
