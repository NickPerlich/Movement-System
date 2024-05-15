using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody rb;
    [SerializeField] 
    private float moveSpeed = 5f;
    [SerializeField] 
    private int jumpForce = 5;
    [SerializeField] 
    private GameObject cameraOrientation;

    private const float StandingCameraHeight = 0.75f;
    private const float CrouchCameraHeight = 0.375f;

    private Vector2 moveDirection = Vector2.zero;
    private PlayerControls playerControls;

    private InputAction move;
    private InputAction jump;
    private InputAction sprint;
    private InputAction crouch;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        sprint = playerControls.Player.Sprint;
        sprint.Enable();
        sprint.performed += StartSprint;
        sprint.canceled += EndSprint;

        crouch = playerControls.Player.Crouch;
        crouch.Enable();
        crouch.performed += Crouch;
        crouch.canceled += Uncrouch;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();
        crouch.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 newLocalVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.y * moveSpeed);

        // using cameraPosition's transform as local space, transforms the direction of newLocalVelocity into global space
        Vector3 newGlobalVelocity = cameraOrientation.transform.TransformDirection(newLocalVelocity);

        rb.velocity = newGlobalVelocity;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (transform.position.y <= 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void StartSprint(InputAction.CallbackContext context)
    {
        moveSpeed *= 1.5f;
    }

    private void EndSprint(InputAction.CallbackContext context)
    {
        moveSpeed /= 1.5f;
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        Vector3 crouchCamPos = new Vector3(0, CrouchCameraHeight, 0);
        cameraOrientation.transform.localPosition = crouchCamPos;
    }

    private void Uncrouch(InputAction.CallbackContext context)
    {
        Vector3 standingCamPos = new Vector3(0, StandingCameraHeight, 0);
        cameraOrientation.transform.localPosition = standingCamPos;
    }
}
