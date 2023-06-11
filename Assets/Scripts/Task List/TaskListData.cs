using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TaskListData
{
    public List<TaskObjectData> rootTasks = new List<TaskObjectData>();
    public TaskObjectData test = new TaskObjectData("test", null, null);
}
