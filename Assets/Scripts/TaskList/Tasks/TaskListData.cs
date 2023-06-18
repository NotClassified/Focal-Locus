using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskListData
{
    public List<TaskData> tasks = new List<TaskData>();

    public TaskListData() { }
    public TaskListData(List<TaskData> cloneTasks)
    {
        foreach (TaskData task in cloneTasks)
        {
            this.tasks.Add(new TaskData(task));
        }
    }
}
