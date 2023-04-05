using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    public List<ScreenState> screens = new List<ScreenState>();

    private ScreenState currentState;

    private void Awake()
    {
        instance = this;

        foreach (ScreenState screen in screens)
        {
            screen.Activate(false);
        }
    }

    private void Start()
    {
        ChangeState(screens[0].GetType());

    }

    public void ChangeState(Type stateType)
    {
        ScreenState next = FindScreen(stateType);

        if (currentState != null)
        {
            currentState.OnExit();
        }
        next.OnEnter();

        currentState = next;
    }

    public ScreenState FindScreen(Type screenType)
    {
        foreach (ScreenState state in screens)
        {
            if (state.GetType() == screenType)
            {
                return state;
            }
        }
        Debug.LogError("couldn't find state");
        return null;
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(screens[0].GetType());
        }
    }
}
