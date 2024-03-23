using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public TMPro.TMP_InputField result;

    private void Start()
    {
        result.text = Application.persistentDataPath;
    }
}
