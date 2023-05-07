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

    public void SaveData()
    {
        DeleteData(); //for overwriting

        if (currentData == null)
        {
            currentData = new TaskListCollection();
        }

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

                listManager.LoadCurrentCollection();
            }
        }
    }

    public void FormatData()
    {
        DeleteData();
        currentData = new TaskListCollection();

        listManager.LoadCurrentCollection();
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

        collection.lists[dayIndex].allTasks.Clear();
        //copy "listData" to the "dayIndex" list
        foreach (TaskObjectData task in listData.allTasks)
        {
            collection.lists[dayIndex].allTasks.Add(new TaskObjectData(task));
        }

        return collection;
    }

    public void OverwriteListLayer(TaskListData layerData, string layerID)
    {
        //if "dayIndex" does not have a list, make empty lists for each day until "dayIndex" does have a list
        while (currentData.lists.Count <= currentData.todayIndex)
        {
            currentData.lists.Add(new TaskListData());
        }

        //delete all tasks in this layer
        TaskListData list = currentData.lists[currentData.todayIndex];
        for (int i = 0; i < list.allTasks.Count; i++)
        {
            if (TaskListManager.IsInsideLayer(list.allTasks[i].layerID, layerID))
            {
                list.allTasks.Remove(list.allTasks[i]);
                i--;
            }
        }
        //paste in tasks for this layer
        foreach (TaskObjectData task in layerData.allTasks)
        {
            list.allTasks.Add(task);
        }
        SaveData();
    }
    public void RemoveTask(string taskID)
    {
        //SortListFromTopLayerToBottomLayer(currentData.lists[currentData.todayIndex]);

        TaskListData list = currentData.lists[currentData.todayIndex];
        for (int i = 0; i < list.allTasks.Count; i++)
        {
            if (list.allTasks[i].layerID.Equals(taskID))
            {
                list.allTasks.Remove(list.allTasks[i]);
                break;
            }
        }
    }

    void SortListFromTopLayerToBottomLayer(TaskListData list)
    {
        List<TaskObjectData> newOrder = new List<TaskObjectData>();
        int layer = 1; //top layer
        while (newOrder.Count < list.allTasks.Count)
        {
            foreach (TaskObjectData task in list.allTasks)
            {
                if (task.layerID.Length == layer)
                    newOrder.Add(task);
            }
            layer++;
        }

        list.allTasks = newOrder;
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
            foreach (TaskObjectData task in currentData.lists[newDay].allTasks)
            {
                task.completed = false;
            }
        }
        currentData.todayIndex = newDay;
        listManager.LoadCurrentCollection();
    }
}
