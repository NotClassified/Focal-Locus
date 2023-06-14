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


    public void ResetTasks()
    {
        //foreach(Transform child in listParent)
        //{
        //    TaskObject task = child.GetComponent<TaskObject>();
        //    if (task.completed)
        //    {
        //        ToggleTaskComplete(task);
        //    }
        //}


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


    //void AddTask(string taskName)
    //{
    //    GameObject taskObject = Instantiate(taskPrefab, listParent);
    //    taskObject.GetComponent<TaskObject>().TaskName = taskName;
    //////}
    //void AddTask(TaskObjectData taskData)
    //{
    //    TaskObject taskscript;
    ////}
    //public void ConfrimNewTask(string newTaskName)
    //{
    //    AddTask(newTaskName);

    //    UpdateData();
    //}

    //public void RemoveTask(TaskObject task)
    //{


    //    Destroy(task.gameObject);
    //    StartDelayUpdateRoutine(2);
    //    progressManager.UpdateProgress();
    //}

    //public void ToggleTaskComplete(TaskUI task)
    //{
    //    task.completed = !task.completed;
    //    //darken task when completed
    //    Image button = task.transform.GetChild(0).GetComponent<Image>();
    //    var alpha = button.color;
    //    alpha.a = task.completed ? .5f : 1f;
    //    button.color = alpha;

    //    //slash text when completed
    //    TextMeshProUGUI taskText = task.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
    //    if (task.completed)
    //        taskText.fontStyle = FontStyles.Strikethrough;
    //    else
    //        taskText.fontStyle = FontStyles.Bold;

    //    progressManager.UpdateProgress();
    //}
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
        //gather all data from today's list and save it
        //dataManager.SaveData(list, DayIndex, firstDay);
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
            //AddTask(taskList[i]);
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
}
