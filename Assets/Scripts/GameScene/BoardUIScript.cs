using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardUIScript : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private TilemapManager tm;
    private GameObject objectToDrag;
    private bool clicked;
    // Start is called before the first frame update
    void Start()
    {
        objectToDrag = GameObject.Find("Main Camera");
        tm = GameObject.Find("Tilemap").GetComponent<TilemapManager>();
    }
    
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(objectToDrag.transform.position);
 
        offset = objectToDrag.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
 
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
 
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        objectToDrag.transform.position = curPosition;
 
    }
}
