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

    float move_speed = 2;

    bool pressing_swtich = false;
    bool opening_door = false;

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
                    player.transform.position = player_target.position;
                    player.transform.rotation = player_target.rotation;

                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, switch_camera_target.position, move_speed * Time.deltaTime);
                    player_camera.transform.rotation = Quaternion.Lerp(player_camera.transform.rotation, switch_camera_target.rotation, move_speed * Time.deltaTime);

                    if (Vector3.Distance(player_camera.transform.position, switch_camera_target.position) < 0.25f)
                    {
                        state = CutsceneState.PRESS_SWITCH;
                    }
                    break;
            }
            case CutsceneState.PRESS_SWITCH:
            {
                    if (!pressing_swtich)
                    {
                        StartCoroutine(PressSwitch());
                    }
                    break;
            }
            case CutsceneState.MOVE_TO_DOOR:
            {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, door_camera_target.position, move_speed * 0.75f * Time.deltaTime);
                    player_camera.transform.rotation = Quaternion.Lerp(player_camera.transform.rotation, door_camera_target.rotation, move_speed * 0.75f * Time.deltaTime);
                    //player_camera.transform.LookAt(door_switch.target.gameObject.transform);

                    if (Vector3.Distance(player_camera.transform.position, door_camera_target.position) < 0.25f)
                    {
                        state = CutsceneState.OPEN_DOOR;
                    }
                    break;
            }
            case CutsceneState.OPEN_DOOR:
            {
                    if (!opening_door)
                    {
                        StartCoroutine(OpenDoor());
                    }
                    break;
            }
                // Lerp to camera starting position
        }
    }

    public void StartCutscene()
    {
        player.accept_input = false;
        player.ResetAnimator();
        
        if (player.GetComponent<Animator>().GetBool("armed"))
        {
            StartCoroutine(player.Sheath());
        }

        player_camera.GetComponent<RPGCameraController>().enabled = false;

        state = CutsceneState.MOVE_TO_SWITCH;
    }

    IEnumerator OpenDoor()
    {
        opening_door = true;
        door_switch.target.Open();

        yield return new WaitForSeconds(1);

        player.accept_input = true;
        player_camera.GetComponent<RPGCameraController>().enabled = true;

        state = CutsceneState.NONE;
        opening_door = false;
    }

    IEnumerator PressSwitch()
    {
        pressing_swtich = true;
        player.GetComponent<Animator>().Play("Attack-L3", 2);

        yield return new WaitForSeconds(0.15f);

        door_switch.Click();

        yield return new WaitForSeconds(0.5f);

        state = CutsceneState.MOVE_TO_DOOR;
        pressing_swtich = false;
    }
}
