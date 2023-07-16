using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskListCollection
{
    public List<TaskListData> lists = new List<TaskListData>();
    public int dayIndex;
    public DaysOfWeek firstDay; //index 0

    public int newestTaskID = 0;
    public int GetNewID() => ++newestTaskID;

    public TaskListCollection()
    {
        lists.Add(new TaskListData());
    }
}
