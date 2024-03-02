using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class TaskListDataManager : MonoBehaviour
{
    TaskListManager listManager;

    public TaskListCollection currentData = new TaskListCollection();
    [SerializeField] string fileName;

    TaskData lastDeletedTask;

    private void Awake()
    {
        listManager = GetComponent<TaskListManager>();
    }
    private void Start()
    {
        ReadData();
        Debug.Log("File Location: " + Application.persistentDataPath);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            ConvertDataToJson(true);
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        { 
            using (FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + ".json",
                                                 FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ConvertFileToJson(reader, true);
                }
            }
        }
    }

    public void SaveData()
    {
        DeleteDataFile(); //for overwriting

        using (FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + ".json", 
                                             FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(ConvertDataToJson(false));
                writer.Flush(); // applies the changes to the file

                //Debug.Log(Application.persistentDataPath + "/" + fileName + ".json");
            }
        }
    }

    public void ReadData()
    {
        if (!DataExists())
        {
            Debug.LogWarning("No data");
            return;
        }

        using (FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + ".json", 
                                             FileMode.Open, FileAccess.ReadWrite))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                currentData = JsonUtility.FromJson<TaskListCollection>(ConvertFileToJson(reader, false));
                LoadData();
            }
        }
    }
    void LoadData()
    {
        listManager.LoadCollection();
    }
    string ConvertDataToJson(bool print)
    {
        string json = JsonUtility.ToJson(currentData, true);
        if (print)
            Debug.Log("Serialized data: \n" + json);

        return json;
    }
    string ConvertFileToJson(StreamReader reader, bool print)
    {
        string json = reader.ReadToEnd();
        if (print)
            Debug.Log("Previously Saved Data: \n" + json);

        return json;
    }

    public void FormatData()
    {
        DeleteDataFile();
        currentData = new TaskListCollection();
        LoadData(); //show empty collection
    }
    void DeleteDataFile()
    {
        if (!DataExists())
            return;

        File.Delete(Application.persistentDataPath + "/" + fileName + ".json");
    }

    bool DataExists() => File.Exists(Application.persistentDataPath + "/" + fileName + ".json");

    public void AddTask(TaskData newTask)
    {
        currentData.lists[currentData.dayIndex].tasks.Add(newTask);
        SaveData();
    }
    public void AddLastDeletedTask(bool isChild)
    {
        if (lastDeletedTask == null)
            return;

        if (isChild)
            listManager.ConfirmChildTask(lastDeletedTask.name);
        else
            listManager.ConfirmNewTask(lastDeletedTask.name);
    }
    public void DeleteTask(TaskData task)
    {
        lastDeletedTask = task;

        DeleteChildren(task);

        //change the parent's child reference, if this task is the child reference
        if (TryFindTask(task.parent_ID, out TaskData parent) && parent.child_ID == task.id)
        {
            if (task.prevSibling_ID != 0)
            {
                parent.child_ID = task.prevSibling_ID;
            }
            else if (task.nextSibling_ID != 0)
            {
                parent.child_ID = task.nextSibling_ID;
            }
            else //there are no siblings, so this parent will no longer be a parent
            {
                parent.child_ID = 0;
            }
        }
        RemoveAndExchangeSiblingIDs(task);

        currentData.lists[currentData.dayIndex].tasks.Remove(task);
        SaveData();
    }
    public void RemoveAndExchangeSiblingIDs(TaskData task)
    {
        TaskData nextSibling;
        if (TryFindTask(task.nextSibling_ID, out nextSibling))
            nextSibling.prevSibling_ID = 0;

        TaskData previousSibling;
        if (TryFindTask(task.prevSibling_ID, out previousSibling))
            previousSibling.nextSibling_ID = 0;

        if (nextSibling != null && previousSibling != null)
        {
            //exchange IDs to siblings
            nextSibling.prevSibling_ID = previousSibling.id;
            previousSibling.nextSibling_ID = nextSibling.id;
        }
    }
    void DeleteChildren(TaskData parent)
    {
        if (parent.child_ID == 0)
            return; //"parent" has no children

        List<TaskData> allTasks = currentData.lists[currentData.dayIndex].tasks;
        for (int i = 0; i < allTasks.Count; i++)
        {
            if (allTasks[i].parent_ID == parent.id)
            {
                DeleteChildren(allTasks[i]);
                currentData.lists[currentData.dayIndex].tasks.Remove(allTasks[i--]);
            }
        }
    }
    public TaskData FindTask(int searchID)
    {
        TaskData foundTask;
        if (!TryFindTask(searchID, out foundTask))
            Debug.LogError("couldn't find task with ID: " + searchID);

        return foundTask;
    }
    public bool TryFindTask(int searchID, out TaskData foundTask)
    {
        foundTask = null;

        if (searchID == 0) //There is no task ID that is 0
            return false;

        //binary search:
        List<TaskData> searchingList = currentData.lists[currentData.dayIndex].tasks;
        int left = 0;
        int right = searchingList.Count - 1;
        int mid;
        while (left <= right)
        {
            mid = (left + right) / 2;

            if (searchingList[mid].id == searchID)
            {
                foundTask = searchingList[mid];
                return true; //task found
            }
            else if (searchingList[mid].id < searchID)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return false; //task not found
    }
    public TaskData FindFirstSibling(int siblingID) => FindFirstSibling(FindTask(siblingID));
    public TaskData FindFirstSibling(TaskData sibling)
    {
        if (sibling.prevSibling_ID != 0) //is there a sibling before?
            return FindFirstSibling(FindTask(sibling.prevSibling_ID));

        else //this is the first sibling
            return sibling;
    }
    public TaskData FindFirstRootSibling()
    {
        foreach (TaskData task in currentData.lists[currentData.dayIndex].tasks)
        {
            if (task.parent_ID == 0)
                 return FindFirstSibling(task);
        }
        Debug.LogWarning("There are no root tasks");
        return null;
    }

    public List<TaskData> GetTodaysList()
    {
        return currentData.lists[currentData.dayIndex].tasks;
    }
    TaskListCollection SaveListToCollection(TaskListCollection collection, TaskListData listData, int dayIndex)
    {
        if (dayIndex < collection.lists.Count) //this day has a list, overwrite the list with "listData"
        {
            collection.lists[dayIndex] = new TaskListData(listData.tasks);
            return collection;
        }
        else //this day has NO list, make an empty list:
        {
            collection.lists.Add(new TaskListData());
            return SaveListToCollection(collection, listData, dayIndex);
        }
    }

    public void ChangeDay(int newDay, int oldDay)
    {
        if (currentData.lists.Count <= oldDay)
            return;

        //copy list from "oldDay" to "newDay" if "newDay" doesn't have a list
        if (currentData.lists.Count <= newDay) 
        {
            currentData = SaveListToCollection(currentData, currentData.lists[oldDay], newDay);
            //uncomplete all tasks on the new day
            foreach (TaskData task in currentData.lists[newDay].tasks)
                task.completed = false;
        }
        currentData.dayIndex = newDay;
        SaveData();

        LoadData();

    }
    public void ChangeFirstDay(DaysOfWeek firstDay)
    {
        currentData.firstDay = firstDay;
        SaveData();
        LoadData();
    }
    public void DeleteToday()
    {
        if (currentData.lists.Count <= 1)
            return;

        int todayIndex = currentData.lists.Count - 1;
        currentData.lists.RemoveAt(todayIndex);

        //load the new today
        todayIndex = currentData.lists.Count - 1;
        currentData.dayIndex = todayIndex;

        SaveData();
        LoadData();
    }
    public void DeleteThisDay()
    {
        if (currentData.lists.Count <= 1)
            return;

        currentData.lists.RemoveAt(currentData.dayIndex);

        //load the day before the deleted day
        if (currentData.dayIndex != 0)
            currentData.dayIndex -= currentData.dayIndex;

        SaveData();
        LoadData();
    }
}
