using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public LayerMask whatIsWall;
    public float wallRunForce, maxWallRunTime, maxWallSpeed;
    public bool isWallRunning;
    public Vector3 forward;

    [SerializeField]
    private Transform cameraOrientation;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform playerCam;

    private PlayerController pc;
    private bool hitWall, isWallRight, isWallLeft;

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
    }

    public void Wallrun()
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

    public void StopWallRun()
    {
        rb.useGravity = true;
        isWallRunning = false;
    }

    public void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, cameraOrientation.right, 1f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -cameraOrientation.right, 1f, whatIsWall);

        if (!isWallLeft && !isWallRight)
        {
            StopWallRun();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitWall = Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.up)) <= 0.1f;
    }
}
