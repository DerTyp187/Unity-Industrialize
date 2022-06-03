public class ItemDictionary : Dictionary<ItemSO>
{
    public override ItemSO GetEntryById(string id) => entries.Find(entry => entry.id == id);
}
