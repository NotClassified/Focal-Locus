using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TaskObjectData
{
    public string name;
    public bool completed;

    public TaskObjectData parent;
    public TaskObjectData child;
    public TaskObjectData nextSibling;
    public TaskObjectData prevSibling;

    public TaskObjectData(TaskObjectData copy)
    {
        if (copy != null)
        {
            copy.name = name;
            copy.completed = completed;
            copy.parent = parent;
            copy.child = child;
            copy.nextSibling = nextSibling;
            copy.prevSibling = prevSibling;
        }
    }

    public TaskObjectData(string name, bool completed, TaskObjectData parent)
    {
        this.name = name;
        this.completed = completed;
        this.parent = parent;
    }

    public TaskObjectData(string name, bool completed, TaskObjectData parent, TaskObjectData child) : this(name, completed, parent)
    {
        this.child = child;
    }

    public TaskObjectData(string name, bool completed, TaskObjectData parent, TaskObjectData child, TaskObjectData nextSibling, TaskObjectData prevSibling) : this(name, completed, parent, child)
    {
        this.nextSibling = nextSibling;
        this.prevSibling = prevSibling;
    }
}
