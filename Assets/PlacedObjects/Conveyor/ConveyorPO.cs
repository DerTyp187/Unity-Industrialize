using System.Collections.Generic;
using UnityEngine;

public class ConveyorPO : PlacedObject
{
    public float speed = 0.3f;

    public ConveyorPO previousConveyor;
    public ConveyorPO nextConveyor;

    public List<Sprite> sprites = new List<Sprite>(); // 0 = Up, 1 = Right, 2 = Down, 3 = Left



    private List<ConveyorPO> GetConveyorsAround()
    {
        List<ConveyorPO> conveyors = new List<ConveyorPO>();
        GridBuildingSystem gridBuildingSystem = GridBuildingSystem.instance;

        List<GridBuildingSystem.GridObject> gridObjects = gridBuildingSystem.buildingGrid.GetGridObjectsAround(transform.position);

        foreach (GridBuildingSystem.GridObject gridObject in gridObjects)
        {
            if (gridObject.GetPlacedObject() != null)
            {
                PlacedObject placedObject = gridObject.GetPlacedObject();
                if (placedObject.gameObject.GetComponent<ConveyorPO>() != null)
                {
                    conveyors.Add(placedObject.gameObject.GetComponent<ConveyorPO>());
                }
            }
        }

        return conveyors;
    }

    private void SetConveyorChain()
    {
        ConveyorPO lastChoiceConveyor = null;
        foreach (ConveyorPO c in GetConveyorsAround())
        {
            if (c == nextConveyor || c == previousConveyor || c == this || c.previousConveyor == this || c.nextConveyor == this)
            {
                continue;
            }

            if (c.nextConveyor == null)
            {
                if (c.previousConveyor == null)
                {
                    lastChoiceConveyor = c;
                }
                else
                {
                    c.nextConveyor = this;
                    c.UpdateChain();
                    previousConveyor = c;
                    return;
                }
            }
        }

        if (lastChoiceConveyor != null)
        {
            lastChoiceConveyor.nextConveyor = this;
            lastChoiceConveyor.UpdateChain();
            previousConveyor = lastChoiceConveyor;
        }
    }

    public Vector2 GetDirection()
    {
        if (nextConveyor != null)
        {
            return (nextConveyor.transform.position - transform.position).normalized;
        }
        else if (previousConveyor != null)
        {
            return (previousConveyor.transform.position - transform.position).normalized * -1;
        }

        return Vector2.up;
    }

    public void SetSpriteToDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        else if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[3];
        }
        else if (direction.y > 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
        else if (direction.y < 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[2];
        }
    }

    public void UpdateChain()
    {
        SetConveyorChain();
        SetSpriteToDirection(GetDirection());

    }
    public override void OnPlace()
    {
        Debug.Log("Conveyor placed");
        UpdateChain();
    }
}
