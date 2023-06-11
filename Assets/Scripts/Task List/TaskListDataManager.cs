using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TaskListDataManager : MonoBehaviour
{
    TaskListManager listManager;

    public TaskListCollection currentData;
    [SerializeField] string fileName;

    private void Awake()
    {
        listManager = GetComponent<TaskListManager>();
        TaskListManager.ListChanged += TaskListChanged;
    }
    private void OnDestroy()
    {
        TaskListManager.ListChanged -= TaskListChanged;

    }
    private void Start()
    {
        LoadData();
        Debug.Log("File Location: " + Application.persistentDataPath);
    }

    void TaskListChanged(bool onlyDisplayChange)
    {
        if (onlyDisplayChange)
        {
            return;
        }
        SaveData();
    }

    public void SaveData()
    {
        DeleteData(); //for overwriting

        if (currentData.lists.Count == 0)
            currentData = new TaskListCollection();

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
            SaveData();
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

        collection.lists[dayIndex].rootTasks.Clear();
        //copy "listData" to the "dayIndex" list
        foreach (TaskObjectData task in listData.rootTasks)
        {
            collection.lists[dayIndex].rootTasks.Add(new TaskObjectData(task));
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
        for (int i = 0; i < list.rootTasks.Count; i++)
        {
            //if (TaskListManager.IsInsideLayer(list.rootTasks[i].layerID, layerID))
            //{
            //    list.rootTasks.Remove(list.rootTasks[i]);
            //    i--;
            //}
        }
        //paste in tasks for this layer
        foreach (TaskObjectData task in layerData.rootTasks)
        {
            list.rootTasks.Add(task);
        }
        SaveData();
    }
    public void AddRootLayerTask(TaskObjectData task)
    {
        currentData.lists[currentData.todayIndex].rootTasks.Add(task);
    }
    public void RemoveTask(string taskID)
    {
        //SortListFromTopLayerToBottomLayer(currentData.lists[currentData.todayIndex]);

        TaskListData list = currentData.lists[currentData.todayIndex];
        for (int i = 0; i < list.rootTasks.Count; i++)
        {
            //if (list.rootTasks[i].layerID.Equals(taskID))
            //{
            //    list.rootTasks.Remove(list.rootTasks[i]);
            //    break;
            //}
        }
    }

    void SortListFromTopLayerToBottomLayer(TaskListData list)
    {
        List<TaskObjectData> newOrder = new List<TaskObjectData>();
        int layer = 1; //top layer
        while (newOrder.Count < list.rootTasks.Count)
        {
            //foreach (TaskObjectData task in list.rootTasks)
            //{
            //    if (task.layerID.Length == layer)
            //        newOrder.Add(task);
            //}
            layer++;
        }

        list.rootTasks = newOrder;
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
            foreach (TaskObjectData task in currentData.lists[newDay].rootTasks)
            {
                task.completed = false;
            }
        }
        currentData.todayIndex = newDay;
        listManager.LoadCurrentCollection();
    }
}
