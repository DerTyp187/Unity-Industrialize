using UnityEngine;

public class Menu : MonoBehaviour
{
    public string id;
    public bool isOpen = false;

    public void Open()
    {
        MenuManager.CloseAllMenus();
        gameObject.SetActive(true);
        isOpen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        isOpen = false;
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}
