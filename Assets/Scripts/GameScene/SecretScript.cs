using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretScript : MonoBehaviour
{
    /**
     * Current Secrets:
     * 1. clicking on the icon in the game screen will make every button function like a middle click when pressing on a revealed tile.
     */
    private byte timesClicked;

    public static bool allMiddleClick;
    
    // Start is called before the first frame update
    void Start()
    {
        timesClicked = 0;
    }

    public void OnGameIconClick()
    {
        timesClicked++;
        if (timesClicked > 4)
        {
            allMiddleClick = true;
            Debug.Log("Secret activated! all mouse buttons now function like a middle click!");
        }
    }
}
