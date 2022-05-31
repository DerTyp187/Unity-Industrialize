using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Camera cam;

    bool demolishMode = false;
    bool movingMode = false;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        // demolish
        if (Input.GetButtonDown("Demolish"))
        {
            Debug.Log("Demolish");
            demolishMode = !demolishMode;
            movingMode = false;
        }

        if (demolishMode && Input.GetMouseButton(0) && MenuManager.AllMenusClosed())
        {
            Vector3 postion = cam.ScreenToWorldPoint(Input.mousePosition);
            GridBuildingSystem.instance.DemolishPlacedObjectTypeSO(postion);
        }

        // moving   
        if (Input.GetButtonDown("Move"))
        {
            Debug.Log("Move");
            movingMode = !movingMode;
            demolishMode = false;
        }

        if (movingMode && Input.GetMouseButtonDown(0) && MenuManager.AllMenusClosed())
        {
            Debug.Log("Moving this position");
            Vector3 postion = cam.ScreenToWorldPoint(Input.mousePosition);
            GridBuildingSystem.instance.SelectMovingPlacedObject(postion);
            movingMode = false;
        }
    }
}
