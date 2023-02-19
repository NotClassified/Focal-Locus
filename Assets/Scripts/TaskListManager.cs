using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskListManager : MonoBehaviour
{
    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;

    public void AddTask(string taskName)
    {
        GameObject task = Instantiate(taskPrefab, listParent);
        task.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = taskName;
    }
}
