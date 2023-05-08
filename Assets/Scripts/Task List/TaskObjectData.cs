using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TaskObjectData
{
    public string name;
    public bool completed;
    public bool rootLayer;

    public TaskObjectData(string name, bool completed, bool rootLayer)
    {
        this.name = name;
        this.completed = completed;
        this.rootLayer = rootLayer;
    }
    public TaskObjectData(TaskObjectData copy)
    {
        this.name = copy.name;
        this.completed = copy.completed;
        this.rootLayer = copy.rootLayer;
    }
}
