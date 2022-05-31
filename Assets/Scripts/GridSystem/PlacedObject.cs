using System.Collections.Generic;
using UnityEngine;

public abstract class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placeObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.identity);

        PlacedObject placedObject = placeObjectTransform.GetComponent<PlacedObject>();
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;

        placedObject.OnPlace();
        placedObject.SetIsBlueprint(false);

        if (placedObjectTypeSO.isWalkable)
        {
            foreach (Vector2Int position in placedObject.GetGridPositionList())
            {
                Pathfinding.instance.GetNode(position.x, position.y).SetIsWalkable(true);
            }
        }

        return placedObject;
    }

    public PlacedObjectTypeSO placedObjectTypeSO;
    public Vector2Int origin;

    [SerializeField] private bool isBlueprint = true;

    public bool GetIsBlueprint() => isBlueprint;
    public void SetIsBlueprint(bool newIsBlueprint)
    {

        if (GetComponent<Collider2D>() != null)
        {
            if (newIsBlueprint)
            {
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                GetComponent<Collider2D>().enabled = true;
            }
        }

        isBlueprint = newIsBlueprint;
    }

    private void Awake()
    {
        SetIsBlueprint(true);
    }

    public abstract void OnPlace();

    public void Move(Vector2Int moveToPosition)
    {
        // Remove old isWalkabe
        if (placedObjectTypeSO.isWalkable)
        {
            foreach (Vector2Int position in GetGridPositionList())
            {
                Pathfinding.instance.GetNode(position.x, position.y).SetIsWalkable(false);
            }
        }
        origin = moveToPosition;
        gameObject.transform.position = GridBuildingSystem.instance.buildingGrid.GetWorldPosition(moveToPosition.x, moveToPosition.y);

        // add new isWalkable
        if (placedObjectTypeSO.isWalkable)
        {
            foreach (Vector2Int position in GetGridPositionList())
            {
                Pathfinding.instance.GetNode(position.x, position.y).SetIsWalkable(true);
            }
        }
    }


    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin);
    }

    public void DestroySelf()
    {
        if (placedObjectTypeSO.isWalkable)
        {
            foreach (Vector2Int position in GetGridPositionList())
            {
                Pathfinding.instance.GetNode(position.x, position.y).SetIsWalkable(false);
            }
        }
        Destroy(gameObject);
    }
}