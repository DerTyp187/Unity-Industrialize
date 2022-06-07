using System.Collections.Generic;
using UnityEngine;

public class IOPort : MonoBehaviour
{
    public bool isInput;
    public ItemSO item;
    public int itemCount = 0;
    private int maxItemCount = 100;

    public int IncreaseItemCount(int value) // Returns rest of value
    {
        int restOfValue = 0;
        if (itemCount + value > maxItemCount)
        {
            restOfValue = value - (maxItemCount - itemCount);
            itemCount = maxItemCount;
        }
        else
        {
            itemCount += value;
        }

        return restOfValue;
    }

    public int DecreaseItemCount(int value) // Returns rest of value
    {
        int restOfValue = 0;
        if (itemCount - value < 0)
        {
            restOfValue = value - itemCount;
            itemCount = 0;
        }
        else
        {
            itemCount -= value;
        }

        return restOfValue;
    }
}