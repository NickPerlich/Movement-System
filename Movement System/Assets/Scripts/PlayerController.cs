using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private PlayerControls playerControls;
    [SerializeField] private int jumpForce = 5;
    [SerializeField] private GameObject standingModel;
    [SerializeField] private GameObject crouchingModel;
    private Vector2 moveDirection = Vector2.zero;
    
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
        crouch.performed += StartCrouch;
        crouch.canceled += EndCrouch;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();
        crouch.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.y * moveSpeed);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (transform.position.y == 0)
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

    private void StartCrouch(InputAction.CallbackContext context)
    {
        crouchingModel.SetActive(true);
        standingModel.SetActive(false);
    }

    private void EndCrouch(InputAction.CallbackContext context)
    {
        standingModel.SetActive(true);
        crouchingModel.SetActive(false);
    }
}
