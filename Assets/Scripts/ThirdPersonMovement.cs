using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("PlayerControls")]
    public float jumpHeight = 3;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public Vector3 externalForce;
    public float gravity = -9.81f;
    PlayerControls playerControls;
    private CharacterController characterController;
    [SerializeField]
    private Vector3 velocity;

    [Header("Camera values")]
    public Transform cameraTransform;

    [Header("Player Ground check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundCheckMask;
    bool isGrounded;
    bool isJumping;

    [Header("Controller Input")]
    Vector2 rawLeftStickInputValue;
    Vector2 rawRightStickInputValue;
    private CinemachineFreeLook cinemachineFreeLook;



    private void Awake()
    {
        playerControls = new PlayerControls();
        cinemachineFreeLook = GetComponentInChildren<CinemachineFreeLook>();
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        playerControls.Gameplay.PlayerMovement.performed += ctx => rawLeftStickInputValue = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.PlayerMovement.canceled += ctx => rawLeftStickInputValue = Vector2.zero;

        playerControls.Gameplay.CameraMovement.performed += ctx => rawRightStickInputValue = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.CameraMovement.canceled += ctx => rawRightStickInputValue = Vector2.zero;

        playerControls.Gameplay.ActionButton.started += ctx => Jump();
        playerControls.Gameplay.ActionButton.canceled += ctx => EndJump();
    }

    private void OnEnable()
    {
        playerControls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        playerControls.Gameplay.Disable();
    }

    void Jump()
    {
        isJumping = true;
    }

    void EndJump()
    {
        isJumping = false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckDistance);
    }

    private bool IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundCheckMask);
        return isGrounded;
    }

    void FixedUpdate()
    {
        if(IsGrounded() && velocity.y <0)
        {
            velocity.y = -2f;
        }

        Vector3 direction = new Vector3(rawLeftStickInputValue.x, 0.0f, rawLeftStickInputValue.y).normalized;
        cinemachineFreeLook.m_XAxis.m_InputAxisValue = rawRightStickInputValue.x;
        cinemachineFreeLook.m_YAxis.m_InputAxisValue = rawRightStickInputValue.y;

        if (direction.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(moveDirection.normalized * speed * Time.deltaTime);

            if (isJumping && isGrounded)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = false;
            }
            velocity.y += gravity * Time.deltaTime;
            characterController.Move((velocity + externalForce) * Time.deltaTime);
        }
        else
        {
            if (isJumping && isGrounded)
            {
                if(externalForce.y >0)
                    velocity.y += externalForce.y;
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = false;
            }

            if (externalForce.y != 0 || externalForce.x != 0 || externalForce.z != 0)
            {
                externalForce.y = -200;
                //if (isGrounded && externalForce.y > 0 )
                //{
                //    externalForce.y = 0;
                //}
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move((velocity + externalForce) * Time.deltaTime);
        }

    }
}
