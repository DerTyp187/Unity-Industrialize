using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlacedObjectTypeSO")]
public class PlacedObjectTypeSO : ScriptableObject
{
    public enum PlacedObjectCategory
    {
        CONVEYOR,
        BUILDING
    }

    public string id;
    public string nameString;
    public Transform prefab;
    public int width;
    public int height;
    public bool isWalkable = false;
    public Sprite iconSprite;
    public PlacedObjectCategory placedObjectCategory;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridPositionList.Add(offset + new Vector2Int(x, y));
            }
        }
        return gridPositionList;
    }
}