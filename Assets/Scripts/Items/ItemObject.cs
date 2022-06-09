using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    public int maxStackSize = 5;
    public int stackSize = 1;
    public ItemSO itemSO;
    [SerializeField] private float timeAlive = 0.0f;
    [SerializeField] private float maxTimeAlive = 600.0f;
    [SerializeField] private float maxTimeWithoutConveyor = 10.0f;
    [SerializeField] private float timeWithoutConveyor = 0.0f;

    [SerializeField] private ConveyorPO lastTouchedConveyor; // current conveyor but not set to null
    [SerializeField] private float conveyorSpeed = 0.0f;
    [SerializeField] private Vector2 conveyorDirection = Vector2.zero;
    [SerializeField] private ConveyorPO currentConveyor;

    [SerializeField] private IOPort ioPort = null;

    public abstract void OnSpawn();



    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Stack Item
        if (collision.gameObject.GetComponent<ItemObject>() != null)
        {
            ItemObject collisionItemObject = collision.gameObject.GetComponent<ItemObject>();
            int addedStackSize = stackSize + collisionItemObject.stackSize;

            if (collisionItemObject.timeAlive < timeAlive) // The other item stacks to me
            {
                if (addedStackSize <= maxStackSize)
                {
                    stackSize = addedStackSize;
                    collisionItemObject.Despawn();
                }
                else
                {
                    stackSize = maxStackSize;
                    collisionItemObject.stackSize = addedStackSize - maxStackSize;
                }
            }
        }

        // IOPort
        if (collision.gameObject.GetComponent<IOPort>() != null)
        {
            ioPort = collision.gameObject.GetComponent<IOPort>();
        }
    }

    // Gets the temporary pivot point based on the conveyor direction
    private Vector3 GetTempPivotPosition(Vector2 conveyorDirection)
    {
        Vector3 tempPivotPosition = transform.position;

        float bufferSpace = 0.1f;
        if (conveyorDirection == Vector2.up) // Up
        {
            tempPivotPosition = transform.position + new Vector3(transform.localScale.x / 2, bufferSpace, 0);
        }

        if (conveyorDirection == Vector2.right) // Right
        {
            tempPivotPosition = transform.position + new Vector3(bufferSpace, transform.localScale.y / 2, 0);
        }

        if (conveyorDirection == Vector2.down) // Down
        {
            tempPivotPosition = transform.position + new Vector3(transform.localScale.x / 2, transform.localScale.y - bufferSpace, 0);
        }

        if (conveyorDirection == Vector2.left) // Left
        {
            tempPivotPosition = transform.position + new Vector3(transform.localScale.x - bufferSpace, transform.localScale.y / 2, 0);
        }

        return tempPivotPosition;
    }

    void Update()
    {
        GridBuildingSystem gridBuildingSystem = GridBuildingSystem.instance;

        # region TimeAlive
        timeAlive += Time.deltaTime;
        if (timeAlive > maxTimeAlive)
        {
            Despawn();
        }
        #endregion

        #region ConveyorBehaviour and Movement
        // TODO Freeze rigibody rotations based on direction      

        //! DEBUG LINE TO SEE TEMP PIVOT POSITION
        Debug.DrawLine(GetTempPivotPosition(conveyorDirection), Vector3.zero, Color.red);

        gridBuildingSystem.buildingGrid.GetXY(GetTempPivotPosition(conveyorDirection), out int tempPivotX, out int tempPivotY);
        GridBuildingSystem.GridObject gridObject = gridBuildingSystem.buildingGrid.GetGridObject(tempPivotX, tempPivotY);

        if (gridObject.GetPlacedObject() != null) // If there is a placed object
        {
            if (gridObject.GetPlacedObject().gameObject.GetComponent<ConveyorPO>()) // If this placed object is conveyor
            {
                timeWithoutConveyor = 0.0f; // reset time without conveyor
                currentConveyor = gridObject.GetPlacedObject().gameObject.GetComponent<ConveyorPO>(); // set current conveyor to this conveyor
                lastTouchedConveyor = currentConveyor; // set last touched conveyor to this conveyor
                conveyorSpeed = currentConveyor.GetSpeed(); // set conveyor speed to this conveyor

                //* If direction changes
                if (conveyorDirection != currentConveyor.GetDirection())
                {
                    // Realign the item to the grid/conveyor
                    gridBuildingSystem.buildingGrid.GetXY(lastTouchedConveyor.gameObject.transform.position, out int x, out int y);
                    transform.position = gridBuildingSystem.buildingGrid.GetWorldPosition(x, y);
                    conveyorDirection = currentConveyor.GetDirection();
                }
            }
            else // Not on a conveyor
            {
                UpdateWithoutConveyor();
            }
        }
        else // Not even on a placed object -> not on a conveyor
        {
            UpdateWithoutConveyor();
        }
        #endregion
        #region IOPort
        if (ioPort != null)
        {
            if (ioPort.item == itemSO)
            {
                stackSize = ioPort.IncreaseItemCount(stackSize);
                if (stackSize == 0)
                {
                    Despawn();
                }
            }
        }

        #endregion
    }

    private void UpdateWithoutConveyor() // Handles the item when it is not on a conveyor
    {
        currentConveyor = null;
        timeWithoutConveyor += Time.deltaTime;
        if (timeWithoutConveyor > maxTimeWithoutConveyor)
        {
            Despawn();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + (conveyorDirection * conveyorSpeed) * Time.fixedDeltaTime);
    }
}
