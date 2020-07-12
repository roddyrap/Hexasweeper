using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SounButtonScript : MonoBehaviour
{
    private Image image;
    public Sprite playSprite;
    public Sprite stopSprite;
    void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        if(MusicManager.instance != null)
        { 
            GetComponent<Button>().onClick.AddListener(MusicManager.instance.ReversePause);
            Debug.Log("G");
            if (MusicManager.instance.isPaused)
            {
                image.sprite = stopSprite;
            }
            else
            {
                image.sprite = playSprite;
            }
        }

    }
    
    void Update()
    {
        if (MusicManager.instance == null) return;
        if (MusicManager.instance.isPaused)
        {
            image.sprite = stopSprite;
        }
        else
        {
            image.sprite = playSprite;
        }
    }
}
