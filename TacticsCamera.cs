using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate the camera.
/// </summary>
public class TacticsCamera : MonoBehaviour
{
    public float rotation = 90;

    /// <summary>
    /// Rotate the camera into the given direction.
    /// </summary>
    private void Rotate(float direction)
    {
        transform.Rotate(Vector3.up, direction, Space.Self);
    }

    /// <summary>
    /// Rotate the camera to the left
    /// </summary>
    public void RotateLeft()
    {
        Debug.Log("RotateLeft");
        Rotate (rotation);
    }

    /// <summary>
    /// Rotate the camera to the right
    /// </summary>
    public void RotateRight()
    {
        Debug.Log("RotateRight");
        Rotate(-rotation);
    }
}
