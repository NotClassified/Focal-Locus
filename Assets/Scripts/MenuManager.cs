using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Transform listParent;

    private void Start()
    {
        foreach (Transform child in listParent)
        {
            child.gameObject.GetComponent<Button>().onClick.AddListener(delegate { LoadScene(child.name); } );
        }
    }

    public void LoadScene(string scene) => SceneManager.LoadScene(scene);
}
