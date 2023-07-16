using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TaskItem : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform childrenContent;


    void Update()
    {
        if (rectTransform.sizeDelta.x != childrenContent.sizeDelta.x)
        {
            var newSize = rectTransform.sizeDelta;
            newSize.x = childrenContent.sizeDelta.x;

            rectTransform.sizeDelta = newSize;
        }
    }
}
