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
        GetComponent<Button>().onClick.AddListener(MusicManager.instance.ReversePause);
        image = transform.GetChild(0).GetComponent<Image>();
        if (MusicManager.instance.isPaused)
        {
            image.sprite = stopSprite;
        }
        else
        {
            image.sprite = playSprite;
        }
        Debug.Log(image.rectTransform.rect.width);
    }
    
    void Update()
    {
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
