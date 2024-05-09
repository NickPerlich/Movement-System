using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int jumpForce = 5;
    [SerializeField] private GameObject standingModel;
    [SerializeField] private GameObject crouchingModel;
    [SerializeField] private GameObject cameraPosition;
    [SerializeField] private GameObject orientation;
    private float standingCamHeight;
    private const float CrouchCameraHeight = 0.375f;
    [SerializeField] private float rotationSpeed = 0.15f;

    private Vector2 moveDirection = Vector2.zero;
    private PlayerControls playerControls;

    private InputAction move;
    private InputAction jump;
    private InputAction sprint;
    private InputAction crouch;

    private void Awake()
    {
        playerControls = new PlayerControls();
        standingCamHeight = cameraPosition.transform.position.y;
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
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.y * moveSpeed);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.Euler(0, orientation.transform.eulerAngles.y, 0), rotationSpeed
            );
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
        cameraPosition.transform.position = new Vector3(
                cameraPosition.transform.position.x,
                CrouchCameraHeight,
                cameraPosition.transform.position.z
            );
    }

    private void EndCrouch(InputAction.CallbackContext context)
    {
        standingModel.SetActive(true);
        crouchingModel.SetActive(false);
        cameraPosition.transform.position = new Vector3(
                cameraPosition.transform.position.x,
                standingCamHeight,
                cameraPosition.transform.position.z
            );
    }
}
