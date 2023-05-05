using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskListCollection
{
    public List<TaskListData> lists = new List<TaskListData>();
    public int todayIndex;
    public DaysOfWeek firstDay; //index 0
}
