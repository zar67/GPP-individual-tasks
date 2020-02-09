using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollectables : MonoBehaviour
{
    // UI
    public Image double_jump_ui;
    public Image speed_boost_ui;
    
    // Player Reference
    RPGCharacterController player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
    }

    private void Update()
    {
        Vector2 next_position = new Vector2(8, 28);
        if (player.double_jump != null)
        {
            double_jump_ui.GetComponent<RectTransform>().position = next_position;
            next_position.x = 52;
            double_jump_ui.fillAmount = (player.double_jump.collectible_time_limit - player.double_jump.elapsed_time) / player.double_jump.collectible_time_limit;
        }
        else
        {
            double_jump_ui.fillAmount = 0;
        }

        if (player.speed_boost != null)
        {
            speed_boost_ui.GetComponent<RectTransform>().position = next_position;
            speed_boost_ui.fillAmount = (player.speed_boost.collectible_time_limit - player.speed_boost.elapsed_time) / player.speed_boost.collectible_time_limit;
        }
        else
        {
            speed_boost_ui.fillAmount = 0;
        }
    }
}
