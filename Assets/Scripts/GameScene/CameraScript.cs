using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera cam;

    public float movementSpeed;
    public float zoomModifier;
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
        float basicSpeed = movementSpeed;
        transform.position = new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal") * (basicSpeed + zoomModifier * cam.orthographicSize / regOrthoSize), transform.position.y + Input.GetAxisRaw("Vertical") * (basicSpeed + zoomModifier * cam.orthographicSize / regOrthoSize));
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
