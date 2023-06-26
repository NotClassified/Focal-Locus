using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenState : MonoBehaviour
{
    public virtual void OnEnter()
    {
        Activate(true);
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit()
    {
        Activate(false);
    }

    public void Activate(bool active) => gameObject.SetActive(active);
}
