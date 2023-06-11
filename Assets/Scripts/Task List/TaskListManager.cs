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

    ///<summary> (bool onlyDisplayChange) </summary>
    public static event System.Action<bool> ListChanged;

    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    TaskObjectData activeParent = null;

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


    [ContextMenu("Printables")]
    void Printables()
    {
        if (activeParent is null)
            print(null);
        else
            print(activeParent.name);
    }

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
    void AddTaskAndSiblings(TaskObjectData _taskData)
    {
        AddTask(_taskData);

        if (_taskData.nextSibling != null)
        {
            AddTaskAndSiblings(_taskData.nextSibling);
        }
    }
    ///<summary> adds root tasks </summary>
    void AddTaskAndSiblings() => AddTaskAndSiblings(dataManager.currentData.lists[DayIndex].rootTasks[0]);
    TaskObjectData GetFirstSibling()
    {
        TaskActions firstTaskAction = listParent.GetComponentInChildren<TaskActions>();
        if (firstTaskAction is not null)
            return firstTaskAction.taskData;
        else
            return null;
    }
    TaskObjectData GetLastSibling()
    {
        if (activeParent is null || activeParent.child is null) //find first sibling
        {
            TaskObjectData firstSibling = GetFirstSibling();
            if (firstSibling is not null)
                return GetLastSibling(firstSibling);
            else
                return null; //no tasks in this layer
        }

        return GetLastSibling(activeParent.child);
    }
    TaskObjectData GetLastSibling(TaskObjectData sibling)
    {
        if (sibling.nextSibling is null)
            return sibling;
        else
            return sibling.nextSibling;
    }

    public void ConfrimNewTask(string newTaskName)
    {
        TaskObjectData lastSibling = GetLastSibling();

        TaskObjectData newTask = new TaskObjectData(newTaskName, activeParent, lastSibling);
        if (lastSibling is not null)
            lastSibling.nextSibling = newTask;

        if (activeParent is null)
            dataManager.AddRootLayerTask(newTask);
        else if (lastSibling is null)
            activeParent.child = newTask; //"newTask" is the first task in this layer

        AddTask(newTask);
        if (ListChanged != null)
            ListChanged(false); 
    }

    public void CompleteTask(TaskObjectData taskData)
    {
        taskData.completed = !taskData.completed;
        UpdateData();
    }
    public void RemoveTask(TaskActions task)
    {
        //if (task.taskData.hasChildren)
        //    RemoveChildren(task.taskData.layerID);

        //dataManager.RemoveTask(task.taskData.layerID);

        task.gameObject.SetActive(false);
        Destroy(task.gameObject);
        ResetTaskLayerIDs();
        StartDelayUpdateRoutine(2);
    }
    void RemoveChildren(string parentID)
    {
        foreach (TaskActions actionScript in listParent.GetComponentsInChildren<TaskActions>())
        {
            //if (IsInsideLayer(actionScript.taskData.layerID, parentID))
            //    RemoveTask(actionScript);
        }
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
        if (ListChanged != null)
            ListChanged(false);
    }
    //TaskActions FindTaskActionScript(TaskObjectData task)
    //{
    //    foreach (TaskActions actionScript in listParent.GetComponentsInChildren<TaskActions>())
    //    {
    //        if (actionScript.taskData == task)
    //        {
    //            return actionScript;
    //        }
    //    }
    //    return null;
    //}
    public static bool IsInsideLayer(string taskID, string layerID)
    {
        if (taskID == null)
            taskID = "";

        return taskID.Length == layerID.Length + 1 && taskID.Substring(0, taskID.Length - 1).Equals(layerID);
    }
    public TaskObjectData GetParentOfLayer(string layerID)
    {
        foreach (TaskObjectData task in dataManager.currentData.lists[dayIndex].rootTasks)
        {
            //if (task.layerID.Equals(layerID))
            //    return task;
        }

        Debug.LogError("no parent found");
        return null;
    }
    public static int GetLayerIDDigit(string id, int digitIndex)
    {
        if (id == null || id.Length <= digitIndex)
        {
            Debug.LogError("invalid ID");
            return 0;
        }

        return int.Parse(id.Substring(digitIndex, digitIndex + 1));
    }
    void ResetTaskLayerIDs()
    {
        //gather all remaining tasks
        List<TaskActions> actionScripts = new List<TaskActions>();
        for (int i = 0; i < listParent.childCount; i++)
        {
            if (listParent.GetChild(i).gameObject.activeSelf)
                actionScripts.Add(listParent.GetChild(i).GetComponent<TaskActions>());
        }
        for (int i = 0; i < actionScripts.Count; i++)
        {
            //string newID = actionScripts[i].taskData.layerID;
            //newID = newID.Substring(0, newID.Length - 1) + i;
            //actionScripts[i].taskData.layerID = newID;
        }
    }
    void GoToRootLayer()
    {
        activeParent = null;
        foreach (TaskObjectData task in dataManager.currentData.lists[dayIndex].rootTasks)
        {
            AddTask(task);
        }
        if (ListChanged != null)
            ListChanged(true);
    }

    public void ChangeLayer(TaskObjectData newParent)
    {
        ClearActiveTasks();

        if (newParent is null)
        {
            AddTaskAndSiblings(); //add root tasks
        }
        else if (newParent.child is not null)
            AddTaskAndSiblings(newParent.child); //add all children tasks
        activeParent = newParent;

        if (ListChanged != null)
            ListChanged(true);
    }
    public void Button_LeaveActiveLayer()
    {
        if (activeParent is not null)
        {
            ChangeLayer(activeParent.parent);
        }
    }

    void ClearActiveTasks()
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
        TaskListData rootLayer = new TaskListData();

        TaskActions[] taskActionScripts = listParent.GetComponentsInChildren<TaskActions>();
        foreach (TaskActions actionScript in taskActionScripts)
        {
            rootLayer.rootTasks.Add(actionScript.taskData);
        }
        dataManager.currentData.lists[DayIndex] = rootLayer;

        if (ListChanged != null)
            ListChanged(false);
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

    public void LoadCurrentCollection()
    {
        //TaskListCollection collection = dataManager.currentData;
        //if (collection == null)
        //    return;

        //show the task list from "collection" on "dayIndex"
        ClearActiveTasks();
        GoToRootLayer();
        //if (collection.lists.Count > collection.todayIndex) //make sure this collection has enough lists
        //{

        //}
        //else
        //{
        //    activeParent = null;
        //}

        firstDay = dataManager.currentData.firstDay;
        DayIndex = dataManager.currentData.todayIndex;
        SetToday();
    }
}
