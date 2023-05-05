using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TaskListDataManager : MonoBehaviour
{
    TaskListManager listManager;

    public TaskListCollection currentData = new TaskListCollection();
    [SerializeField] string fileName;

    private void Awake()
    {
        listManager = GetComponent<TaskListManager>();
    }
    private void Start()
    {
        LoadData();
        Debug.Log("File Location: " + Application.persistentDataPath);
    }

    public void SaveData(TaskListData listData, int dayIndex, DaysOfWeek firstDay)
    {
        DeleteData(); //for overwriting

        currentData = SaveListToCollection(currentData, listData, dayIndex);
        currentData.firstDay = firstDay;

        string json = JsonUtility.ToJson(currentData, true);
        Debug.Log("Serialized data: \n" + json);
        using (FileStream stream = File.Open(Application.persistentDataPath + "/" + fileName + ".json", 
                                             FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(json);
                writer.Flush(); // applies the changes to the file

                //Debug.Log(Application.persistentDataPath + "/" + fileName + ".json");
            }
        }
    }

    public void LoadData()
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
                string json = reader.ReadToEnd();
                Debug.Log("Previously Saved Data: \n" + json);
                currentData = JsonUtility.FromJson<TaskListCollection>(json);

                listManager.LoadCollection(currentData);
            }
        }
    }

    public void FormatData()
    {
        DeleteData();
        currentData = new TaskListCollection();

        listManager.LoadCollection(currentData);
    }
    void DeleteData()
    {
        if (!DataExists())
            return;

        File.Delete(Application.persistentDataPath + "/" + fileName + ".json");
    }

    bool DataExists() => File.Exists(Application.persistentDataPath + "/" + fileName + ".json");

    TaskListCollection SaveListToCollection(TaskListCollection collection, TaskListData listData, int dayIndex)
    {
        //if "dayIndex" does not have a list, make empty lists for each day until "dayIndex" does have a list
        while (collection.lists.Count <= dayIndex)
        {
            collection.lists.Add(new TaskListData());
        }

        TaskListData listCopy = collection.lists[dayIndex];
        //copy "listData" to "listCopy" ("dayIndex" list)
        listCopy.tasks = new TaskObjectData[listData.tasks.Length];
        for (int i = 0; i < listCopy.tasks.Length; i++)
        {
            listCopy.tasks[i] = new TaskObjectData(listData.tasks[i].name, 
                                                     listData.tasks[i].completed, 
                                                       listData.tasks[i].parent);
        }
        return collection;
    }

    public void ChangeDay(int newDay, int oldDay)
    {
        if (currentData.lists.Count <= oldDay)
        {
            Debug.LogError("the list for \"oldDay\" does not exist");
            return;
        }


        //if "newDay" doesn't have a list, copy list from "oldDay" to "newDay" and reset all "newDay" tasks
        if (currentData.lists.Count <= newDay) 
        {
            currentData = SaveListToCollection(currentData, currentData.lists[oldDay], newDay);
            foreach (TaskObjectData task in currentData.lists[newDay].tasks)
            {
                task.completed = false;
            }
        }
        currentData.todayIndex = newDay;
        listManager.LoadCollection(currentData);
    }
}
