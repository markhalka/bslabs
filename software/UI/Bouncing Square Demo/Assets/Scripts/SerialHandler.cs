using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

class SerialHandler : MonoBehaviour
{
    [SerializeField]
    UnitySerialPort serial_port;

    [SerializeField]
    public TMP_Text serial_data;

    void string prev_serial_data;

    void Update()
    {
        if (serial_data != prev_serial_data)
        {
            // parse it looking for a servo thing, if it exists then you should update the joint positions
            List<float> float_list = new List<float>();
            if (TryExtractFloats(serial_data, float_list))
            {
                // this is valid joint data
            }

            prev_serial_data = serial_data;
        }
    }

    void updateJointPositions(List<float> new_angles)
    {
        // now the difference is the definition of joint angles is different i think (or is it, now that you have the delta?)
        // nono this is coming from fabrik, so it is different
        // ok, so 
    }

    private bool TryExtractFloats(string input, out List<float> floatList)
    {
        floatList = null;

        if (input.StartsWith("servo:"))
        {
            string numbersPart = input.Substring(6);
            string[] numberStrings = numbersPart.Split(',');

            floatList = new List<float>();

            foreach (string numberString in numberStrings)
            {
                if (float.TryParse(numberString, out float number))
                {
                    floatList.Add(number);
                }
                else
                {
                    Console.WriteLine($"Error converting '{numberString}' to float.");
                    return false; // Stop processing if a conversion error occurs
                }
            }

            return true; // Successfully extracted and converted numbers
        }

        return false; // String does not have the 'servo:' prefix
    }


    private float[] getDeltas(ArmState current_arm_state, ArmState future_arm_state)
    {
        int n_joints = current_arm_state.n_joints;
        float[] output = new float[n_joints];
        for (int i = 0; i < n_joints; i++)
        {
            output[i] = future_arm_state.angles[i] - current_arm_state.angles[i];
        }
        return output;
    }

    private float[] getRadianValues(float[] angles)
    {
        int n_joints = angles.Length;
        float[] output = new float[n_joints];
        float gear_ratio = 1; // i think this is handled in servo_controller
        float deg_to_rad = (float)(Math.PI) / 180.0f;
        for (int i = 0; i < n_joints; i++)
        {
            output[i] = angles[i] * gear_ratio * deg_to_rad;
        }
        return output;
    }

    private string getServoSerialString(string header, float[] data)
    {
        string dataToSend = header + data[0].ToString("F2");
        for (int i = 1; i < data.Length; i++)
        {
            dataToSend += "," + data[i].ToString("F2");
        }
        return dataToSend;
    }

    private string getActionSerialString(ArmState future_arm_state, string key)
    {
        List<string> string_list = new List<string>();
        Dictionary<string, Dictionary<LockStatus, string>> command_dictionary = new Dictionary<string, Dictionary<LockStatus, string>>
            {
                {"self", new Dictionary<LockStatus, string>{{ LockStatus.LOCK, "unlock"}, {LockStatus.UNLOCK, "lock"} } },
                {"other",  new Dictionary<LockStatus, string>{{ LockStatus.LOCK, "pull"}, {LockStatus.UNLOCK, "push"}} }
            };

        return "a:" + command_dictionary[key][future_arm_state.action_map[key]];
    }

    public void sendServoDelta(ArmState current_arm_state, ArmState future_arm_state)
    {
        float[] serial_data = getRadianValues(getDeltas(current_arm_state, future_arm_state));
        string servo_data = getServoSerialString("s:", serial_data);
        Debug.Log(servo_data);
        serial_port.SendSerialDataAsLine(servo_data);
    }

    public void sendServoAngle(float[] servo_angles)
    {
        int n_joints = servo_angles.Length;
        float[] converted_servo_angles = getRadianValues(servo_angles);
        converted_servo_angles[2] *= -1;
        string servo_data = getServoSerialString("s:", converted_servo_angles);
        Debug.Log(servo_data);
        serial_port.SendSerialDataAsLine(servo_data);
    }

    public void sendPositionSerial(Vector3 position)
    {
        float[] serial_data = new float[] { position.x, position.y, 0.0f };
        string servo_data = getServoSerialString("p:", serial_data);
        Debug.Log(servo_data);
        serial_port.SendSerialDataAsLine(servo_data);
    }

    public void sendResetSignal()
    {
        string reset_signal = "r";
        Debug.Log(reset_signal);
        serial_port.SendSerialDataAsLine(reset_signal);
    }

    public void sendInitSignal()
    {
        string serial_data = "v";
        Debug.Log(serial_data);
        serial_port.SendSerialDataAsLine(serial_data);
    }

    /*
    public void sendInitSerial(Vector3 position)
    {
        if (serialPort.IsOpen)
        {
            float[] serial_data = convertUnityToRealPosition(position);
            string servo_data = getServoSerialString("i:", serial_data);
            Debug.Log(servo_data);
            serialPort.Write(servo_data);
        }
    }
    */

    public void sendActionSerial(ArmState future_arm_state, string key)
    {
        string action_data = getActionSerialString(future_arm_state, key);
        serialPort.Write(action_data);
        Debug.Log(action_data);

    }
};

