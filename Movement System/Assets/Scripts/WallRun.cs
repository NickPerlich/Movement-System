using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public LayerMask whatIsWall;
    public bool isWallRunning, isWallClimbing;
    public Vector3 forward;

    [SerializeField]
    private Transform cameraOrientation;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform playerCam;
    [SerializeField]
    private float wallRunForce, maxWallRunTime, maxWallSpeed;
    [SerializeField]
    private float wallClimbForce, maxWallClimbTime, maxWallClimbSpeed;

    private PlayerController pc;
    private bool hitWall, isWallRight, isWallLeft, isWallFront;
    private Vector3 wallNormalVector;

    // Start is called before the first frame update
    private void Start()
    {
        pc = gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void WallRunInput()
    {
        bool isMovingRight = pc.moveDirection.x > 0;
        bool isMovingLeft = pc.moveDirection.x < 0;

        if (hitWall && isWallLeft || hitWall && isWallRight)
        {
            Wallrun();
        }
        else if (hitWall && isWallFront)
        {
            WallClimb();
        }
    }

    private void Wallrun()
    {
        isWallRunning = true;
        rb.useGravity = false;

        if (rb.velocity.magnitude <= maxWallSpeed)
        {
            forward = cameraOrientation.forward.z >= 0 ? Vector3.forward : -Vector3.forward;

            rb.AddForce(wallRunForce * Time.fixedDeltaTime * forward);

            // keep character sticking to wall
            if (isWallRight)
            {
                rb.AddForce(cameraOrientation.right * wallRunForce / 5 * Time.fixedDeltaTime);
            }
            else
            {
                rb.AddForce(-cameraOrientation.right * wallRunForce / 5 * Time.fixedDeltaTime);
            }
        }
    }

    private void StopWallRun()
    {
        rb.useGravity = true;
        isWallRunning = false;
    }

    private void WallClimb()
    {
        isWallClimbing = true;
        rb.useGravity = false;

        if (rb.velocity.magnitude <= maxWallClimbSpeed)
        {
            rb.AddForce(Vector3.up * wallClimbForce * Time.fixedDeltaTime);
            rb.AddForce(-wallNormalVector * wallClimbForce / 5 * Time.fixedDeltaTime);
        }
    }

    private void StopWallClimb()
    {
        rb.useGravity = true;
        isWallClimbing = false;
    }

    public void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, cameraOrientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -cameraOrientation.right, 1f, whatIsWall);
        isWallFront = Physics.Raycast(transform.position, cameraOrientation.forward, 1f, whatIsWall);

        if (!isWallLeft && !isWallRight)
        {
            StopWallRun();
        }

        if (!isWallFront)
        {
            StopWallClimb();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        wallNormalVector = collision.GetContact(0).normal;
        hitWall = Mathf.Abs(Vector3.Dot(wallNormalVector, Vector3.up)) <= 0.1f;
    }
}
