using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum Prompt
{
    Cancel, Confirm, AddTask, AddChild, ChangeFirstDay, DeleteAction, GoToToday, DeleteTask,
}
public enum DeleteActionOptions
{
    DeleteToday, FormatData
}
public enum AddTaskOptions
{
    CreateNew, AddLastDeleted
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

    private void Awake()
    {
        promptParent.SetActive(false);
        taskManager = GetComponent<TaskListManager>();
        dataTaskManager = GetComponent<TaskListDataManager>();

        inputField = promptParent.transform.Find("inputField").GetComponent<TMP_InputField>();
        dropDown = promptParent.transform.Find("dropDown").GetComponent<TMP_Dropdown>();
        question_Text = promptParent.transform.Find("question_Text").GetComponent<TextMeshProUGUI>();
        
    }

    public void PromptInputField(string input) => inputFieldValue = input;
    public void PromptDropDown(int option)
    {
        option--; //skip the blank/title option
        bool lastPromptAction = true; //disables all prompts, should be set to false if another prompt is expected

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
                        dataTaskManager.DeleteToday();
                        break;
                    case DeleteActionOptions.FormatData:
                        dataTaskManager.FormatData();
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
                        lastPromptAction = false; //show input field prompt
                        break;
                    case AddTaskOptions.AddLastDeleted:
                        dataTaskManager.AddLastDeletedTask();
                        break;
                    default:
                        Debug.LogError("This delete option hasn't been implemented: " + addOption);
                        break;
                }
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
        if (extraData is not null)
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
                        taskManager.ConfrimNewTask(inputFieldValue);
                        break;
                    case Prompt.AddChild:
                        taskManager.ConfrimChildTask(inputFieldValue);
                        break;
                    case Prompt.DeleteAction:
                        dataTaskManager.FormatData();
                        break;
                    case Prompt.GoToToday:
                        dataTaskManager.ChangeDay(dataTaskManager.currentData.lists.Count - 1, 0);
                        break;
                    case Prompt.DeleteTask:
                        taskManager.RemoveTask((TaskUI) activeExtraData);
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
            case Prompt.AddChild:
                placeHolder = "Child Task Name...";
                break;
            default:
                inputField.gameObject.SetActive(false);
                return; //This UI not needed
        }
        AssignInputFieldPlaceHolder(placeHolder);
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
            default:
                dropDown.gameObject.SetActive(false);
                return; //This UI not needed
        }

        dropDown.ClearOptions();
        dropDown.AddOptions(options);
        dropDown.gameObject.SetActive(true);
    }
}
