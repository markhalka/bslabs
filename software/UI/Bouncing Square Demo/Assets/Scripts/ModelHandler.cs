using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHandler : MonoBehaviour
{
    [SerializeField]
    Transform[] joints;

    [SerializeField]
    Transform self_lock;

    [SerializeField]
    Transform other_lock;

    Vector3 self_lock_pos;
    Vector3 other_lock_pos;

    ArmState current_arm_state;
    ArmState future_arm_state;

    float delta = 10f;

    int save_id = 0;
    List<ArmState> saved_states = new List<ArmState>();

    public int n_joints = 4;

    void Start()
    {
        current_arm_state = new ArmState(n_joints);
        future_arm_state = new ArmState(n_joints);

        self_lock_pos = self_lock.transform.localPosition;
        other_lock_pos = other_lock.transform.localPosition;
    }

    public void LockChange(string lock_key, bool val)
    {
        if (val)
        {
            future_arm_state.action_map[lock_key] = LockStatus.LOCK;
        }
        else
        {
            future_arm_state.action_map[lock_key] = LockStatus.UNLOCK;
        }
        Update3dModel();
    }

    private void rotateBetween(float[] old_angles, float[] new_angles)
    {
        for (int i = 0; i < n_joints; i++)
        {
            joints[i].Rotate(0, 0, new_angles[i] - old_angles[i], Space.Self);
        }
    }

    private int delta_y = -10;
    private void UpdateLock(Transform transform, Vector3 default_pos, bool is_locked)
    {
        float new_x = is_locked ? default_pos.x : default_pos.x + delta_y;
        transform.localPosition = new Vector3(new_x, default_pos.y, default_pos.z);
    }

    public string SaveArmState()
    {
        string saved_state_name = "Saved State: " + save_id;
        save_id += 1;
        saved_states.Add(future_arm_state.Clone());
        for (int i = 0; i < n_joints; i++)
        {
            Debug.Log(future_arm_state.angles[i]);
        }
        return saved_state_name;
    }

    public void LoadWaypoint(int val)
    {
        rotateBetween(future_arm_state.angles, saved_states[val].angles);
        current_arm_state.Update(saved_states[val]);
        future_arm_state.Update(saved_states[val]);
    }

    public void ResetArmState()
    {
        rotateBetween(future_arm_state.angles, current_arm_state.angles);
        future_arm_state = current_arm_state.Clone();
        Update3dModel();
    }

    public void updateArmState()
    {
        current_arm_state.Update(future_arm_state);
    }

    public void UpdateJoint(int current_joint, int direction)
    {
        future_arm_state.angles[current_joint] += delta * direction;
    }

    private void Update3dModel()
    {
        UpdateLock(self_lock, self_lock_pos, future_arm_state.action_map["self"] == LockStatus.LOCK);
        UpdateLock(other_lock, other_lock_pos, future_arm_state.action_map["other"] == LockStatus.LOCK);
    }
    
    public ArmState GetCurrentArmState()
    {
        return current_arm_state;
    }

    public ArmState GetFutureArmState()
    {
        return future_arm_state;
    }

    void Update()
    {
        
    }
}
