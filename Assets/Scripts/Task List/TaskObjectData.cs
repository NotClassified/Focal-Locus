using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskObjectData
{
    public string name;
    public bool completed;
    public int id;

    public int parent_ID;
    public int child_ID;
    public int nextSibling_ID;
    public int prevSibling_ID;

    public TaskObjectData(string name, int id, int parent_ID)
    {
        this.name = name;
        this.id = id;
        this.parent_ID = parent_ID;
    }

    public TaskObjectData(TaskObjectData clone)
    {
        this.name = clone.name;
        this.completed = clone.completed;
        this.id = clone.id;
        this.parent_ID = clone.parent_ID;
        this.child_ID = clone.child_ID;
        this.nextSibling_ID = clone.nextSibling_ID;
        this.prevSibling_ID = clone.prevSibling_ID;
    }
}
