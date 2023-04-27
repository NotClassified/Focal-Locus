using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskGroupObject : TaskObject
{
    public void Button_ShowGroup()
    {
        manager.ShowList(groupName);
    }
}
