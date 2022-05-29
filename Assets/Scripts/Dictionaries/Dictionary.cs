using System.Collections.Generic;
using UnityEngine;

public abstract class Dictionary<TEntryType> : MonoBehaviour
{
    public List<TEntryType> entries = new List<TEntryType>();

    public abstract TEntryType GetEntryById(string id);

    public static Dictionary<TEntryType> instance { get; private set; }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public void Register(TEntryType entry)
    {
        entries.Add(entry);
    }

    public void Unregister(TEntryType entry)
    {
        entries.Remove(entry);
    }
}
