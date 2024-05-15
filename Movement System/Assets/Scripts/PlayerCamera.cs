using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] 
    private float sensX;
    [SerializeField] 
    private float sensY;
    [SerializeField]
    private Transform cameraOrientation;
    [SerializeField]
    float timerLength;
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    Image timerTextPanel;

    private float xRotation;
    private float yRotation;
    float timer;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        timer = timerLength;
        timerTextPanel.enabled = true;
        timerText.text = "3";
    }

    // Update is called once per frame
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0.5f)
        {
            timerText.text = "" + (Mathf.Round(10.0f * (timer - 0.5f))/ 10.0f);
        }
        else if (timer > 0.0f)
        {
            timerText.text = "GO!";
        }
        else
        {
            timerText.text = "";
            timerTextPanel.enabled = false;
        }
        if (timer <= 0)
        {
            // get mouse input 
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 60f);

            //rotate camera and orientation
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            cameraOrientation.localRotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
