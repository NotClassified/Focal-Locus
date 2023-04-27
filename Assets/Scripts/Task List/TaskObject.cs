using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskObject : MonoBehaviour
{
    protected TaskListManager manager;

    public bool completed;
    public string groupName;

    string taskName;
    public string TaskName
    {
        get => taskName;
        set
        {
            taskName = value;
            transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
    }

    private void Start()
    {
        manager = FindObjectOfType<TaskListManager>();
        if (manager == null)
        {
            Debug.LogError("manager not found");
        }
    }

    public void ButtonToggleTaskStatus()
    {
        manager.ToggleTaskComplete(this);
        manager.StartDelayUpdateRoutine(2);
    }
    public void ButtonMoveUp() => manager.MoveUpTask(transform);

    public void ButtonRemove() => manager.RemoveTask(this);

}
