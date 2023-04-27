using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskListData
{
    public List<TaskObjectData> tasks = new List<TaskObjectData>();
    public List<string> groups = new List<string>();
}
