using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DaysOfWeek
{
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
}

public class TaskListManager : MonoBehaviour
{
    public static event System.Action ListChange;

    TaskListDataManager dataManager;

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
        foreach (TaskUI task in listParent.GetComponentsInChildren<TaskUI>())
        {
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
            AddTaskAndNextSiblings(FindFirstSibling(taskUI.Data.child_ID));
            ChangeParent(taskUI.Data);
        }
    }
    public void TaskAddChild(TaskUI taskUI)
    {
        ChangeParent(taskUI.Data);
        GetComponent<PromptManager>().PromptAction(Prompt.AddChild);
    }

    void AddTask(TaskData taskData, bool newTask)
    {
        GameObject taskObject = Instantiate(taskPrefab, listParent);
        taskObject.GetComponent<TaskUI>().Data = taskData;

        if (newTask)
            dataManager.AddTask(taskData);
    }
    void AddNewSiblingTask(TaskData newSibling)
    {
        if (listParent.childCount > 0) //exchange sibling IDs
        {
            TaskData prevSibling = listParent.GetChild(listParent.childCount - 1).GetComponent<TaskUI>().Data;
            prevSibling.nextSibling_ID = newSibling.id;
            newSibling.prevSibling_ID = prevSibling.id;
        }
        AddTask(newSibling, true);
        SetParentCompleteStatus();
    }
    public TaskData ConfrimNewTask(string newTaskName)
    {
        TaskData newTask = new TaskData(newTaskName, TaskIDManager.getNewID, activeParent is null ? 0 : activeParent.id);
        AddNewSiblingTask(newTask);
        ListChange();

        return newTask;
    }
    public void ConfrimChildTask(string newTaskName)
    {
        RemoveAllTasks();
        TaskData newChildTask = new TaskData(newTaskName, TaskIDManager.getNewID, activeParent.id);

        activeParent.child_ID = newChildTask.id; //give parent the child's ID
        activeParent.completed = false; //due to new child task, the parent won't be completed

        AddTask(newChildTask, true);
        ListChange();

    }
    public void DeconfirmChildTask() => ChangeParent(activeParent.parent_ID);

    TaskUI GetUIComponent(int searchID)
    {
        foreach (TaskUI task in listParent.GetComponentsInChildren<TaskUI>())
        {
            if (task.Data.id == searchID)
            {
                return task;
            }
        }
        return null;
    }
    void ChangeParent(TaskData parent) => activeParent = parent;
    void ChangeParent(int parentID)
    {
        if (parentID == 0)
            ChangeParent(null); //no parent, these are root tasks
        else
            ChangeParent(dataManager.FindTask(parentID));
    }

    public void RemoveTask(TaskUI task)
    {
        dataManager.RemoveTask(task.Data);
        Destroy(task.gameObject);

        ListChange();

    }

    public void ToggleTaskComplete(TaskUI task)
    {
        task.Data.completed = !task.Data.completed;
        task.UpdateUI(TaskUI.DataProperties.CompleteStatus);
        SetParentCompleteStatus();

        dataManager.SaveData();
        ListChange();
    }
    void SetParentCompleteStatus()
    {
        if (activeParent is not null)
        {
            bool allChildrenCompleted = true;
            foreach (TaskUI task in listParent.GetComponentsInChildren<TaskUI>())
            {
                if (!task.Data.completed)
                {
                    allChildrenCompleted = false;
                    break;
                }
            }
            activeParent.completed = allChildrenCompleted;
            print(allChildrenCompleted);
        }
    }
    void SetParentCompleteStatus(TaskData child)
    {
        Debug.LogError("not implemented");
    }
    void SetParentCompleteStatus(int childID)
    {
        Debug.LogError("not implemented");

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

    void AddTaskAndNextSiblings(TaskData task)
    {
        AddTask(task, false);
        if (task.nextSibling_ID != 0) //does this task have a next sibling?
        {
            TaskData nextSibling = dataManager.FindTask(task.nextSibling_ID);
            AddTaskAndNextSiblings(nextSibling);
        }
    }
    TaskData FindFirstSibling(int siblingID) => FindFirstSibling(dataManager.FindTask(siblingID));
    TaskData FindFirstSibling(TaskData sibling)
    {
        if (sibling.prevSibling_ID != 0) //is there a sibling before?
            return FindFirstSibling(dataManager.FindTask(sibling.prevSibling_ID));

        else //this is the first sibling
            return sibling;
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
        ListChange();

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
                AddTask(taskList[i], false);
        }
        firstDay = collection.firstDay;
        DayIndex = collection.dayIndex;
        SetToday();
        ListChange();

    }

    public void ShowParentSiblings()
    {
        if (activeParent is null)
            return;
        RemoveAllTasks();
        AddTaskAndNextSiblings(FindFirstSibling(activeParent));
        ChangeParent(activeParent.parent_ID);
    }
    bool IsTaskShown(int taskId)
    {

        foreach (TaskUI task in listParent.GetComponentsInChildren<TaskUI>())
        {
            if (task.Data.id == taskId)
            {
                return true;
            }
        }
        return false;
    }
}
