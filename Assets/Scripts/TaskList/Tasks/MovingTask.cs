using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovingTask : MonoBehaviour
{
    public TextMeshProUGUI name_Text;
    private TaskUI hoveringTaskUI;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out hoveringTaskUI))
        {
            print(hoveringTaskUI.Data.name);
        }
    }
}
