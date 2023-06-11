using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] Transform listParent;

    Slider progressBar;
    TextMeshProUGUI progressText;

    private void Awake()
    {
        progressBar = transform.Find("Progress Bar").GetComponent<Slider>();
        progressText = progressBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        TaskListManager.ListChanged += ListChanged;
    }
    private void OnDestroy() => TaskListManager.ListChanged -= ListChanged;

    void ListChanged(bool onlyDisplayChange) => UpdateProgress();

    public void UpdateProgress()
    {
        int total = 0;
        int completed = 0;

        foreach (TaskActions task in listParent.GetComponentsInChildren<TaskActions>())
        {
            if (task.gameObject.activeSelf)
            {
                total++;
                if (task.taskData.completed)
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
