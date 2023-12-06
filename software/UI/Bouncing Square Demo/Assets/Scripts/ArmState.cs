using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockStatus { LOCK, UNLOCK };

public class ArmState
{
    public float[] angles;
    public int n_joints;
    public Dictionary<string, LockStatus> action_map = new Dictionary<string, LockStatus>
      {
          { "self", LockStatus.UNLOCK },
          { "other", LockStatus.UNLOCK }
      };

    public ArmState(int n_joints)
    {
        this.n_joints = n_joints;
        angles = new float[n_joints];
        for(int i = 0; i < n_joints; i++)
        {
            angles[i] = 0.0f;
        }
    }

    public void Update(ArmState other)
    {
        for (int i = 0; i < n_joints; i++)
        {
            angles[i] = other.angles[i];
        }
        action_map["self"] = other.action_map["self"];
        action_map["other"] = other.action_map["other"];
    }

    public ArmState Clone()
    {
        ArmState new_state = new ArmState(n_joints);
        new_state.Update(this);
        return new_state;
    }
};
