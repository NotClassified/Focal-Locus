using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public enum Prompt
{
    Cancel, Confirm, AddTask, AddChild, ChangeFirstDay, FormatData, GoToToday
}

public class PromptManager : MonoBehaviour
{
    TaskListManager taskManager;
    TaskListDataManager dataTaskManager;

    [SerializeField] GameObject promptParent;
    Prompt activePrompt;

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
        option--; //skip the blank option

        switch (activePrompt)
        {
            case Prompt.ChangeFirstDay:
                int dayIndex = (option - taskManager.DayIndex) % TaskListManager.amountOfDaysInAWeek;
                while (dayIndex < 0)
                {
                    dayIndex += TaskListManager.amountOfDaysInAWeek;
                }
                taskManager.firstDay = (DaysOfWeek) dayIndex;
                //taskManager.StartDelayUpdateRoutine(2);
                break;
            default:
                Debug.LogError("This prompt doesn't have dropdown options: " + option);
                break;
        }
    }

    public void PromptAction(PromptComponent component)
    {
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
                    case Prompt.FormatData:
                        dataTaskManager.FormatData();
                        break;
                    case Prompt.GoToToday:
                        dataTaskManager.ChangeDay(dataTaskManager.currentData.lists.Count - 1, 0);
                        break;
                }
                taskManager.SetToday();
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
            case Prompt.AddTask:
                placeHolder = "Task Name...";
                break;
            case Prompt.AddChild:
                placeHolder = "Child Task Name...";
                break;
            default:
                inputField.gameObject.SetActive(false);
                return; //This UI not needed
        }
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
            case Prompt.FormatData:
                question = "Are you sure you want to Format Data?";
                break;
            case Prompt.GoToToday:
                question = "Go to Today's List?";
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
        options.Add("");

        switch (prompt)
        {
            case Prompt.ChangeFirstDay:
                for (int i = 0; i < System.Enum.GetValues(typeof(DaysOfWeek)).Length; i++)
                {
                    DaysOfWeek day = (DaysOfWeek) i;
                    options.Add(day.ToString());
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
