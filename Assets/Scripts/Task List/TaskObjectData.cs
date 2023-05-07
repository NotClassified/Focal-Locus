using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskObjectData
{
    public string name;
    public bool completed;
    public string layerID;
    public bool hasChildren;

    public TaskObjectData(string name, bool completed, string id, bool hasChildren)
    {
        this.name = name;
        this.completed = completed;
        this.layerID = id;
        this.hasChildren = hasChildren;
    }
    public TaskObjectData(TaskObjectData copy)
    {
        this.name = copy.name;
        this.completed = copy.completed;
        this.layerID = copy.layerID;
    }
}
