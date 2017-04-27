using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that allows player to move with Horizontal/Vertical Axis Input
/// </summary>

[RequireComponent(typeof (CharacterController))]
public class ControllerMove : MonoBehaviour
{
    private CharacterController _characterController;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update ()
    {
        if (!UIManager.Instance.isPaused)
        {
            // 1. Grab input from input devices
            float horizontal = Input.GetAxis("Horizontal"); // Left and Right movement
            float vertical = Input.GetAxis("Vertical"); // Up and Down movement

            // 2. Plug your values into CharacterController
            _characterController.Move(transform.forward * Time.deltaTime * vertical * 10f); // Move along forward facing
            _characterController.Move(transform.right * Time.deltaTime * horizontal * 10f); // Move along forward facing
                                                                                            //transform.Rotate( 0f, horizontal * Time.deltaTime * 90f, 0f);

            // 3. Let's add gravity
            _characterController.Move(Physics.gravity);

            // 4. Press space to jump!
            //if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && _characterController.isGrounded)
            //{
            //    _characterController.Move(new Vector3(0f, 10f, 0f));
            //}

            // 5. Code acceleration and velocity. Make gravity / jump force more gradual
            //
            // Not Implemented
        }
    }
}
