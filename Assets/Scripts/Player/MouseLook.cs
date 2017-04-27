using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that moves camera based upon mouse location
/// </summary>

// PUT THIS ON MAIN CAMERA
//
public class MouseLook : MonoBehaviour
{
    private float upDownLook = 0f;
    private float leftRightLook = 0f;

    private float xOffset;

    private void Start()
    {
        xOffset = transform.localEulerAngles.y;
    }

    void Update ()
    {
        if (!UIManager.Instance.isPaused)
        {
            // 1. Get mouse input data
            //
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 150f; // mouseDelta or horizontal mouse speed
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * 150f; // mouseDelta or vertical mouse speed

            // 2. Rotate the camera
            //
            transform.Rotate(0f, mouseX * 1.5f, 0f);

            // 2.b. Clamp / Constrain the X angle
            //
            upDownLook -= mouseY;
            upDownLook = Mathf.Clamp(upDownLook, -80f + xOffset, 80f + xOffset);
            leftRightLook += mouseX;

            // 3. Unroll the camera
            //
            transform.localEulerAngles = new Vector3(upDownLook, 0f, 0f);
            transform.parent.transform.eulerAngles = new Vector3(0f, leftRightLook, 0f);
        }
    }
}
