using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    TaskListDataManager dataManager;

    [SerializeField] Transform listParent;
    [SerializeField] Transform groupListParent;

    Slider progressBar;
    TextMeshProUGUI progressText;

    private void Awake()
    {
        dataManager = GetComponent<TaskListDataManager>();

        progressBar = transform.Find("Progress Bar").GetComponent<Slider>();
        progressText = progressBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        TaskListManager.ListChange += UpdateProgress;
    }
    private void OnDestroy()
    {
        TaskListManager.ListChange -= UpdateProgress;

    }

    public void UpdateProgress()
    {
        if (progressBar is null)
            return;

        int total = 0;
        int completed = 0;

        ////tasks that are showing now
        //foreach (Transform task in listParent)
        //{
        //    TaskUI taskScript = task.GetComponent<TaskUI>();
        //    if (taskScript != null)
        //    {
        //        total++;
        //        if (taskScript.Data.completed)
        //            completed++;
        //    }
        //}

        //all tasks in today's list except parent tasks
        List<TaskData> allTasks = dataManager.currentData.lists[dataManager.currentData.dayIndex].tasks;
        foreach (TaskData task in allTasks)
        {
            if (task.child_ID == 0) //not a parent task
            {
                total++;
                if (task.completed)
                    completed++;
            }
        }

        if (total > 0)
        {
            progressBar.maxValue = total;
            progressBar.value = completed;
            int percentage = (int) ((float) completed / total * 100);
            progressText.text = percentage + "%";
        }
        else
        {
            progressBar.maxValue = 1;
            progressBar.value = 0;
            progressText.text = "";
        }
    }
}
