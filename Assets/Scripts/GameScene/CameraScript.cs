using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera cam;

    public float movementSpeed;
    public float zoomSpeed;
    public float mouseWheelMultiplier;
    private float regOrthoSize;

    private Vector3 regPos;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        regOrthoSize = cam.orthographicSize;
        regPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Equals))
            if (cam.orthographicSize <= zoomSpeed)
                cam.orthographicSize = zoomSpeed;
            else cam.orthographicSize -= zoomSpeed;
        if(Input.GetKey(KeyCode.Minus))
            cam.orthographicSize += zoomSpeed;
        // if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        // {
        //     transform.position = new Vector3(transform.position.x, transform.position.y + movementSpeed);
        // }
        // if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        // {
        //     transform.position = new Vector3(transform.position.x, transform.position.y - movementSpeed);
        //     
        // }
        // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        // {
        //     transform.position = new Vector3(transform.position.x - movementSpeed, transform.position.y);
        // }
        // if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        // {
        //     transform.position = new Vector3(transform.position.x  + movementSpeed, transform.position.y);
        // }
        transform.position = new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal") * zoomSpeed, transform.position.y + Input.GetAxisRaw("Vertical") * zoomSpeed);
    }

    private void Update()
    {
        cam.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed * mouseWheelMultiplier;
        if (cam.orthographicSize < zoomSpeed)
        {
            cam.orthographicSize = zoomSpeed;
        }
    }

    public void CenterCam()
    {
        cam.orthographicSize = regOrthoSize;
        cam.transform.position = regPos;
    }
}
