using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskListManager : ScreenState
{
    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    [SerializeField] GameObject newTaskPrompt;
    string newTaskName;

    public override void OnEnter()
    {
        base.OnEnter();
        NewTaskPrompt(false);
    }

    void AddTask()
    {
        GameObject task = Instantiate(taskPrefab, listParent);
        task.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = newTaskName;
    }

    public void ResetTasks()
    {
        foreach(TaskObject task in listParent.GetComponentsInChildren<TaskObject>())
        {
            if (task.CompleteStatus)
            {
                task.TaskStatus();
            }
        }
    }

    public void NewTaskPrompt(bool active)
    {
        newTaskPrompt.SetActive(active);
        newTaskPrompt.GetComponentInChildren<TMP_InputField>().text = "";
    }

    public void ConfrimNewTask()
    {
        AddTask();
        NewTaskPrompt(false);
    }

    public void SetNewTaskName(string name) => newTaskName = name;

}
