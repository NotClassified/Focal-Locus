using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptComponent : MonoBehaviour
{
    public Prompt prompt;
    public object extraData;

    private void Awake()
    {
        switch (prompt)
        {
            case Prompt.DeleteTask:
                extraData = GetComponentInParent<TaskUI>();
                break;
            case Prompt.TaskOptions:
                extraData = GetComponentInParent<TaskUI>();
                break;
        }

        GetComponent<Button>().onClick.AddListener(
            delegate { 
                FindObjectOfType<PromptManager>().PromptAction(this, extraData); 
            }) ;
    }
}
