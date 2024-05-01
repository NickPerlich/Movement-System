using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public PlayerControls playerControls;
    public int jumpForce = 10;
   
    private Vector2 moveDirection = Vector2.zero;
    private InputAction move;
    private InputAction jump;
    private InputAction sprint;

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
        sprint.performed += Sprint;
        sprint.canceled += EndSprint; 
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        sprint.Disable();
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
        if (transform.position.y < 0.6)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        moveSpeed *= 1.5f;
    }

    private void EndSprint(InputAction.CallbackContext context)
    {
        moveSpeed /= 1.5f;
    }
}
