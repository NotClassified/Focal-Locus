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

                GetComponent<TaskListManager>().LoadCollection(currentData);
            }
        }
    }

    public void FormatData()
    {
        DeleteData();
        currentData = new TaskListCollection();
        SaveData(new TaskListData(), 0, 0);
        GetComponent<TaskListManager>().LoadCollection(currentData);
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
        if (dayIndex < collection.lists.Count) //this day has a list, overwrite the list with "listData"
        {
            collection.lists[dayIndex] = listData;
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

        bool newDayCreated = false;

        //copy list from "oldDay" to "newDay" if "newDay" doesn't have a list
        if (currentData.lists.Count <= newDay) 
        {
            currentData = SaveListToCollection(currentData, currentData.lists[oldDay], newDay);
            newDayCreated = true;
        }
        currentData.dayIndex = newDay;
        GetComponent<TaskListManager>().LoadCollection(currentData);

        if (newDayCreated)
            GetComponent<TaskListManager>().ResetTasks();
    }
}
