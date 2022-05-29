using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    float speed = 150f;

    [SerializeField]
    float fastSpeed = 300f;

    [SerializeField]
    float slowSpeed = 50f;

    [Header("Zoom")]
    [SerializeField]
    float maxCameraSize = 100f;

    [SerializeField]
    float minCameraSize = 3f;

    [SerializeField]
    float cameraSizeSteps = 3f;

    Vector2 cameraMovement;
    float currentSpeed = 0.0f;

    bool drag = false;

    Vector3 origin;
    Vector3 difference;



    Camera cam;
    Rigidbody2D rb;

    void Start()
    {
        cam = GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            difference = (cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position);
            if (!drag)
            {
                drag = true;
                origin = cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            cam.transform.position = origin - difference;
        }
    }

    void Update()
    {
        currentSpeed = speed;

        cameraMovement.x = Input.GetAxis("Horizontal");
        cameraMovement.y = Input.GetAxis("Vertical");

        if (Input.GetButton("CameraFast"))
            currentSpeed = fastSpeed;
        else if (Input.GetButton("CameraSlow"))
            currentSpeed = slowSpeed;


        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            ZoomCameraToPoint(cam.ScreenToWorldPoint(Input.mousePosition), cameraSizeSteps);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            ZoomCameraToPoint(cam.ScreenToWorldPoint(Input.mousePosition), -cameraSizeSteps);


    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + cameraMovement * currentSpeed * Time.fixedDeltaTime);
    }

    void ZoomCameraToPoint(Vector3 point, float amount)
    {
        float multiplier = (1.0f / cam.orthographicSize * amount);

        transform.position += (point - transform.position) * multiplier;

        cam.orthographicSize -= amount;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minCameraSize, maxCameraSize);
    }
}