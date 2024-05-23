using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] 
    private Transform camOrientation;

    private void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camOrientation.position;
        transform.rotation = camOrientation.rotation;
    }
}
