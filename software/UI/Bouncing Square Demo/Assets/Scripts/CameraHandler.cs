using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    GameObject target_marker;

    [SerializeField]
    GameObject reference_marker;

    [SerializeField]
    GameObject turn_off_camera;

    [SerializeField]
    Transform show_cube;

    [SerializeField]
    Transform track_cube;

    [SerializeField]
    Transform base_cube;

    MovingAverage moving_average;
    private int window_size = 30;
    
    void Start()
    {
        Debug.Log("here, and setting it to false!");
        moving_average = new MovingAverage(window_size);
    }

    public void CalculateTargetMarkerPosition()
    {
        if (!reference_marker.activeSelf || !target_marker.activeSelf)
        {
            // Debug.Log("one of the markers not found");
            return;
        }
        Vector3 position_delta = target_marker.transform.position - reference_marker.transform.position;
        int scale = 1000; // m to mm
        position_delta.z *= -1;
        int y_offset = 150;

        Vector3 new_position = new Vector3(base_cube.position.x + position_delta.x * scale, base_cube.position.y + position_delta.y * scale,
            base_cube.position.z + position_delta.z * scale);
        
        moving_average.AddValue(new_position);
        Vector3 filtered_position = moving_average.CalculateAverage();
        show_cube.transform.position = filtered_position;
        track_cube.transform.position = new Vector3(filtered_position.x, filtered_position.y + y_offset, filtered_position.z);
    }

    void Update()
    {
        if(turn_off_camera.activeSelf)
        {
            turn_off_camera.SetActive(false);
        }

        CalculateTargetMarkerPosition();
    }
}
