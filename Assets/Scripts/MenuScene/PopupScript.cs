using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScript : MonoBehaviour
{
    private TextMeshProUGUI title;

    private TextMeshProUGUI content;
    private ContentSizeFitter csf;
    private ScrollRect sr;
    private GameObject darkener;

    private void Awake()
    {
        darkener = transform.parent.gameObject;
        title = transform.Find("PopupTitle").GetComponent<TextMeshProUGUI>();
        Debug.Log(title);
        content = transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<TextMeshProUGUI>();
        sr = transform.Find("Scroll View").GetComponent<ScrollRect>();
        csf = content.gameObject.GetComponent<ContentSizeFitter>();
    }
    public void SetContent(String contentString)
    {
        content.text = contentString;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sr.horizontal = false;
        sr.verticalScrollbar.value = 1;
    }

    public void SetTitle(String titleString)
    {
        title.text = titleString;
    }

    public void Quit()
    {
        darkener.SetActive(false);
    }
}
