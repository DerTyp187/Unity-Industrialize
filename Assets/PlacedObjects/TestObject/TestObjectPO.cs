using UnityEngine;

public class TestObjectPO : PlacedObject
{
    public override void OnPlace()
    {
        Debug.Log("Placed Object: " + placedObjectTypeSO.name);
    }
}
