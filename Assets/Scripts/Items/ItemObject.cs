using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    public int maxStackSize = 5;
    public int stackSize = 1;
    public ItemSO itemSO;
    public float timeAlive = 0.0f;
    public float maxTimeAlive = 600.0f;
    public float maxTimeWithoutConveyor = 10.0f;
    public float timeWithoutConveyor = 0.0f;

    private ConveyorPO lastTouchedConveyor; // current conveyor but not set to null
    private float conveyorSpeed = 0.0f;
    private Vector2 conveyorDirection = Vector2.zero;

    public abstract void OnSpawn();

    private ConveyorPO currentConveyor;

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
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > maxTimeAlive)
        {
            Despawn();
        }

        // Problem: Pivot point of an item is bottom-left, so its not proberly aligned with the conveyor
        GridBuildingSystem gridBuildingSystem = GridBuildingSystem.instance;

        //* Position has to change depending on which direction the item is heading, in order to stay on the conveyor
        Vector3 tempPivotPosition = transform.position;

        if (currentConveyor != null)
        {
            // TODO Freeze rigibody rotations based on direction
            //* KEEP IN MIND -> PIVOT POINT IS BOTTOM-LEFT

            // If direction changes
            if (conveyorDirection != currentConveyor.GetDirection())
            {
                // Realign
                gridBuildingSystem.buildingGrid.GetXY(lastTouchedConveyor.gameObject.transform.position, out int x, out int y);
                transform.position = gridBuildingSystem.buildingGrid.GetWorldPosition(x, y);
                conveyorDirection = currentConveyor.GetDirection();
            }

            if (conveyorDirection.y > 0) // Top
            {
                Debug.Log("Heading top");
                tempPivotPosition = transform.position + new Vector3(transform.localScale.x / 2, 0, 0);
            }

            if (conveyorDirection.x > 0) // Right
            {
                Debug.Log("Heading right");
                tempPivotPosition = transform.position + new Vector3(0, transform.localScale.y / 2, 0);
            }

            if (conveyorDirection.y < 0) // Bottom
            {
                Debug.Log("Heading bottom");
                tempPivotPosition = transform.position + new Vector3(transform.localScale.x / 2, transform.localScale.y, 0);
            }

            if (conveyorDirection.x < 0) // Left
            {
                Debug.Log("Heading left");
                tempPivotPosition = transform.position + new Vector3(transform.localScale.x, transform.localScale.y / 2, 0);
            }
            Debug.DrawLine(tempPivotPosition, Vector3.zero, Color.red);
        }


        gridBuildingSystem.buildingGrid.GetXY(tempPivotPosition, out int tempPivotX, out int tempPivotY);

        GridBuildingSystem.GridObject gridObject = gridBuildingSystem.buildingGrid.GetGridObject(tempPivotX, tempPivotY);

        if (gridObject.GetPlacedObject() != null)
        {
            if (gridObject.GetPlacedObject().gameObject.GetComponent<ConveyorPO>())
            {
                timeWithoutConveyor = 0.0f;
                currentConveyor = gridObject.GetPlacedObject().gameObject.GetComponent<ConveyorPO>();
                lastTouchedConveyor = currentConveyor;
                conveyorSpeed = currentConveyor.GetSpeed();

            }
            else
            {
                UpdateWithoutConveyor();
            }
        }
        else
        {
            UpdateWithoutConveyor();
        }
    }

    private void UpdateWithoutConveyor()
    {
        currentConveyor = null;
        timeWithoutConveyor += Time.deltaTime;
        if (timeWithoutConveyor > maxTimeWithoutConveyor)
        {
            Debug.Log("Not on conveyor -> DESPAWN");
            Despawn();
        }
    }

    void FixedUpdate()
    {
        if (currentConveyor != null)
        {
            rb.MovePosition(rb.position + (conveyorDirection * conveyorSpeed) * Time.fixedDeltaTime);
        }
        else
        {
            // TODO Interrupt all movement
        }

    }
}
