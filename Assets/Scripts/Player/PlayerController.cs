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
            ToggleDemolishMode();
        }

        if (demolishMode && Input.GetMouseButton(0) && MenuManager.AllMenusClosed())
        {
            Vector3 postion = cam.ScreenToWorldPoint(Input.mousePosition);
            GridBuildingSystem.instance.DemolishPlacedObjectTypeSO(postion);
        }

        // moving   
        if (Input.GetButtonDown("Move"))
        {
            ToggleMovingMode();
        }

        if (movingMode && Input.GetMouseButtonDown(0) && MenuManager.AllMenusClosed())
        {
            Debug.Log("Moving this position");
            Vector3 postion = cam.ScreenToWorldPoint(Input.mousePosition);
            GridBuildingSystem.instance.SelectMovingPlacedObject(postion);
            movingMode = false;
        }
    }

    public void ToggleDemolishMode()
    {
        Debug.Log("Demolish");
        demolishMode = !demolishMode;
        movingMode = false;
    }

    public void ToggleMovingMode()
    {
        Debug.Log("Move");
        movingMode = !movingMode;
        demolishMode = false;
    }
}
