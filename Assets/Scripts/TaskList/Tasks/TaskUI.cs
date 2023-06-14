using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskUI : MonoBehaviour
{
    private TaskData _data;
    public TaskData Data
    {
        get => _data;
        set
        {
            _data = value;
            UpdateUI(DataProperties.All);
        }
    }

    [SerializeField] TextMeshProUGUI name_Text;
    [SerializeField] Image name_ButtonImage;
    [SerializeField] GameObject addChildButton_Obj;

    private TaskListManager _manager;
    private TaskListManager Manager
    {
        get
        {
            if (_manager is null)
                _manager = FindObjectOfType<TaskListManager>();

            return _manager;
        }
    }

    public void MainAction() => Manager.TaskMainAction(this);
    public void AddChild() => Manager.TaskAddChild(this);
    public void Remove() => Manager.RemoveTask(this);

    public void MoveUp() => Manager.MoveUpTask(transform);

    public enum DataProperties
    {
        All, Name, CompleteStatus, ParentStatus
    }

    public void UpdateUI(DataProperties updateProperty)
    {
        switch (updateProperty)
        {
            case DataProperties.All:
                UpdateUI(DataProperties.Name);
                UpdateUI(DataProperties.CompleteStatus);
                UpdateUI(DataProperties.ParentStatus);
                break;

            case DataProperties.Name:
                name_Text.text = Data.name;
                break;

            case DataProperties.CompleteStatus:
                //darken task when completed
                var alpha = name_ButtonImage.color;
                alpha.a = Data.completed ? .5f : 1f;
                name_ButtonImage.color = alpha;

                //slash text when completed
                if (Data.completed)
                    name_Text.fontStyle = FontStyles.Strikethrough;
                else
                    name_Text.fontStyle = FontStyles.Bold;
                break;

            case DataProperties.ParentStatus:
                addChildButton_Obj.SetActive(Data.child_ID == 0);
                break;

            default:
                Debug.LogError(updateProperty.ToString() + " (this condition has not been set up)");
                break;
        }
    }
}
