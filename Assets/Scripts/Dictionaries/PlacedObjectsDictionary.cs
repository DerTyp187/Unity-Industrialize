public class PlacedObjectsDictionary : Dictionary<PlacedObjectTypeSO>
{
    public override PlacedObjectTypeSO GetEntryById(string id) => entries.Find(entry => entry.id == id);
}
