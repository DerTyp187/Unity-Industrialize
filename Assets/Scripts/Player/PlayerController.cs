using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Camera cam;

    bool demolishMode = false;

    private void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Demolish"))
        {
            demolishMode = !demolishMode;
        }

        if (demolishMode && Input.GetMouseButton(0) && MenuManager.AllMenusClosed())
        {
            Vector3 postion = cam.ScreenToWorldPoint(Input.mousePosition);
            GridBuildingSystem.instance.DemolishPlacedObjectTypeSO(postion);
        }
    }
}
