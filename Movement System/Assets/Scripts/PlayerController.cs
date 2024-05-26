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
    
    // script references
    private WallRun wr;
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
        wr = gameObject.GetComponent<WallRun>();
        sm = gameObject.GetComponent<SlideManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!sm.isSliding)
        {
            moveDirection = GetInput();
            wr.WallRunInput();
        }
    }

    private void FixedUpdate()
    {
        if (!wr.isWallRunning && !sm.isSliding)
        {
            Vector3 newVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.y * moveSpeed);
            rb.velocity = TransformPlayerDirection(newVelocity);
        }
        wr.CheckForWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isGround = Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.forward)) <= 0.1f;

        if (isGround && !isGrounded)
        {
            isGrounded = true;
        }
    }

    // input: a vector in the player's local space
    // output: a vector in global space with the meaning of the input vector
    public Vector3 TransformPlayerDirection(Vector3 vector)
    {
        return cameraOrientation.transform.TransformDirection(vector);
    }

    private Vector2 GetInput()
    {
        return move.ReadValue<Vector2>();
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
        cameraOrientation.transform.localPosition = crouchCamPos;
    }

    private void Uncrouch(InputAction.CallbackContext context)
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        Vector3 standingCamPos = new Vector3(0, StandingCameraHeight, 0);
        cameraOrientation.transform.localPosition = standingCamPos;
    }

}
