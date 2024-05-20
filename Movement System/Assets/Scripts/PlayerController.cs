using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // settables
    [SerializeField] 
    private Rigidbody rb;
    [SerializeField] 
    private float moveSpeed = 5f;
    [SerializeField] 
    private int jumpForce = 5;
    [SerializeField] 
    private GameObject cameraOrientation;

    // private floats
    private const float StandingCameraHeight = 0.75f;
    private const float CrouchCameraHeight = 0.375f;

    // private vector2s
    private Vector2 moveDirection = Vector2.zero;
    
    // private bools
    private bool isGrounded;

    // player input actions
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
        isGrounded = true;
    }

    // Update is called once per frame
    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 newVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.y * moveSpeed);
        rb.velocity = TransformPlayerDirection(newVelocity);
    }

    // input: a vector in the player's local space
    // output: a vector in global space with the meaning of the input vector
    private Vector3 TransformPlayerDirection(Vector3 vector)
    {
        return cameraOrientation.transform.TransformDirection(vector);
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isWall = Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.up)) <= 0.1f;
        bool isGround = Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.forward)) <= 0.1f;

        if (isGround && !isGrounded)
        {
            isGrounded = true;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
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
