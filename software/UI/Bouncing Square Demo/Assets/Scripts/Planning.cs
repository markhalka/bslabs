using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Planning : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    [SerializeField]
    Transform controller;

    [SerializeField]
    IKController ik_controller;

    Stack<Vector3> plan = new Stack<Vector3>();
    bool done_step = true;
    void Start()
    {
        Debug.Log("Starting plan!");
        createPlan(target.transform.position);
    }


    public void createPlan(Vector3 cube_pos)
    {
        float y_offset = 100;
        float x_offset = 25; // half a cube length;
        float z_offset = 0;

        plan.Push(new Vector3(cube_pos.x + x_offset, cube_pos.y + y_offset, cube_pos.z + z_offset));
        plan.Push(new Vector3(cube_pos.x + x_offset, cube_pos.y + 25, cube_pos.z + z_offset));
    }

    public void checkStep()
    {
        if (ik_controller.isDone())
        {
            done_step = true;
        }
    }

    void Update()
    {
        if (plan.Count == 0)
        {
            return;
        }
        if (done_step)
        {
            Debug.Log("done step!");
            controller.position = plan.Pop();
            done_step = false;
        }
        checkStep();
    }
}
