using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDownClick : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TaskUI taskUI;

    public void OnPointerDown(PointerEventData eventData)
    {
        taskUI.Move();
    }
}
