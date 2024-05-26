using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideManager : MonoBehaviour
{
    public bool isSliding;

    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float slideForce, maxSlideSpeed, maxSlideTime, slideTimer;

    private Vector3 slideDirection;
    private PlayerController pc;
    private bool canSlide;

    // Start is called before the first frame update
    private void Start()
    {
        pc = gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        canSlide = pc.isGrounded && !isSliding && (rb.velocity.magnitude > 0);
        
        if (canSlide && pc.isCrouching)
        {
            StartSlide();
        }
        else if ((!pc.isCrouching || !canSlide) && isSliding)
        {
            EndSlide();
        }
    }

    private void FixedUpdate()
    {
        if (isSliding)
        {
            Slide();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = maxSlideTime;
        slideDirection = pc.TransformPlayerDirection(new Vector3(pc.moveDirection.x, 0, pc.moveDirection.y));
        Slide();
    }

    private void Slide()
    {
        if (rb.velocity.magnitude <= maxSlideSpeed)
        {
            rb.AddForce(slideDirection.normalized * slideForce, ForceMode.Force);
        }

        slideTimer -= Time.fixedDeltaTime;

        if (slideTimer <= 0 || (rb.velocity.magnitude <= 0))
        {
            EndSlide();
        }
    }
        
    private void EndSlide()
        {
            isSliding = false;
        }
    }
