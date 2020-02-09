using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableTimerUI : MonoBehaviour
{
    public Collectable collectable;

    RPGCharacterController player;
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
    }

    private void Update()
    {
        if (collectable != null && collectable.collected)
        {
            //Vector2 rectangle = new Vector2(8 + (player.active_collectables.IndexOf(collectable.gameObject) * 48), 48);
            //GetComponent<RectTransform>().position = rectangle;
            image.fillAmount = (collectable.collectible_time_limit - collectable.elapsed_time) / collectable.collectible_time_limit;
        }
        else
        {
            image.fillAmount = 0;
        }
    }
}
