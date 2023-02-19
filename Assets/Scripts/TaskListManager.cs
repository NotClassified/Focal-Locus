using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskListManager : MonoBehaviour
{
    [SerializeField] Transform listParent;
    [SerializeField] GameObject taskPrefab;
    [SerializeField] GameObject newTaskPrompt;
    string newTaskName;

    private void Start()
    {
        NewTaskPrompt(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddTask();
            NewTaskPrompt(false);
        }
    }

    void AddTask()
    {
        GameObject task = Instantiate(taskPrefab, listParent);
        task.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = newTaskName;
    }

    public void NewTaskPrompt(bool active)
    {
        newTaskPrompt.SetActive(active);
        newTaskPrompt.GetComponentInChildren<TMP_InputField>().text = "";
    }

    public void SetNewTaskName(string name) => newTaskName = name;

}
