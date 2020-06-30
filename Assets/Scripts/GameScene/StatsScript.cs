using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsScript : MonoBehaviour
{
    public static StatsScript instance;
    public static DateTime startTime;
    private static GameObject endScreen;

    private static TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        endScreen = transform.parent.Find("End Screen").gameObject;
        instance = this;
        startTime = DateTime.Now;
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!endScreen.activeSelf) text.text = "Time:\n" + FormatDateTime(DateTime.Now-startTime) + "\nFlags Left:\n" + (Board.board == null? Difficulty.currentDifficulty.bombAmount : (Difficulty.currentDifficulty.bombAmount - Board.board.flagsOnBoard));
    }

    public static String FormatDateTime(TimeSpan date)
    {
        return (date.Hours >= 10 ? date.Hours.ToString() : "0" + date.Hours) + ":" + (date.Minutes >= 10 ? date.Minutes.ToString() : "0" + date.Minutes) + ":" + (date.Seconds >= 10 ? Mathf.Round( date.Seconds).ToString() : "0" + Mathf.Round( date.Seconds).ToString());
        
    }
}
