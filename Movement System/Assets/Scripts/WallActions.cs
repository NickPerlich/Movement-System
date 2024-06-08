using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallActions : MonoBehaviour
{
    public LayerMask whatIsWall;
    public bool isWallRunning, isWallClimbing;

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

    private SlideManager sm;
    private bool hitWall, isWallRight, isWallLeft, isWallFront;
    private Vector3 wallNormalVector, wallRunDirection;

    // Start is called before the first frame update
    private void Start()
    {
        sm = gameObject.GetComponent<SlideManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!sm.isSliding)
        {
            WallRunInput();
        }
    }

    private void FixedUpdate()
    {
        CheckForWall();
    }

    public void WallRunInput()
    {
        if (hitWall && isWallLeft || hitWall && isWallRight)
        {
            WallRun();
        }
        else if (hitWall && isWallFront)
        {
            WallClimb();
        }
    }

    private void WallRun()
    {
        wallRunDirection = isWallRunning ? wallRunDirection : GetWallRunDirection();
        
        rb.useGravity = false;
        isWallRunning = true;

        if (rb.velocity.magnitude <= maxWallSpeed)
        {
            rb.AddForce(wallRunForce * Time.fixedDeltaTime * wallRunDirection);

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

    private void StopWallRun()
    {
        rb.useGravity = true;
        isWallRunning = false;
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

    private Vector3 GetWallRunDirection()
    {
        RaycastHit hit;
        Vector3 lookDirection = cameraOrientation.forward;

        if (Physics.Raycast(cameraOrientation.position, lookDirection, out hit, 2f))
        {
            Vector3 alongWall = hit.transform.right;
            return Vector3.Project(cameraOrientation.forward, alongWall).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        wallNormalVector = collision.GetContact(0).normal;
        hitWall = Mathf.Abs(Vector3.Dot(wallNormalVector, Vector3.up)) <= 0.1f;
    }
}
