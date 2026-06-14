using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PT_PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    public float speed = 3;
    public float gravity = -9.18f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // INPUT
        float x = 0f;
        float z = 0f;
        bool sprint = false;
        bool jumpDown = false;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;

        if (kb != null)
        {
            if (kb.wKey.isPressed) z = 1f;
            if (kb.sKey.isPressed) z = -1f;
            if (kb.aKey.isPressed) x = -1f;
            if (kb.dKey.isPressed) x = 1f;

            sprint = kb.leftShiftKey.isPressed;
            jumpDown = kb.spaceKey.wasPressedThisFrame;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        sprint = Input.GetKey(KeyCode.LeftShift);
        jumpDown = Input.GetButtonDown("Jump");
#endif

        speed = (sprint && isGrounded) ? 10f : 5f;

        // RUCH WZGLĘDEM KAMERY
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = right * x + forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // SKOK
        if (jumpDown && isGrounded)
        {
            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        // GRAWITACJA
        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }
}
