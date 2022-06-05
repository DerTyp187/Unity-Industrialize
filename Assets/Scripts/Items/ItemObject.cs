using UnityEngine;

public abstract class ItemObject : MonoBehaviour
{
    public int maxStackSize = 5;
    public int stackSize = 1;
    public ItemSO itemSO;
    public float timeAlive = 0.0f;
    public float maxTimeAlive = 60.0f;
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
    }

    void FixedUpdate()
    {
        float speed = 0.3f; // TODO Get speed from ConveyorBelt
        Vector2 direction = new Vector2(1f, 0f); // TODO Get direction from ConveyorBelt

        rb.MovePosition(rb.position + (direction * speed) * Time.fixedDeltaTime);
    }
}
