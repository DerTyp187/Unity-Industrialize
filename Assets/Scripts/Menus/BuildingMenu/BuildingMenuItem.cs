using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuItem : MonoBehaviour
{
    [SerializeField] PlacedObjectTypeSO placedObjectType;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;

    public void SelectPlacedObjectType()
    {
        MenuManager.CloseAllMenus();
        GridBuildingSystem.instance.SelectPlacedObjectTypeSO(placedObjectType);
    }

    private void UpdateItem()
    {
        nameText.text = placedObjectType.name;
        image.sprite = placedObjectType.iconSprite;
    }

    public void SetPlacedObjectType(PlacedObjectTypeSO newPlacedObjectType)
    {
        placedObjectType = newPlacedObjectType;
        UpdateItem();
    }

    public PlacedObjectTypeSO GetPlacedObjectType()
    {
        return placedObjectType;
    }
}
