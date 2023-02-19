using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskObject : MonoBehaviour
{
    bool completed;

    public void TaskStatus()
    {
        completed = !completed;

        Image button = transform.GetChild(0).GetComponent<Image>();
        var alpha = button.color;
        alpha.a = completed ? .5f : 1f;
        button.color = alpha;

        if (completed)
            transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        else
            transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().fontStyle -= FontStyles.Strikethrough;
    }

    public void DeleteTask() => Destroy(gameObject);
}
