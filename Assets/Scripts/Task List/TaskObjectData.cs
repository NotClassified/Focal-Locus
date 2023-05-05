using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskObjectData
{
    public string name;
    public bool completed;
    public TaskObjectData parent = null;

    public TaskObjectData(string name, bool completed, TaskObjectData parent)
    {
        this.name = name;
        this.completed = completed;
        this.parent = parent;
    }
}
