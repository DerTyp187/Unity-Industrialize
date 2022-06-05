using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! DEBUG TEXT
public class StackSizeText : MonoBehaviour
{
    public ItemObject itemObject;
    void Update()
    {
        gameObject.GetComponent<TMPro.TextMeshPro>().text = itemObject.stackSize.ToString();
    }
}
