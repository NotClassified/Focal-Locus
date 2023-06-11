using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TaskObjectData
{
    public string name;
    public bool completed;
    public TaskListData children = new TaskListData();

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

    public TaskObjectData(string name, TaskObjectData parent, TaskObjectData prevSibling)
    {
        this.name = name;
        this.parent = parent;
        this.prevSibling = prevSibling;
    }
}
