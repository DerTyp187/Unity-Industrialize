using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string id;
    public string name;
    public GameObject prefab;
    public Sprite icon;

    public void Spawn(Vector3 position)
    {
        GameObject item = Instantiate(prefab, position, Quaternion.identity);
        item.name = name;
    }
}
