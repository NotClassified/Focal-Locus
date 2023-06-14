using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DaysOfWeek
{
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public enum SiblingTask
{
    Both, Next, Previous
}

public class TaskListManager : MonoBehaviour
{
    TaskListDataManager dataManager;
    ProgressManager progressManager;

    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    TaskData activeParent;

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


    public void ResetTasks()
    {
        foreach (Transform child in listParent)
        {
            TaskUI task = child.GetComponent<TaskUI>();
            if (task.Data.completed)
            {
                ToggleTaskComplete(task);
            }
        }
    }
    public void ButtonChangeDay(int dayIncrement)
    {
        if (DayIndex + dayIncrement >= 0)
        {
            dataManager.ChangeDay(DayIndex + dayIncrement, DayIndex);

            ShowList();
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
        if (progressManager == null)
        {
            progressManager = GetComponent<ProgressManager>();

            if (progressManager == null)
                Debug.LogError("No data manager detected");
        }
    }

    public void OnEnable()
    {
        ShowList();
    }

    public void TaskMainAction(TaskUI taskUI)
    {
        if (taskUI.Data.child_ID == 0)
        {
            ToggleTaskComplete(taskUI);
        }
        else //this task has children
        {
            RemoveAllTasks();
            AddSiblingTasks(dataManager.FindTask(taskUI.Data.child_ID), SiblingTask.Both);
            activeParent = taskUI.Data;
        }
    }
    public void TaskAddChild(TaskUI taskUI)
    {
        activeParent = taskUI.Data;
        GetComponent<PromptManager>().PromptAction(Prompt.AddChild);
    }

    void AddTask(TaskData taskData)
    {
        GameObject taskObject = Instantiate(taskPrefab, listParent);
        taskObject.GetComponent<TaskUI>().Data = taskData;

        if (listParent.childCount > 1) //exchange sibling IDs
        {
            TaskData prevSibling = listParent.GetChild(listParent.childCount - 2).GetComponent<TaskUI>().Data;
            prevSibling.nextSibling_ID = taskData.id;
            taskData.prevSibling_ID = prevSibling.id;
        }
    }
    public TaskData ConfrimNewTask(string newTaskName)
    {
        TaskData newTask = new TaskData(newTaskName, TaskIDManager.getNewID, activeParent is null ? 0 : activeParent.id);
        AddTask(newTask);

        dataManager.AddTask(newTask);
        progressManager.UpdateProgress();

        return newTask;
    }
    public void ConfrimChildTask(string newTaskName)
    {
        RemoveAllTasks();
        //create child task
        TaskData newTask = new TaskData(newTaskName, TaskIDManager.getNewID, activeParent.id);
        Instantiate(taskPrefab, listParent).GetComponent<TaskUI>().Data = newTask;

        activeParent.child_ID = newTask.id; //give parent the child's ID
        GetUIComponent(activeParent.id).UpdateUI(TaskUI.DataProperties.ParentStatus);

        dataManager.AddTask(newTask);
        progressManager.UpdateProgress();
    }
    public void DeconfirmChildTask() => ChangeParent(activeParent.parent_ID);

    TaskUI GetUIComponent(int searchID)
    {
        foreach (Transform task in listParent)
        {
            if (task.GetComponent<TaskUI>().Data.id == searchID)
            {
                return task.GetComponent<TaskUI>();
            }
        }
        return null;
    }
    void ChangeParent(int parentID)
    {
        if (parentID == 0)
        {
            activeParent = null;
            return;
        }
        activeParent = dataManager.FindTask(parentID);
    }

    public void RemoveTask(TaskUI task)
    {
        dataManager.RemoveTask(task.Data);
        Destroy(task.gameObject);

        progressManager.UpdateProgress();
    }

    public void ToggleTaskComplete(TaskUI task)
    {
        task.Data.completed = !task.Data.completed;
        task.UpdateUI(TaskUI.DataProperties.CompleteStatus);

        dataManager.SaveData();
        progressManager.UpdateProgress();
    }
    public void MoveUpTask(Transform task)
    {
        int newIndex = task.GetSiblingIndex();

        while (newIndex > 0 && task.parent.GetChild(newIndex - 1).gameObject.activeSelf is false)
        {
            newIndex--;
        }
        //if ((task.parent == listParent && newIndex > 0) || (task.parent == groupListParent && newIndex > 1))
        //{
        //    newIndex--;
        //}
        //else
        //    return;

        task.SetSiblingIndex(newIndex);

        UpdateData();
    }

    void AddSiblingTasks(TaskData sibling, SiblingTask direction)
    {
        switch (direction)
        {
            case SiblingTask.Both: //add this sibling, next sibling, and previous sibling
                AddTask(sibling); 
                AddSiblingTasks(sibling, SiblingTask.Next);
                AddSiblingTasks(sibling, SiblingTask.Previous);
                break;

            case SiblingTask.Next:
                if (sibling.nextSibling_ID != 0)
                {
                    TaskData nextSibling = dataManager.FindTask(sibling.nextSibling_ID);
                    AddTask(nextSibling);
                    AddSiblingTasks(nextSibling, SiblingTask.Next);
                }
                break;

            case SiblingTask.Previous:
                if (sibling.prevSibling_ID != 0)
                {
                    TaskData prevSibling = dataManager.FindTask(sibling.prevSibling_ID);
                    AddTask(prevSibling);
                    AddSiblingTasks(prevSibling, SiblingTask.Previous);
                }
                break;
        }
    }

    ///<summary> will not save </summary>
    void RemoveAllTasks()
    {
        foreach(Transform task in listParent)
        {
            task.gameObject.SetActive(false);
            Destroy(task.gameObject);
        }
    }

    void UpdateData()
    {

        progressManager.UpdateProgress();
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
        if (collection is null)
            return;

        RemoveAllTasks();
        //show the task list from "collection" on "dayIndex"
        var taskList = collection.lists[collection.dayIndex].tasks;
        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskList[i].parent_ID == 0)
                AddTask(taskList[i]);
        }
        firstDay = collection.firstDay;
        DayIndex = collection.dayIndex;
        SetToday();
        progressManager.UpdateProgress();
    }

    public void ShowList()
    {
        progressManager.UpdateProgress();
    }
    public void ShowParentSiblings()
    {
        if (activeParent is null)
            return;
        RemoveAllTasks();
        AddSiblingTasks(activeParent, SiblingTask.Both);
        ChangeParent(activeParent.parent_ID);
    }
    bool IsTaskShown(int taskId)
    {
        foreach (Transform task in listParent)
        {
            if (task.GetComponent<TaskUI>().Data.id == taskId)
            {
                return true;
            }
        }
        return false;
    }
}
