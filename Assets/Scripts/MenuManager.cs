using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class MenuManager : ScreenState
{
    [SerializeField] GameObject buttonPrefab;
    Transform list;

    private void Start()
    {
        list = transform.GetChild(0);

        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                GameObject button = Instantiate(buttonPrefab, list);
                button.name = child.name;
                button.GetComponentInChildren<TextMeshProUGUI>().text = child.name;

                Type childType = child.GetComponent<ScreenState>().GetType();
                button.GetComponent<Button>().onClick.AddListener(delegate { ScreenManager.instance.ChangeState(childType); });
            }
        }
    }

}
