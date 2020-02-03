using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableTimerUI : MonoBehaviour
{
    public Collectable collectable;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (collectable != null && collectable.collected)
        {
            image.fillAmount = (collectable.collectible_time_limit - collectable.elapsed_time) / collectable.collectible_time_limit;
        }
        else
        {
            image.fillAmount = 0;
        }
    }
}
