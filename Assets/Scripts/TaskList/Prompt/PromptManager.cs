using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum Prompt
{
    Cancel, Confirm, AddTask, AddChild, ChangeFirstDay, DeleteAction, GoToToday, DeleteTask, TaskOptions, EditName
}
public enum DeleteActionOptions
{
    DeleteToday, DeleteThisDay, FormatData
}
public enum AddTaskOptions
{
    CreateNew, AddLastDeleted
}
public enum TaskOptions
{
    EditName, RemoveTask
}

public class PromptManager : MonoBehaviour
{
    TaskListManager taskManager;
    TaskListDataManager dataTaskManager;

    [SerializeField] GameObject promptParent;
    Prompt activePrompt;
    object activeExtraData;

    TMP_InputField inputField;
    string inputFieldValue;

    TMP_Dropdown dropDown;
    TextMeshProUGUI question_Text;

    System.Action DeleteOptionMethod;
    System.Action AddTaskOptionMethod;

    private void Awake()
    {
        promptParent.SetActive(false);
        taskManager = GetComponent<TaskListManager>();
        dataTaskManager = GetComponent<TaskListDataManager>();

        inputField = promptParent.transform.Find("inputField").GetComponent<TMP_InputField>();
        dropDown = promptParent.transform.Find("dropDown").GetComponent<TMP_Dropdown>();
        question_Text = promptParent.transform.Find("question_Text").GetComponent<TextMeshProUGUI>();
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.gameObject.activeInHierarchy)
        {
            PromptAction(Prompt.Confirm);
        }
    }

    public void PromptInputField(string input) => inputFieldValue = input;
    public void PromptDropDown(int option)
    {
        option--; //skip the blank/title option
        bool lastPromptAction = true; //disables all prompts, invoking "Confrim" isn't needed

        switch (activePrompt)
        {
            case Prompt.ChangeFirstDay:
                int dayIndex = (option - taskManager.DayIndex) % TaskListManager.amountOfDaysInAWeek;
                while (dayIndex < 0)
                {
                    dayIndex += TaskListManager.amountOfDaysInAWeek;
                }
                dataTaskManager.ChangeFirstDay((DaysOfWeek)dayIndex);
                break;
            case Prompt.DeleteAction:
                DeleteActionOptions deleteOption = (DeleteActionOptions)option;
                switch (deleteOption)
                {
                    case DeleteActionOptions.DeleteToday:
                        DeleteOptionMethod = () => dataTaskManager.DeleteToday();
                        lastPromptAction = false; //wait for "Confrim" to be invoked
                        break;
                    case DeleteActionOptions.DeleteThisDay:
                        DeleteOptionMethod = () => dataTaskManager.DeleteThisDay();
                        lastPromptAction = false; //wait for "Confrim" to be invoked
                        break;
                    case DeleteActionOptions.FormatData:
                        DeleteOptionMethod = () => dataTaskManager.FormatData();
                        lastPromptAction = false; //wait for "Confrim" to be invoked
                        break;
                    default:
                        Debug.LogError("This delete option hasn't been implemented: " + deleteOption);
                        break;
                }
                break;
            case Prompt.AddTask:
                AddTaskOptions addOption = (AddTaskOptions)option;
                switch (addOption)
                {
                    case AddTaskOptions.CreateNew:
                        AssignInputFieldPlaceHolder("Task Name...");

                        dropDown.gameObject.SetActive(false);
                        lastPromptAction = false; //needs to show another prompt
                        break;
                    case AddTaskOptions.AddLastDeleted:
                        dataTaskManager.AddLastDeletedTask(isChild: false);
                        break;
                    default:
                        Debug.LogError("This delete option hasn't been implemented: " + addOption);
                        break;
                }
                break;
            case Prompt.AddChild:
                addOption = (AddTaskOptions)option;
                switch (addOption)
                {
                    case AddTaskOptions.CreateNew:
                        AssignInputFieldPlaceHolder("Child Task Name...");

                        dropDown.gameObject.SetActive(false);
                        lastPromptAction = false; //needs to show another prompt
                        break;
                    case AddTaskOptions.AddLastDeleted:
                        dataTaskManager.AddLastDeletedTask(isChild: true);
                        break;
                    default:
                        Debug.LogError("This delete option hasn't been implemented: " + addOption);
                        break;
                }
                break;
            case Prompt.TaskOptions:
                TaskOptions taskOption = (TaskOptions)option;
                switch (taskOption)
                {
                    case TaskOptions.EditName:
                        activePrompt = Prompt.EditName;
                        AssignInputFieldPlaceHolder("Edit Task Name...");

                        dropDown.gameObject.SetActive(false);
                        break;
                    case TaskOptions.RemoveTask:
                        activePrompt = Prompt.DeleteTask;
                        AssignQuestion(Prompt.DeleteTask);

                        dropDown.gameObject.SetActive(false);
                        lastPromptAction = false; //needs to show another prompt
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
                lastPromptAction = false; //needs to show another prompt
                break;
            default:
                Debug.LogError("This prompt doesn't have dropdown options: " + activePrompt);
                break;
        }
        //disbale all prompts if there are not any new prompts expected after this drop down prompt
        promptParent.SetActive(!lastPromptAction);
    }

    public void PromptAction(PromptComponent component, object extraData)
    {
        if (extraData != null)
            activeExtraData = extraData;

        PromptAction(component.prompt);
    }
    public void PromptAction(Prompt prompt)
    {
        switch (prompt)
        {
            case Prompt.Cancel:
                if (activePrompt is Prompt.AddChild)
                {
                    taskManager.DeconfirmChildTask();
                }
                promptParent.SetActive(false);
                break;
            case Prompt.Confirm:
                switch (activePrompt)
                {
                    case Prompt.AddTask:
                        taskManager.ConfirmNewTask(inputFieldValue);
                        break;
                    case Prompt.AddChild:
                        taskManager.ConfirmChildTask(inputFieldValue);
                        break;
                    case Prompt.DeleteAction:
                        DeleteOptionMethod?.Invoke();
                        break;
                    case Prompt.GoToToday:
                        dataTaskManager.ChangeDay(dataTaskManager.currentData.lists.Count - 1, 0);
                        break;
                    case Prompt.DeleteTask:
                        taskManager.RemoveTask((TaskUI) activeExtraData);
                        break;
                    case Prompt.EditName:
                        taskManager.ConfirmNewTaskName((TaskUI)activeExtraData, inputFieldValue);
                        break;
                }
                promptParent.SetActive(false);
                break;
            default: //Open Prompt
                activePrompt = prompt;

                AssignInputFieldPlaceHolder(prompt);
                AssignQuestion(prompt);
                AssignDropDown(prompt);

                promptParent.SetActive(true);
                break;
        }
    }

    void AssignInputFieldPlaceHolder(Prompt prompt)
    {
        string placeHolder;
        switch (prompt)
        {
            default:
                inputField.gameObject.SetActive(false);
                return; //This UI not needed
        }
#pragma warning disable CS0162 // Unreachable code detected
        AssignInputFieldPlaceHolder(placeHolder);
#pragma warning restore CS0162 // Unreachable code detected
    }
    void AssignInputFieldPlaceHolder(string placeHolder)
    {
        inputField.text = "";
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = placeHolder;
        inputField.gameObject.SetActive(true);
        Invoke("FocusOnInputField", .1f);
    }
    void FocusOnInputField() => inputField.Select();

    void AssignQuestion(Prompt prompt)
    {
        string question;
        switch (prompt)
        {
            case Prompt.GoToToday:
                question = "Go to Today's List?";
                break;
            case Prompt.DeleteTask:
                question = "Delete Task?";
                break;
            default:
                question_Text.gameObject.SetActive(false);
                return; //This UI not needed
        }
        question_Text.text = question;
        question_Text.gameObject.SetActive(true);
    }

    void AssignDropDown(Prompt prompt)
    {
        List<string> options = new List<string>();
        options.Add(""); //non-option

        switch (prompt)
        {
            case Prompt.ChangeFirstDay:
                options[0] = "Change this Day of the Week";
                for (int i = 0; i < System.Enum.GetValues(typeof(DaysOfWeek)).Length; i++)
                {
                    DaysOfWeek day = (DaysOfWeek)i;
                    options.Add(day.ToString());
                }
                break;
            case Prompt.DeleteAction:
                options[0] = "Choose Delete Action";
                for (int i = 0; i < System.Enum.GetValues(typeof(DeleteActionOptions)).Length; i++)
                {
                    DeleteActionOptions deleteOption = (DeleteActionOptions)i;
                    options.Add(deleteOption.ToString());
                }
                break;
            case Prompt.AddTask:
                options[0] = "Choose Task Add Option";
                for (int i = 0; i < System.Enum.GetValues(typeof(AddTaskOptions)).Length; i++)
                {
                    AddTaskOptions addOption = (AddTaskOptions)i;
                    options.Add(addOption.ToString());
                }
                break;
            case Prompt.AddChild:
                options[0] = "Choose Child Add Option";
                for (int i = 0; i < System.Enum.GetValues(typeof(AddTaskOptions)).Length; i++)
                {
                    AddTaskOptions addOption = (AddTaskOptions)i;
                    options.Add(addOption.ToString());
                }
                break;
            case Prompt.TaskOptions:
                options[0] = "Choose Task Option";
                for (int i = 0; i < System.Enum.GetValues(typeof(TaskOptions)).Length; i++)
                {
                    TaskOptions addOption = (TaskOptions)i;
                    options.Add(addOption.ToString());
                }
                break;
            default:
                dropDown.gameObject.SetActive(false);
                return; //This UI not needed
        }

        dropDown.ClearOptions();
        dropDown.AddOptions(options);
        dropDown.gameObject.SetActive(true);
    }
}
