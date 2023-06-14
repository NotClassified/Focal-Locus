using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskIDManager
{
    private static int newestID;
    public static int getNewID
    {
        get => ++newestID;
    }
    public static void SetNewestID(int newestID) => TaskIDManager.newestID = newestID;
}
