using UnityEngine;
public class MenuManager : MonoBehaviour
{
    public static void OpenMenu(string id)
    {
        MenuDictionary.instance.GetEntryById(id).Open();
    }

    public static void CloseMenu(string id)
    {
        MenuDictionary.instance.GetEntryById(id).Close();
    }

    public static void ToggleMenu(string id)
    {
        MenuDictionary.instance.GetEntryById(id).Toggle();
    }

    public static void CloseAllMenus()
    {
        foreach (Menu menu in MenuDictionary.instance.entries)
        {
            menu.Close();
        }
    }

    private void Start()
    {
        CloseAllMenus();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllMenus();
        }

        if (Input.GetButtonDown("BuildingMenu"))
        {
            ToggleMenu("menu_building");
        }
    }
}
