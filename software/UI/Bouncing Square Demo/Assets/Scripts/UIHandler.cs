using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RainbowArt.CleanFlatUI;
using TMPro;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    Selector mode_selector;

    [SerializeField]
    TMP_Dropdown joint_selector;

    [SerializeField]
    TMP_Dropdown waypoint_dropdown;

    [SerializeField]
    SwitchSimple self_lock_switch;

    [SerializeField]
    SwitchSimple other_lock_switch;

    [SerializeField]
    Button send_button;

    [SerializeField]
    Button clear_button;

    [SerializeField]
    Button save_button;

    [SerializeField]
    Button increase_button;

    [SerializeField]
    Button decrease_button;

    [SerializeField]
    Toast send_confirmation;

    [SerializeField]
    Toast save_confirmation;

    [SerializeField]
    Button reset_button;

    [SerializeField]
    ModelHandler model_handler;

    [SerializeField]
    IKController ik_controller;

    [SerializeField]
    GameObject lock_obj;

    [SerializeField]
    GameObject default_joint_obj;

    private enum Modes { Direct, Waypoint, Position }
    private Modes current_mode = Modes.Direct;
    int current_joint = 0;

    SerialHandler serial_handler;

    int n_joints = 4;

    void Start()
    {
        serial_handler = new SerialHandler();

        mode_selector.OnValueChanged.AddListener(ModeValueChanged);
        joint_selector.onValueChanged.AddListener(JointValueChanged);
        waypoint_dropdown.onValueChanged.AddListener(LoadWaypoint);

        send_button.onClick.AddListener(() => SendButtonClick());
        clear_button.onClick.AddListener(() => ClearButtonClick());
        save_button.onClick.AddListener(() => SaveButtonClick());

        increase_button.onClick.AddListener(() => UpdateJointButtonClick(1));
        decrease_button.onClick.AddListener(() => UpdateJointButtonClick(-1));

        reset_button.onClick.AddListener(() => Reset());

        self_lock_switch.OnValueChanged.AddListener(SelfLockChange);
        other_lock_switch.OnValueChanged.AddListener(OtherLockChange);

        send_confirmation.ShowTime = 2.0f;
        send_confirmation.HideToast();

        save_confirmation.HideToast();

        // InvokeRepeating("UpdateArm", 0.5f, 2.0f);
    }

    private void ModeValueChanged(int idx)
    {
        switch (idx)
        {
            case 0:
                current_mode = Modes.Direct;
                break;
            case 1:
                current_mode = Modes.Position;
                break;
            case 2:
                current_mode = Modes.Waypoint;
                break;
        }
    }

    private void UpdateJointButtonClick(int direction)
    {
        model_handler.UpdateJoint(current_joint, direction);
        if (current_mode == Modes.Direct)
        {
            SendButtonClick();
        }
    }

    private void JointValueChanged(int val)
    {
        int end_effector_id = 4;
        if (val == end_effector_id)
        {
            lock_obj.SetActive(true);
            default_joint_obj.SetActive(false);
        }
        else
        {
            lock_obj.SetActive(false);
            default_joint_obj.SetActive(true);
        }
        current_joint = val;
    }

    private void SaveButtonClick()
    {
        string saved_state_name = model_handler.SaveArmState();
        save_confirmation.ShowToast();
        waypoint_dropdown.options.Add(new TMP_Dropdown.OptionData() { text = saved_state_name });
    }


    private void ClearButtonClick()
    {
        model_handler.ResetArmState();
    }

    public void SendButtonClick()
    {
        if (current_mode == Modes.Waypoint || current_mode == Modes.Direct)
        {
            serial_handler.sendServoDelta(model_handler.GetCurrentArmState(), model_handler.GetFutureArmState());
            model_handler.updateArmState();
            if (current_mode == Modes.Waypoint)
            {
                send_confirmation.ShowToast();
            }
        }
        else if (current_mode == Modes.Position)
        {
            float[] current_servo_values = ik_controller.Angles;
            for(int i = 0; i < current_servo_values.Length; i++)
            {
                Debug.Log(current_servo_values[i]);
            }
            serial_handler.sendServoAngle(current_servo_values);
        }
    }

    void UpdateArm()
    {
        float[] current_servo_values = ik_controller.Angles;
        serial_handler.sendServoAngle(current_servo_values);
    }

    private void LoadWaypoint(int val)
    {
        model_handler.LoadWaypoint(val);
    }

    private void LockChange(string lock_key, bool val)
    {
        if (current_mode == Modes.Direct)
        {
            serial_handler.sendActionSerial(model_handler.GetFutureArmState(), lock_key);
            //SendButtonClick();
        }
        model_handler.LockChange(lock_key, val);
    }

    private void SelfLockChange(bool val)
    {
        LockChange("self", val);
    }

    private void OtherLockChange(bool val)
    {
        LockChange("other", val);
    }

    private void Reset()
    {
        serial_handler.sendResetSignal();
    }

    void Update()
    {
        
    }
}
