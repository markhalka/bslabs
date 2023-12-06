using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovingAverage
{
    private List<Vector3> values;
    private int windowSize;

    public MovingAverage(int windowSize)
    {
        if (windowSize <= 0)
        {
            throw new ArgumentException("Window size must be greater than zero.");
        }

        this.windowSize = windowSize;
        values = new List<Vector3>();
    }

    public void AddValue(Vector3 value)
    {
        values.Add(value);

        // Trim the list to the specified window size
        if (values.Count > windowSize)
        {
            values.RemoveAt(0);
        }
    }

    public Vector3 CalculateAverage()
    {
        if (values.Count == 0)
        {
            throw new InvalidOperationException("No values to calculate average.");
        }

        // Calculate the average Vector3
        Vector3 sum = values.Aggregate((acc, v) => acc + v);
        return sum / values.Count;
    }
}
