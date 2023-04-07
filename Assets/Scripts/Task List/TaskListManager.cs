using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskListManager : ScreenState
{
    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    string newTaskName;

    int dayStreak;
    int dayIndex;

    [SerializeField] GameObject newTaskPrompt;
    bool NewTaskPrompt
    {
        set
        {
            newTaskPrompt.SetActive(value);
            newTaskPrompt.GetComponentInChildren<TMP_InputField>().text = "";
        }
    }
    public void ButtonNewTask() => NewTaskPrompt = true;

    public override void OnEnter()
    {
        base.OnEnter();
        NewTaskPrompt = false;
    }


    public void ButtonResetTasks()
    {
        foreach(TaskObject task in listParent.GetComponentsInChildren<TaskObject>())
        {
            ToggleTaskComplete(task);
        }
        UpdateData();
    }

    void AddTask(string taskName)
    {
        GameObject taskObject = Instantiate(taskPrefab, listParent);
        taskObject.GetComponent<TaskObject>().TaskName = taskName;
    }
    void AddTask(TaskObjectData taskData)
    {
        TaskObject taskscript = Instantiate(taskPrefab, listParent).GetComponent<TaskObject>();

        taskscript.TaskName = taskData.name;
        if (taskData.completed)
            ToggleTaskComplete(taskscript);
    }
    public void ConfrimNewTask()
    {
        AddTask(newTaskName);
        NewTaskPrompt = false;

        UpdateData();
    }

    public void RemoveTask(GameObject task)
    {
        Destroy(task);
        DelayUpdateRoutine(1);
    }
    public void ToggleTaskComplete(TaskObject task)
    {
        task.completed = !task.completed;

        //darken task when completed
        Image button = task.transform.GetChild(0).GetComponent<Image>();
        var alpha = button.color;
        alpha.a = task.completed ? .5f : 1f;
        button.color = alpha;

        //slash text when completed
        TextMeshProUGUI taskText = task.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        if (task.completed)
            taskText.fontStyle = FontStyles.Strikethrough;
        else
            taskText.fontStyle = FontStyles.Bold;
    }
    public void MoveUpTask(Transform task)
    {
        int index = task.GetSiblingIndex();
        if (index > 0)
        {
            task.SetSiblingIndex(index - 1);
        }
        UpdateData();
    }

    void UpdateData()
    {
        if (GetComponent<TaskListDataManager>() == null)
        {
            Debug.LogError("No data manager detected");
            return;
        }

        TaskListData list = new TaskListData();
        foreach (Transform child in listParent)
        {
            TaskObject taskObj = child.GetComponent<TaskObject>();
            TaskObjectData task = new TaskObjectData();

            task.name = taskObj.TaskName;
            task.completed = taskObj.completed;
            list.tasks.Add(task);
        }

        GetComponent<TaskListDataManager>().SaveData(list, dayIndex);
    }
    public void DelayUpdateRoutine(int frames) => StartCoroutine(DelayUpdate(frames));
    IEnumerator DelayUpdate(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        UpdateData();
    }

    public void LoadCollection(TaskListCollection collection)
    {
        if (collection == null)
            return;

        var taskList = collection.lists[collection.dayIndex].tasks;
        for (int i = 0; i < taskList.Count; i++)
        {
            AddTask(taskList[i]);
        }
        dayIndex = collection.dayIndex;
    }

    public void InputFieldSetNewTaskName(string name) => newTaskName = name;

}
