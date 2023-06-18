using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TaskListDataManager : MonoBehaviour
{
    public TaskListCollection currentData = new TaskListCollection();
    [SerializeField] string fileName;

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
        DeleteData(); //for overwriting

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
        GetComponent<TaskListManager>().LoadCollection(currentData);
        TaskIDManager.SetNewestID(currentData.newestTaskID);
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
        DeleteData();

        currentData = new TaskListCollection();
        SaveData();
        LoadData();
    }
    void DeleteData()
    {
        if (!DataExists())
            return;

        File.Delete(Application.persistentDataPath + "/" + fileName + ".json");
    }

    bool DataExists() => File.Exists(Application.persistentDataPath + "/" + fileName + ".json");

    public void AddTask(TaskData newTask)
    {
        currentData.lists[currentData.dayIndex].tasks.Add(newTask);
        currentData.newestTaskID = newTask.id;
        SaveData();
    }
    public void RemoveTask(TaskData task)
    {
        currentData.lists[currentData.dayIndex].tasks.Remove(task);
        SaveData();
    }
    public TaskData FindTask(int searchID)
    {
        foreach (TaskData task in currentData.lists[currentData.dayIndex].tasks)
        {
            if (task.id == searchID)
            {
                return task;
            }
        }
        Debug.LogError("couldn't find task");
        return null;
    }

    TaskListCollection SaveListToCollection(TaskListCollection collection, TaskListData listData, int dayIndex)
    {
        if (dayIndex < collection.lists.Count) //this day has a list, overwrite the list with "listData"
        {
            collection.lists[dayIndex] = new TaskListData(listData.tasks);
            print(listData.tasks[0].GetHashCode());
            print(collection.lists[dayIndex].tasks[0].GetHashCode());
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

        GetComponent<TaskListManager>().LoadCollection(currentData);

    }
}
