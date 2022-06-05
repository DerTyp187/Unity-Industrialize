using System.Collections.Generic;
using UnityEngine;

public class ConveyorPO : PlacedObject
{
    [SerializeField] private float speed = 0.3f;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>(); // 0 = Up, 1 = Right, 2 = Down, 3 = Left
    [SerializeField] private ConveyorPO previousConveyor;
    [SerializeField] private ConveyorPO nextConveyor;

    #region Getters & Setters
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

    public ConveyorPO GetPreviousConveyor()
    {
        return previousConveyor;
    }

    public ConveyorPO GetNextConveyor()
    {
        return nextConveyor;
    }

    public void SetPreviousConveyor(ConveyorPO newPreviousConveyor)
    {
        if (newPreviousConveyor != nextConveyor)
        {
            previousConveyor = newPreviousConveyor;
        }
        UpdateChain();
    }

    public void SetNextConveyor(ConveyorPO newNextConveyor)
    {
        if (newNextConveyor != previousConveyor)
        {
            nextConveyor = newNextConveyor;
        }
        UpdateChain();
    }
    #endregion


    // Gets all conveyors around this conveyor (UP RIGHT DOWN LEFT)
    private List<ConveyorPO> GetConveyorsAround()
    {
        List<ConveyorPO> conveyors = new List<ConveyorPO>();
        GridBuildingSystem gridBuildingSystem = GridBuildingSystem.instance;

        List<GridBuildingSystem.GridObject> gridObjects = gridBuildingSystem.buildingGrid.GetGridObjectsAround(transform.position);

        foreach (GridBuildingSystem.GridObject gridObject in gridObjects)
        {
            if (gridObject.GetPlacedObject() != null) // If there is a placed object
            {
                PlacedObject placedObject = gridObject.GetPlacedObject();
                if (placedObject.gameObject.GetComponent<ConveyorPO>() != null) // If this placed object is a conveyor
                {
                    conveyors.Add(placedObject.gameObject.GetComponent<ConveyorPO>());
                }
            }
        }
        return conveyors;
    }

    // Integrates this conveyor with the other conveyors around it
    private void SetConveyorChain()
    {
        List<ConveyorPO> possibleNextConveyors = GetConveyorsAround(); // Gets all possible next conveyors for this conveyor
        List<ConveyorPO> possiblePreviousConveyors = GetConveyorsAround(); // Gets all possible previous conveyors for this conveyor

        // If this conveyor is already fully integraded in the chain -> return
        if (previousConveyor != null && nextConveyor != null)
            return;

        // Sort all conveyors around
        foreach (ConveyorPO otherConveyor in GetConveyorsAround())
        {
            // Even if this gets checked in multiple palces, I just want to be sure ;)
            if (otherConveyor == nextConveyor || otherConveyor == previousConveyor || otherConveyor.GetPreviousConveyor() == this || otherConveyor.GetNextConveyor() == this)
            {
                continue;
            }
            else
            {
                if (otherConveyor.GetNextConveyor() == null) // If other conveyor is able to have a nextConveyor
                {
                    if (nextConveyor != otherConveyor) // If this conveyor is not already connected to the other conveyor
                    {
                        possiblePreviousConveyors.Add(otherConveyor);
                    }
                }

                if (otherConveyor.GetPreviousConveyor() == null) // If other conveyor is able to have a previousConveyor
                {
                    if (previousConveyor != otherConveyor) // If this conveyor is not already connected to the other conveyor
                    {
                        possibleNextConveyors.Add(otherConveyor);
                    }
                }
            }
        }

        // If there is a possible previous conveyor and this conveyor is able to have a previousConveyor
        if (possiblePreviousConveyors.Count > 0 && previousConveyor == null)
        {
            foreach (ConveyorPO otherConveyor in possiblePreviousConveyors)
            {
                if (otherConveyor.GetPreviousConveyor() != this && otherConveyor.GetNextConveyor() == null) // If other conveyor is not already connected to this conveyor
                {
                    previousConveyor = otherConveyor; // Maybe prioritize Conveyor which is already in a chain?
                    previousConveyor.SetNextConveyor(this);
                    break;
                }

            }
        }

        // If there is a possible next conveyor and this conveyor is able to have a nextConveyor
        if (possibleNextConveyors.Count > 0 && nextConveyor == null)
        {
            foreach (ConveyorPO otherConveyor in possibleNextConveyors)
            {
                if (otherConveyor.GetNextConveyor() != this && otherConveyor.GetPreviousConveyor() == null) // If other conveyor is not already connected to this conveyor
                {
                    nextConveyor = otherConveyor; // Maybe prioritize Conveyor which is already in a chain?
                    nextConveyor.SetPreviousConveyor(this);
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
