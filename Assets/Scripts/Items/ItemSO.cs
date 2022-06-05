using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string id;
    public string itemName;
    public GameObject prefab;
    public Sprite icon;

    public GameObject Spawn(Vector3 position)
    {
        // Snap to grid

        GridBuildingSystem.instance.buildingGrid.GetXY(position, out int x, out int y);

        position = GridBuildingSystem.instance.buildingGrid.GetWorldPosition(x, y);

        GameObject item = Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        item.name = name;
        item.GetComponent<ItemObject>().itemSO = this;

        return item;
    }
}
