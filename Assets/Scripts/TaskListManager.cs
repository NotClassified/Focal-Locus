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
    int dayStreak;

    public override void OnEnter()
    {
        base.OnEnter();
        NewTaskPrompt(false);
    }


    public void ResetTasks()
    {
        bool allTasksCompleted = true;
        foreach(TaskObject task in listParent.GetComponentsInChildren<TaskObject>())
        {
            if (task.CompleteStatus)
            {
                task.TaskStatus();
            }
            else
                allTasksCompleted = false;
        }
        if (allTasksCompleted)
        {
            dayStreak++;
            UpdateData();
        }
    }

    public void NewTaskPrompt(bool active)
    {
        newTaskPrompt.SetActive(active);
        newTaskPrompt.GetComponentInChildren<TMP_InputField>().text = "";
    }

    public void ConfrimNewTask()
    {
        AddTask(newTaskName);
        UpdateData();
        NewTaskPrompt(false);
    }

    void AddTask(string taskName)
    {
        GameObject task = Instantiate(taskPrefab, listParent);
        task.GetComponent<TaskObject>().TaskName = taskName;
    }
    public void RemoveTask(GameObject task)
    {
        Destroy(task);
        StartCoroutine(WaitForRemovingTask());
    }
    public IEnumerator WaitForRemovingTask()
    {
        yield return new WaitForEndOfFrame();
        UpdateData();
    }

    public void UpdateData()
    {
        if (GetComponent<TaskListDataManager>() == null)
        {
            return;
        }

        TaskListCollection data = new TaskListCollection();
        List<string> names = new List<string>();
        foreach (Transform child in listParent)
        {
            names.Add(child.GetComponent<TaskObject>().TaskName);
        }
        data.list.tasks = names;
        data.list.streak = dayStreak;

        GetComponent<TaskListDataManager>().SaveData(data);
    }

    public void LoadData(TaskListCollection data)
    {
        if (data == null)
            return;

        for (int i = 0; i < data.list.tasks.Count; i++)
        {
            AddTask(data.list.tasks[i]);
        }
        dayStreak = data.list.streak;
    }

    public void SetNewTaskName(string name) => newTaskName = name;

}
