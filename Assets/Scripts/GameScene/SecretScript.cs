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
        allMiddleClick = PlayerPrefs.GetInt("AllMiddleClick") == 1;
        timesClicked = 0;
    }

    public void OnGameIconClick()
    {
        timesClicked++;
        if (timesClicked == 5)
        {
            timesClicked = 0;
            if (!allMiddleClick)
            {
                allMiddleClick = true;
                Debug.Log("Secret activated! all mouse buttons now function like a middle click!");
                FlashingTextScript.statusText.DisplayText("Secret activated! all mouse buttons now also function as middle click!", 0.2f, 5);
            }
            else
            {
                allMiddleClick = false;
                FlashingTextScript.statusText.DisplayText("Secret deactivated! Only middle click will reveal adjacent tiles!", 0.2f, 5);
            }
            PlayerPrefs.SetInt("AllMiddleClick", allMiddleClick? 1 : 0);
        }
    }
}
