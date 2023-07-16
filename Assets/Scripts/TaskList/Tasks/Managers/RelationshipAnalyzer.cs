using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipAnalyzer : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    GameObject toggleParent;
    GameObject listParent;
    TaskListDataManager dataManager;

    private void Awake()
    {
        dataManager = GetComponent<TaskListDataManager>();
        toggleParent = Instantiate(prefab, transform);
        listParent = transform.Find("Scroll List").gameObject;
    }

    public void ShowRelationships()
    {
        listParent.SetActive(false);
        toggleParent.SetActive(true);

        var list = dataManager.GetTodaysList();

    }
}
