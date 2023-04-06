using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskObject : MonoBehaviour
{
    bool completed;
    string taskName;
    public string TaskName
    {
        get => taskName;
        set
        {
            taskName = value;
            transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = value;
        }
    }

    public bool CompleteStatus
    {
        get => completed;
    }

    public void TaskStatus()
    {
        completed = !completed;

        Image button = transform.GetChild(0).GetComponent<Image>();
        var alpha = button.color;
        alpha.a = completed ? .5f : 1f;
        button.color = alpha;

        TextMeshProUGUI taskText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        if (completed)
            taskText.fontStyle = FontStyles.Strikethrough;
        else
            taskText.fontStyle = FontStyles.Bold;
    }

    public void DeleteTask()
    {
        FindObjectOfType<TaskListManager>().RemoveTask(gameObject);
    }

    public void MoveUp()
    {
        int index = transform.GetSiblingIndex();
        if (index > 0)
        {
            transform.SetSiblingIndex(index - 1);
        }
    }
}
