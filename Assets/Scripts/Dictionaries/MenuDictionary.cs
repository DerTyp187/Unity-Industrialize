public class MenuDictionary : Dictionary<Menu>
{
    // Menus are registering themselves automatically
    public override Menu GetEntryById(string id) => entries.Find(entry => entry.id == id);
}
