using System.Collections;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public enum CutsceneState
    {
        NONE,
        MOVE_TO_SWITCH,
        PRESS_SWITCH,
        MOVE_TO_ACTION,
        TRIGGER_ACTION,
        MOVE_BACK
    }

    public Transform player_target;
    public Transform switch_camera_target;
    public Transform action_camera_target;
    public bool camera_return_to_start_position;

    protected Vector3 return_position;
    protected Quaternion return_rotation;

    protected RPGCharacterController player;
    protected GameObject player_camera;
    protected Switch target_switch;
    [HideInInspector]
    public CutsceneState state = CutsceneState.NONE;

    protected float move_speed = 2;
    protected float rotation_speed = 3.5f;

    protected bool pressing_swtich = false;
    protected bool triggered_action = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
        player_camera = GameObject.FindGameObjectWithTag("MainCamera");
        target_switch = GetComponent<Switch>();
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
                    if (!pressing_swtich && !player.GetComponent<Animator>().GetBool("armed"))
                    {
                        StartCoroutine(PressSwitch());
                    }
                    break;
                }
            case CutsceneState.MOVE_TO_ACTION:
                {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, action_camera_target.position, move_speed * 0.75f * Time.deltaTime);
                    player_camera.transform.rotation = Quaternion.Lerp(player_camera.transform.rotation, action_camera_target.rotation, rotation_speed * Time.deltaTime);

                    if (Vector3.Distance(player_camera.transform.position, action_camera_target.position) < 0.25f)
                    {
                        state = CutsceneState.TRIGGER_ACTION;
                    }
                    break;
                }
            case CutsceneState.TRIGGER_ACTION:
                {
                    if (!triggered_action)
                    {
                        StartCoroutine(TriggerAction());
                    }
                    break;
                }
            case CutsceneState.MOVE_BACK:
                {
                    player_camera.transform.position = Vector3.Lerp(player_camera.transform.position, return_position, move_speed * 1.5f * Time.deltaTime);
                    player_camera.transform.rotation = Quaternion.Lerp(player_camera.transform.rotation, return_rotation, rotation_speed * Time.deltaTime);
                    //player_camera.transform.LookAt(player.transform.position + new Vector3(0, 2.4f, 0));

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
            return_rotation = Quaternion.LookRotation(player.transform.position - player_camera.transform.position + new Vector3(0, 2.4f, 0), Vector3.up);
        }
        else
        {
            return_position = player.transform.position - (player.transform.forward * 5.5f) + (player.transform.up * 5.3f);
            return_rotation = Quaternion.LookRotation(player.transform.position - (player.transform.position - (player.transform.forward * 5.5f) + (player.transform.up * 5.3f) - new Vector3(0, 2.4f, 0)), Vector3.up);
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

    public virtual void Trigger()
    {
        Debug.Log("Triggered");
    }

    IEnumerator TriggerAction()
    {
        triggered_action = true;
        Trigger();

        yield return new WaitForSeconds(1);

        state = CutsceneState.MOVE_BACK;
        triggered_action = false;
    }

    IEnumerator PressSwitch()
    {
        pressing_swtich = true;
        player.GetComponent<Animator>().Play("Attack-L3", 2);

        yield return new WaitForSeconds(0.15f);

        target_switch.Click();

        yield return new WaitForSeconds(0.5f);

        state = CutsceneState.MOVE_TO_ACTION;
        pressing_swtich = false;
    }
}
