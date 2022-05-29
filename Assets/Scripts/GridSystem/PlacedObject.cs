using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
            foreach(Vector2Int position in placedObject.GetGridPositionList())
            {
                Pathfinding.Instance.GetNode(position.x, position.y).SetIsWalkable(true);
            }

        }


        return placedObject;
    }

    public PlacedObjectTypeSO placedObjectTypeSO;
    Vector2Int origin;

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
                Pathfinding.Instance.GetNode(position.x, position.y).SetIsWalkable(false);
            }
        }
        Destroy(gameObject);
    }
}