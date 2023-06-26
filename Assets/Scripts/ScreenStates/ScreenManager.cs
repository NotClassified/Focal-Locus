using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    List<ScreenState> screens = new List<ScreenState>();

    private ScreenState currentState;

    private void Awake()
    {
        instance = this;
        foreach (ScreenState screen in GetComponentsInChildren<ScreenState>(true))
        {
            screens.Add(screen);
        }

        foreach (ScreenState screen in screens)
        {
            screen.Activate(false);
        }
    }

    private void Start()
    {
        ChangeState<MenuManager>();

    }
    public void ChangeState<T>() where T : ScreenState => ChangeState(typeof(T));
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
