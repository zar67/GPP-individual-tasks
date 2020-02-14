using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCutscene : MonoBehaviour
{
    enum CutsceneState
    {
        NONE,
        MOVE_TO_SWITCH,
        PRESS_SWITCH,
        MOVE_TO_DOOR,
        OPEN_DOOR
    }

    public Transform player_target;
    public Transform switch_camera_target;
    public Transform door_camera_target;

    RPGCharacterController player;
    GameObject player_camera;
    DoorSwitch door_switch;
    CutsceneState state = CutsceneState.NONE;

    float move_speed = 3;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
        player_camera = GameObject.FindGameObjectWithTag("MainCamera");
        door_switch = GetComponent<DoorSwitch>();
    }

    private void Update()
    {
        switch (state)
        {
            case CutsceneState.MOVE_TO_SWITCH:
            {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, switch_camera_target.position, move_speed);
                    player_camera.transform.LookAt(door_switch.gameObject.transform);

                    if (Vector3.Distance(player_camera.transform.position, switch_camera_target.position) < 0.25f)
                    {
                        state = CutsceneState.PRESS_SWITCH;
                    }
                    break;
            }
            case CutsceneState.PRESS_SWITCH:
            {
                    door_switch.Click();
                    state = CutsceneState.MOVE_TO_DOOR;
                    break;
            }
            case CutsceneState.MOVE_TO_DOOR:
            {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, door_camera_target.position, move_speed);
                    player_camera.transform.LookAt(door_switch.target.gameObject.transform);

                    if (Vector3.Distance(player_camera.transform.position, door_camera_target.position) < 0.25f)
                    {
                        state = CutsceneState.OPEN_DOOR;
                    }
                    break;
            }
            case CutsceneState.OPEN_DOOR:
            {
                    door_switch.target.Open();
                    player.accept_input = true;
                    player_camera.GetComponent<RPGCameraController>().enabled = true;

                    state = CutsceneState.NONE;
                    break;
            }
        }
    }

    public void StartCutscene()
    {
        player.accept_input = false;
        player.ResetAnimator();

        player_camera.GetComponent<RPGCameraController>().enabled = false;

        state = CutsceneState.MOVE_TO_SWITCH;
    }
}
