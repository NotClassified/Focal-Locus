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
    TaskListDataManager dataManager;
    ProgressManager progressManager;

    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    [SerializeField] Transform groupListParent;
    [SerializeField] GameObject groupTaskPrefab;
    public List<string> groupsCompleted = new List<string>();
    string activeGroupName = "";

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
        foreach(Transform child in listParent)
        {
            TaskObject task = child.GetComponent<TaskObject>();
            if (task.completed)
            {
                ToggleTaskComplete(task);
            }
        }
        foreach (Transform childInGroup in groupListParent)
        {
            TaskObject task = childInGroup.GetComponent<TaskObject>();
            if (task != null && task.completed)
            {
                ToggleTaskComplete(task);
            }
        }
        groupsCompleted.Clear();
    }
    public void ButtonChangeDay(int dayIncrement)
    {
        if (DayIndex + dayIncrement >= 0)
        {
            dataManager.ChangeDay(DayIndex + dayIncrement, DayIndex);
            activeGroupName = "";
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


    void AddTask(string taskName)
    {
        GameObject taskObject = Instantiate(taskPrefab, listParent);
        taskObject.GetComponent<TaskObject>().TaskName = taskName;
    }
    void AddTask(string taskName, string groupName)
    {
        GameObject taskObject = Instantiate(taskPrefab, groupListParent);
        taskObject.GetComponent<TaskObject>().TaskName = taskName;
        taskObject.GetComponent<TaskObject>().groupName = groupName;
    }
    void AddGroup(string groupName)
    {
        GameObject taskObject = Instantiate(groupTaskPrefab, listParent);
        taskObject.GetComponent<TaskObject>().TaskName = groupName;
        taskObject.GetComponent<TaskObject>().groupName = groupName;
    }
    void AddTask(TaskObjectData taskData)
    {
        TaskObject taskscript;
        if (taskData.isGroup)
        {
            taskscript = Instantiate(groupTaskPrefab, listParent).GetComponent<TaskObject>();
            taskscript.TaskName = taskData.groupName;
            if (taskData.completed)
            {
                groupsCompleted.Add(taskData.groupName);
            }
        }
        else
        {
            if (taskData.groupName.Equals(""))
                taskscript = Instantiate(taskPrefab, listParent).GetComponent<TaskObject>();
            else //task is in a group
            {
                taskscript = Instantiate(taskPrefab, groupListParent).GetComponent<TaskObject>();
            }

            taskscript.TaskName = taskData.name;
        }
        taskscript.groupName = taskData.groupName;
        if (taskData.completed)
            ToggleTaskComplete(taskscript);
    }
    public void ConfrimNewTask(string newTaskName)
    {
        if (groupListParent.parent.gameObject.activeSelf)
            AddTask(newTaskName, activeGroupName);
        else
            AddTask(newTaskName);

        UpdateData();
    }
    public void ConfrimNewGroup(string newGroupName)
    {
        AddGroup(newGroupName);

        UpdateData();
    }

    public void RemoveTask(TaskObject task)
    {
        if (task is TaskGroupObject)
        {
            foreach (Transform child in groupListParent)
            {
                TaskObject childScript = child.GetComponent<TaskObject>();
                if (childScript != null && childScript.groupName.Equals(task.groupName))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        Destroy(task.gameObject);
        StartDelayUpdateRoutine(2);
        progressManager.UpdateProgress();
    }
    bool IsGroupCompleted(string groupName)
    {
        foreach (Transform child in groupListParent)
        {
            TaskObject task = child.GetComponent<TaskObject>();
            if (task != null && task.groupName.Equals(groupName) && !task.completed)
            {
                return false;
            }
        }
        return true;
    }
    void ToggleGroupCompleteStatus(string groupName, bool complete)
    {
        if (complete) //group completed
        {
            if (groupsCompleted.Contains(groupName))
                    return; //group is already completed
            groupsCompleted.Add(groupName);
        }
        else //group not completed
        {
            if (!groupsCompleted.Contains(groupName))
                return; //group hasn't been completed yet
            groupsCompleted.Remove(groupName);
        }

        foreach (Transform child in listParent)
        {
            TaskGroupObject task = child.GetComponent<TaskGroupObject>();
            if (task != null && task.TaskName.Equals(groupName))
            {
                task.ButtonToggleTaskStatus();
            }
        }
        progressManager.UpdateProgress();
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

        progressManager.UpdateProgress();
    }
    public void MoveUpTask(Transform task)
    {
        int newIndex = task.GetSiblingIndex();

        while (newIndex > 0 && task.parent.GetChild(newIndex - 1).gameObject.activeSelf is false)
        {
            newIndex--;
        }
        if ((task.parent == listParent && newIndex > 0) || (task.parent == groupListParent && newIndex > 1))
        {
            newIndex--;
        }
        else
            return;

        task.SetSiblingIndex(newIndex);

        UpdateData();
    }

    ///<summary> will not save </summary>
    void RemoveAllTasks()
    {
        foreach(Transform task in listParent)
        {
            task.gameObject.SetActive(false);
            Destroy(task.gameObject);
        }
        foreach (Transform task in groupListParent)
        {
            if (task.GetComponent<TaskObject>() != null)
            {
                task.gameObject.SetActive(false);
                Destroy(task.gameObject);
            }
        }
        groupsCompleted.Clear();
    }

    void UpdateData()
    {
        //gather all data from today's list and save it
        TaskListData list = new TaskListData();
        foreach (Transform child in listParent)
        {
            TaskObject taskObj = child.GetComponent<TaskObject>();
            TaskObjectData task = new TaskObjectData();

            task.name = taskObj.TaskName;
            task.completed = taskObj.completed;
            task.groupName = taskObj.groupName;
            task.isGroup = taskObj is TaskGroupObject;
            list.tasks.Add(task);
        }
        foreach (Transform child in groupListParent)
        {
            TaskObject taskObj = child.GetComponent<TaskObject>();
            if (taskObj != null)
            {
                TaskObjectData task = new TaskObjectData();

                task.name = taskObj.TaskName;
                task.completed = taskObj.completed;
                task.groupName = taskObj.groupName;
                list.tasks.Add(task);
            }
        }

        dataManager.SaveData(list, DayIndex, firstDay);
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
        if (collection == null)
            return;

        RemoveAllTasks();
        //show the task list from "collection" on "dayIndex"
        var taskList = collection.lists[collection.dayIndex].tasks;
        for (int i = 0; i < taskList.Count; i++)
        {
            AddTask(taskList[i]);
        }
        firstDay = collection.firstDay;
        DayIndex = collection.dayIndex;
        SetToday();
        progressManager.UpdateProgress();
    }

    public void ShowList()
    {
        listParent.parent.gameObject.SetActive(true);
        groupListParent.parent.gameObject.SetActive(false);
        addGroup_Button.interactable = true;

        if (!activeGroupName.Equals(""))
        {
            ToggleGroupCompleteStatus(activeGroupName, IsGroupCompleted(activeGroupName));
        }
        progressManager.UpdateProgress();
    }
    public void ShowList(string groupName)
    {
        listParent.parent.gameObject.SetActive(false);
        groupListParent.parent.gameObject.SetActive(true);
        addGroup_Button.interactable = false;

        foreach (Transform child in groupListParent)
        {
            TaskObject task = child.GetComponent<TaskObject>();
            if (task != null)
            {
                task.gameObject.SetActive(task.groupName.Equals(groupName));
            }
        }
        activeGroupName = groupName;
        progressManager.UpdateProgress();
    }
}
