using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovingTask : MonoBehaviour
{
    public TextMeshProUGUI name_Text;
    public TaskUI hoveringTask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out hoveringTask))
        {
            hoveringTask.DarkenColor(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out TaskUI exitTask))
        {
            exitTask.DarkenColor(false);
        }
    }
}
