using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] Transform listParent;
    [SerializeField] Transform groupListParent;

    Slider progressBar;
    TextMeshProUGUI progressText;

    private void Awake()
    {
        progressBar = transform.Find("Progress Bar").GetComponent<Slider>();
        progressText = progressBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateProgress()
    {
        if (progressBar is null)
            return;

        int total = 0;
        int completed = 0;

        foreach (Transform task in listParent)
        {
            TaskUI taskScript = task.GetComponent<TaskUI>();
            if (taskScript != null)
            {
                total++;
                if (taskScript.Data.completed)
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
