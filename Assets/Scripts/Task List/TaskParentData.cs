using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TaskParentData : TaskObjectData
{
    public List<TaskObjectData> children;

    public TaskParentData(TaskObjectData task) : base(task)
    {
        this.children = new List<TaskObjectData>();
    }
}
