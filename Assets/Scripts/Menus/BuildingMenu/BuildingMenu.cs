using System.Collections.Generic;
using UnityEngine;
public class BuildingMenu : Menu
{

    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemPrefab;

    private void Start()
    {
        MenuDictionary.instance.Register(this);
        CreateItemList();
    }

    private void CreateItemList()
    {
        List<PlacedObjectTypeSO> placedObectTypeSOList = PlacedObjectsDictionary.instance.entries;

        foreach (PlacedObjectTypeSO p in placedObectTypeSOList)
        {
            GameObject item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, itemListParent);
        }
    }

}
