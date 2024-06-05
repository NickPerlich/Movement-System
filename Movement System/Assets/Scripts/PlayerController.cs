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
    private Transform cameraOrientation;
    
    // script references
    private WallActions wa;
    private SlideManager sm;

    // private floats
    private const float StandingCameraHeight = 0.75f;
    private const float CrouchCameraHeight = 0.375f;

    // private vector2s
    public Vector2 moveDirection = Vector2.zero;
    
    // public bools
    public bool isGrounded;
    public bool isCrouching;
    public bool isSprinting;

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
        move.performed += Move;
        move.canceled += Stop;

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
        wa = gameObject.GetComponent<WallActions>();
        sm = gameObject.GetComponent<SlideManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!sm.isSliding)
        {
            wa.WallRunInput();
        }
    }

    private void FixedUpdate()
    {
        bool canWalk = !wa.isWallClimbing && !wa.isWallRunning && !sm.isSliding;

        if (canWalk)
        {
            Vector3 newVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.y * moveSpeed);
            rb.velocity = TransformPlayerDirection(newVelocity);
        }
        wa.CheckForWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isGround = Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.forward)) <= 0.1f;

        if (isGround && !isGrounded)
        {
            isGrounded = true;
        }
    }

    // This method transforms the given direction vector
    // from the local space of the camera's transform to world space
    // to allow for look-based movement
    public Vector3 TransformPlayerDirection(Vector3 direction)
    {
        return cameraOrientation.TransformDirection(direction);
    }

    private void Move(InputAction.CallbackContext context)
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void Stop(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
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
        if (!isCrouching && isGrounded)
        {
            moveSpeed *= 1.5f;
            isSprinting = true;
        }
    }

    private void EndSprint(InputAction.CallbackContext context)
    {
        moveSpeed /= 1.5f;
        isSprinting = false;
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        Vector3 crouchCamPos = new Vector3(0, CrouchCameraHeight, 0);
        cameraOrientation.localPosition = crouchCamPos;
    }

    private void Uncrouch(InputAction.CallbackContext context)
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        Vector3 standingCamPos = new Vector3(0, StandingCameraHeight, 0);
        cameraOrientation.localPosition = standingCamPos;
    }

}
