using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DaysOfWeek
{
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public class TaskListManager : ScreenState
{
    TaskListDataManager dataManager;

    public static event System.Action ListChanged;

    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    TaskObjectData activeParentTask = null;

    int dayStreak;
    public DaysOfWeek firstDay;
    public const int amountOfDaysInAWeek = 7;
    [SerializeField] TextMeshProUGUI dayIndex_text;
    [SerializeField] TextMeshProUGUI dayName_text;
    [SerializeField] TextMeshProUGUI nextDayButton_Text;

    int dayIndex;
    public int DayIndex
    {
        get => dayIndex;
        set
        {
            dayIndex = value;
            dayIndex_text.text = value.ToString();
        }
    }

    [SerializeField] Button addGroup_Button;

    public void ResetTasks()
    {
        foreach (TaskActions task in listParent.GetComponentsInChildren<TaskActions>())
        {
            CompleteTask(task.taskData);
        }
    }
    public void ButtonChangeDay(int dayIncrement)
    {
        if (DayIndex + dayIncrement >= 0 && listParent.GetComponentsInChildren<TaskActions>().Length > 0)
        {
            dataManager.ChangeDay(DayIndex + dayIncrement, DayIndex);
            StartDelayUpdateRoutine(2);
        }
    }
    public void SetToday()
    {
        //find which day of the week is today
        int todaysIndex = ((int)firstDay + dayIndex) % amountOfDaysInAWeek;
        dayName_text.text = ((DaysOfWeek)todaysIndex).ToString();

        //if this is the last day that has a list, the next day button should be a "+" sign
        if (dataManager.currentData.lists.Count <= DayIndex + 1)
            nextDayButton_Text.text = "+";
        else
            nextDayButton_Text.text = ">";
    }

    private void Awake()
    {
        if (dataManager == null)
        {
            dataManager = GetComponent<TaskListDataManager>();

            if (dataManager == null)
                Debug.LogError("No data manager detected");
        }
        //ListChanged += ActionMessage;
    }
    void ActionMessage() => Debug.Log("Action");

    void AddTask(TaskObjectData _taskData)
    {
        TaskActions task = Instantiate(taskPrefab, listParent).GetComponent<TaskActions>();
        task.taskData = _taskData;
    }
    public void ConfrimNewTask(string newTaskName)
    {
        TaskObjectData newTask = new TaskObjectData(newTaskName, false, activeParentTask);
        AddTask(newTask);

        UpdateData();
    }

    public void CompleteTask(TaskObjectData taskData)
    {
        taskData.completed = !taskData.completed;
        UpdateData();
    }
    public void RemoveTask(TaskActions task)
    {
        if (task.parentTask != null)
        {
            RemoveTask(task.parentTask);
        }
        Destroy(task.gameObject);
        StartDelayUpdateRoutine(2);
        //progressManager.UpdateProgress();
    }
    public void MoveUpTask(Transform task)
    {
        int index = task.GetSiblingIndex();
        if (index > 0)
        {
            while (!listParent.GetChild(index - 1).gameObject.activeSelf)
            {
                index--;
                if (index <= 0)
                    return; //the task is at the top of the shown list
            }
            task.SetSiblingIndex(index - 1);
        }
        UpdateData();
    }


    void RemoveAllTasks() //note: won't save
    {
        foreach(Transform task in listParent)
        {
            task.gameObject.SetActive(false);
            Destroy(task.gameObject);
        }
    }

    void UpdateData()
    {
        //gather all data from today's list and save it
        TaskListData list = new TaskListData();
        TaskActions[] taskScripts = listParent.GetComponentsInChildren<TaskActions>();
        list.tasks = new TaskObjectData[taskScripts.Length];

        for (int i = 0; i < taskScripts.Length; i++)
        {
            list.tasks[i] = taskScripts[i].taskData;
        }
        dataManager.SaveData(list, DayIndex, firstDay);

        ListChanged();
    }
    public void StartDelayUpdateRoutine(int frames) => StartCoroutine(DelayUpdate(frames));
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

        RemoveAllTasks();
        //show the task list from "collection" on "dayIndex"
        if (collection.lists.Count > 0)
        {
            TaskObjectData[] taskList = collection.lists[collection.todayIndex].tasks;
            for (int i = 0; i < taskList.Length; i++)
            {
                AddTask(taskList[i]);
            }
        }
        firstDay = collection.firstDay;
        DayIndex = collection.todayIndex;
        SetToday();
        ListChanged();
    }
}
