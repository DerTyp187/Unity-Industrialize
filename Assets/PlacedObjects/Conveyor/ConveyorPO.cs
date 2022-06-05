using System.Collections.Generic;
using UnityEngine;

public class ConveyorPO : PlacedObject
{
    [SerializeField] private float speed = 0.3f;

    public ConveyorPO previousConveyor;
    public ConveyorPO nextConveyor;

    public List<Sprite> sprites = new List<Sprite>(); // 0 = Up, 1 = Right, 2 = Down, 3 = Left


    public float GetSpeed()
    {
        if (nextConveyor != null)
        {
            return speed;
        }
        else
        {
            return 0;
        }
    }

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
        // TODO Last conveyor does not connect to first conveyor (circle)        
        List<ConveyorPO> possibleNextConveyors = GetConveyorsAround();
        List<ConveyorPO> possiblePreviousConveyors = GetConveyorsAround();

        if (previousConveyor != null && nextConveyor != null)
        {
            return;
        }

        foreach (ConveyorPO c in GetConveyorsAround())
        {
            if (c == nextConveyor || c == previousConveyor || c == this || c.previousConveyor == this || c.nextConveyor == this)
            {
                continue;
            }
            else
            {
                if (c.nextConveyor == null)
                {
                    if (nextConveyor != c)
                    {
                        possiblePreviousConveyors.Add(c);
                    }
                }

                if (c.previousConveyor == null)
                {
                    if (previousConveyor != c)
                    {
                        possibleNextConveyors.Add(c);
                    }
                }
            }
        }

        if (possiblePreviousConveyors.Count > 0 && previousConveyor == null)
        {
            Debug.Log("PREV " + possiblePreviousConveyors.Count);
            foreach (ConveyorPO c in possiblePreviousConveyors)
            {
                if (c.previousConveyor != this && c.nextConveyor == null)
                {
                    previousConveyor = c; // Maybe prioritize Conveyor which is already in a chain?
                    previousConveyor.nextConveyor = this;
                    previousConveyor.UpdateChain();
                    break;
                }

            }
        }

        if (possibleNextConveyors.Count > 0 && nextConveyor == null)
        {
            Debug.Log("Next " + possibleNextConveyors.Count);
            foreach (ConveyorPO c in possibleNextConveyors)
            {
                if (c.nextConveyor != this && c.previousConveyor == null)
                {
                    nextConveyor = c; // Maybe prioritize Conveyor which is already in a chain?
                    nextConveyor.previousConveyor = this;
                    nextConveyor.UpdateChain();
                    break;
                }
            }
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
