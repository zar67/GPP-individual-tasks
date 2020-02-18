using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCutscene : MonoBehaviour
{
    public enum CutsceneState
    {
        NONE,
        MOVE_TO_SWITCH,
        PRESS_SWITCH,
        MOVE_TO_DOOR,
        OPEN_DOOR,
        MOVE_BACK
    }

    public Transform player_target;
    public Transform switch_camera_target;
    public Transform door_camera_target;
    public bool camera_return_to_start_position;

    Vector3 return_position;

    RPGCharacterController player;
    GameObject player_camera;
    DoorSwitch door_switch;
    [HideInInspector]
    public CutsceneState state = CutsceneState.NONE;

    float move_speed = 2.5f;
    float rotation_speed = 4;

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
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, switch_camera_target.position, move_speed * Time.deltaTime);
                    player_camera.transform.rotation = Quaternion.Lerp(player_camera.transform.rotation, switch_camera_target.rotation, rotation_speed * Time.deltaTime);

                    if (Vector3.Distance(player_camera.transform.position, switch_camera_target.position) < 0.25f)
                    {
                        state = CutsceneState.PRESS_SWITCH;
                    }
                    break;
            }
            case CutsceneState.PRESS_SWITCH:
            {
                    if (!pressing_swtich && player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(2).IsName("Default"))
                    {
                        StartCoroutine(PressSwitch());
                    }
                    break;
            }
            case CutsceneState.MOVE_TO_DOOR:
            {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, door_camera_target.position, move_speed * 0.75f * Time.deltaTime);
                    player_camera.transform.rotation = Quaternion.Lerp(player_camera.transform.rotation, door_camera_target.rotation, rotation_speed * Time.deltaTime);

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
            case CutsceneState.MOVE_BACK:
            {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, return_position, move_speed * 1.5f * Time.deltaTime);
                    player_camera.transform.LookAt(player.transform.position + new Vector3(0, 2.4f, 0));

                    if (Vector3.Distance(player_camera.transform.position, return_position) < 0.1f)
                    {
                        player.accept_input = true;

                        if (camera_return_to_start_position)
                        {
                            player_camera.GetComponent<RPGCameraController>().EnableCamera();
                        }   
                        else
                        {
                            player_camera.GetComponent<RPGCameraController>().ResetCamera();
                        }
                        
                        state = CutsceneState.NONE;
                    }
                    break;
            }
        }
    }

    public void StartCutscene()
    {
        player.accept_input = false;
        player.ResetAnimator();

        player.transform.position = player_target.position;
        player.transform.rotation = player_target.rotation;

        if (camera_return_to_start_position)
        {
            return_position = player_camera.transform.position;
        }
        else
        {
            return_position = player.transform.position - (player.transform.forward * 5.5f) + (player.transform.up * 5.3f);
        }
        

        player_camera.GetComponent<RPGCameraController>().DisableCamera();

        if (player.GetComponent<Animator>().GetBool("armed"))
        {
            StartCoroutine(player.Sheath());
        }

        player_camera.transform.position = switch_camera_target.position;
        player_camera.transform.rotation = switch_camera_target.rotation;
        state = CutsceneState.PRESS_SWITCH;
    }

    IEnumerator OpenDoor()
    {
        opening_door = true;
        door_switch.target.Open();

        yield return new WaitForSeconds(1);

        state = CutsceneState.MOVE_BACK;
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
