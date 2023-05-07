using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskTreeData
{
    public string GetNextTreeID(string parentID, int taskAmountOnThisLayer)
    {
        return parentID + taskAmountOnThisLayer.ToString();
    }
}
