using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make sure that the object carrying the script follows the camera
/// </summary>
public class FaceTheCamera : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        FacesTheCamera();
    }

    private void FacesTheCamera()
    {
        // get the camera position
        Vector3 target = Camera.main.transform.position;

        // set the y of the target to the y of the camera position
        target.y = transform.position.y;

        // make the object carrying the script look at this point.
        transform.LookAt (target);
    }
}
