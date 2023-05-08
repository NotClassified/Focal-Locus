using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TaskListCollection
{
    public List<TaskListData> lists = new List<TaskListData>();
    public int todayIndex;
    public DaysOfWeek firstDay; //index 0

    public TaskListCollection()
    {
        lists.Add(new TaskListData()); //add empty list for writing on
    }
}
