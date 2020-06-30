using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashingTextScript : MonoBehaviour
{
    public static FlashingTextScript statusText;
    private TextMeshProUGUI text;

    private float time;

    private float timeStandby;
    // Start is called before the first frame update
    void Start()
    {
        statusText = this;
        text = GetComponent<TextMeshProUGUI>();
        LeanTween.scale(gameObject, Vector3.zero, 0);

    }

    public void DisplayText(String message, float time)
    {
        this.timeStandby = 0;
        text.text = message;
        this.time = time;
        LeanTween.scale(gameObject, Vector3.one, time / 2).setOnComplete(OnComplete);
    }
    
    public void DisplayText(String message, float timeMovement, float timeStandby)
    {
        text.text = message;
        this.time = timeMovement;
        this.timeStandby = timeStandby;
        LeanTween.scale(gameObject, Vector3.one, time / 2).setOnComplete(OnComplete);
    }

    private void OnComplete()
    {
        StartCoroutine(FinishDisplaying());
    }
    
    IEnumerator FinishDisplaying()
    {
        yield return new WaitForSeconds(timeStandby);
        LeanTween.scale(gameObject, Vector3.zero, time / 2);
    }
}
