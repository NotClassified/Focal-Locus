using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptComponent : MonoBehaviour
{
    public Prompt prompt;

    private void Awake()
    {

        GetComponent<Button>().onClick.AddListener(
            delegate { 
                FindObjectOfType<PromptManager>().PromptAction(this); 
            }) ;
    }
}
