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
    [SerializeField] GameObject movingTaskPrefab;
    [SerializeField] Transform mainCanvas;
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
        if (dayName_text is not null)
            dayName_text.text = ((DaysOfWeek)todaysIndex).ToString();

        //if this is the last day that has a list, the next day button should be a "+" sign
        if (dataManager.currentData.lists.Count <= DayIndex + 1)
            nextDayButton_Text.text = "+";
        else
            nextDayButton_Text.text = ">";
    }

    private void Awake()
    {
        dataManager = GetComponent<TaskListDataManager>();
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
            AddTaskAndNextSiblings(dataManager.FindFirstSibling(taskUI.Data.child_ID));
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
        int taskID = TaskIDManager.getNewID;
        if (newTaskName.Equals("/id"))
        {
            newTaskName = taskID.ToString();
        }

        TaskData newTask = new TaskData(newTaskName, taskID, activeParent is null ? 0 : activeParent.id);
        AddNewSiblingTask(newTask);
        ListChange();

        return newTask;
    }
    public void ConfrimChildTask(string newTaskName)
    {
        int taskID = TaskIDManager.getNewID;
        if (newTaskName.Equals("/id"))
        {
            newTaskName = taskID.ToString();
        }

        RemoveAllTasks();
        TaskData newChildTask = new TaskData(newTaskName, taskID, activeParent.id);

        activeParent.child_ID = newChildTask.id; //give parent the child's ID
        //due to new child task, the parents won't be completed
        if (activeParent.completed)
        {
            activeParent.completed = false;
            SetParentCompleteStatus(activeParent);
        }

        AddTask(newChildTask, true);
        ListChange();
    }
    public void DeconfirmChildTask() => ChangeParent(activeParent.parent_ID);

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
        dataManager.DeleteTask(task.Data);
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
        if (activeParent is null)
            return;

        bool allChildrenCompleted = true;
        foreach (TaskUI task in listParent.GetComponentsInChildren<TaskUI>())
        {
            if (!task.Data.completed)
            {
                allChildrenCompleted = false;
                break;
            }
        }
        if (activeParent.completed != allChildrenCompleted)
        {
            activeParent.completed = allChildrenCompleted;
            //make sure all parents of these tasks have the correct complete status
            SetParentCompleteStatus(activeParent);

            dataManager.SaveData();
        }
    }
    void SetParentCompleteStatus(TaskData child)
    {
        if (child.parent_ID == 0)
            return; //no parent

        bool allChildrenCompleted = IsTaskAndNextSiblingsComplete(dataManager.FindFirstSibling(child));

        TaskData parent = dataManager.FindTask(child.parent_ID);
        if (parent.completed != allChildrenCompleted)
        {
            parent.completed = allChildrenCompleted;
            //make sure all parents of these tasks have the correct complete status
            SetParentCompleteStatus(parent);
        }
    }

    bool IsTaskAndNextSiblingsComplete(TaskData sibling)
    {
        if (sibling.completed)
        {
            if (sibling.nextSibling_ID != 0) //is there a next sibling?
                return IsTaskAndNextSiblingsComplete(dataManager.FindTask(sibling.nextSibling_ID));
            else
                return true;
        }
        else
            return false;
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

    public IEnumerator MoveTask(TaskData activeTask)
    {
        GameObject movingTask = Instantiate(movingTaskPrefab, mainCanvas);
        MovingTask movingScript = movingTask.GetComponent<MovingTask>();
        movingScript.name_Text.text = activeTask.name;

        while (!Input.GetMouseButtonUp(0))
        {
            movingTask.GetComponent<RectTransform>().position = Input.mousePosition;
            yield return null;
        }
        bool makeTaskNextSibling = movingScript.transform.position.y < movingScript.hoveringTask.transform.position.y;

        TaskData lastHoveredTask = movingScript.hoveringTask.Data;
        //stop if the active task won't move/change siblings
        if (lastHoveredTask == activeTask || (makeTaskNextSibling && lastHoveredTask.nextSibling_ID.Equals(activeTask.id)) 
            || (!makeTaskNextSibling && lastHoveredTask.prevSibling_ID.Equals(activeTask.id)))
        {
            Destroy(movingTask);
            yield break;
        }

        dataManager.RemoveAndExchangeSiblingIDs(activeTask);
        //if the last hovered task is higher than mouse, make activeTask next sibling
        if (makeTaskNextSibling)
        {
            #region Exchanging Sibling References
            if (lastHoveredTask.nextSibling_ID != 0)
                dataManager.FindTask(lastHoveredTask.nextSibling_ID).prevSibling_ID = activeTask.id;
            if (lastHoveredTask.prevSibling_ID.Equals(activeTask.id))
                lastHoveredTask.prevSibling_ID = activeTask.prevSibling_ID;

            activeTask.prevSibling_ID = lastHoveredTask.id;
            activeTask.nextSibling_ID = lastHoveredTask.nextSibling_ID;
            lastHoveredTask.nextSibling_ID = activeTask.id;
            #endregion
        }
        else //make activeTask previous sibling
        {
            #region Exchanging Sibling References
            //Exchanging Sibling References:
            if (lastHoveredTask.prevSibling_ID != 0)
                dataManager.FindTask(lastHoveredTask.prevSibling_ID).nextSibling_ID = activeTask.id;
            if (lastHoveredTask.nextSibling_ID.Equals(activeTask.id))
                lastHoveredTask.nextSibling_ID = activeTask.nextSibling_ID;

            activeTask.prevSibling_ID = lastHoveredTask.prevSibling_ID;
            activeTask.nextSibling_ID = lastHoveredTask.id;
            lastHoveredTask.prevSibling_ID = activeTask.id; 
            #endregion
        }
        dataManager.SaveData();

        Destroy(movingTask);
        RemoveAllTasks();
        AddTaskAndNextSiblings(dataManager.FindFirstSibling(activeTask));
    }


    void RemoveAllTasks()
    {
        foreach(Transform task in listParent)
        {
            task.gameObject.SetActive(false);
            Destroy(task.gameObject);
        }
    }

    public void LoadCollection(TaskListCollection collection)
    {
        if (collection is null)
            return;

        RemoveAllTasks();
        //show root tasks from "collection" on "dayIndex"
        var taskList = collection.lists[collection.dayIndex].tasks;
        for (int i = 0; i < taskList.Count; i++)
        {
            if (taskList[i].parent_ID == 0)
                AddTask(taskList[i], false);
        }
        activeParent = null;
        firstDay = collection.firstDay;
        DayIndex = collection.dayIndex;
        SetToday();
        ListChange();

    }

    public void ShowParentSiblings()
    {
        if (activeParent is null)
        {
            ScreenManager.instance.ChangeState<MenuManager>(); //leave this list
            return;
        }

        RemoveAllTasks();
        AddTaskAndNextSiblings(dataManager.FindFirstSibling(activeParent));
        ChangeParent(activeParent.parent_ID);
    }
}
