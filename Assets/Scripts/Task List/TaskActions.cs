using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskActions : MonoBehaviour
{
    protected TaskListManager manager;

    public TaskObjectData taskData;

    private void Start()
    {
        manager = FindObjectOfType<TaskListManager>();
        if (manager == null)
        {
            Debug.LogError("manager not found");
        }
    }

    private void OnEnable()
    {
        TaskListManager.ListChanged += UpdateTask;
    }
    private void OnDisable()
    {
        TaskListManager.ListChanged -= UpdateTask;
    }

    public void UpdateTask()
    {
        transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = taskData.name;

        //darken task when completed
        Image button = transform.GetChild(0).GetComponent<Image>();
        var alphaChange = button.color;
        alphaChange.a = taskData.completed ? .5f : 1f;
        button.color = alphaChange;

        //slash text when completed
        TextMeshProUGUI taskText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        taskText.fontStyle = taskData.completed ? FontStyles.Strikethrough : FontStyles.Bold;
    }

    public void ButtonLayerAction()
    {
        manager.ChangeLayer(taskData);
    }

    public void ButtonToggleTaskStatus()
    {
        manager.CompleteTask(taskData);
    }
    public void ButtonMoveUp() => manager.MoveUpTask(transform);

    public void ButtonRemove() => manager.RemoveTask(this);

}
