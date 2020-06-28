using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    private TextMeshProUGUI stateText;
    private TextMeshProUGUI endStats;
    private TextMeshProUGUI highScoreText;
    // Start is called before the first frame update
    private void Awake()
    {
        stateText = transform.Find("State").GetComponent<TextMeshProUGUI>();
        highScoreText = transform.Find("highScoreText").GetComponent<TextMeshProUGUI>();
        endStats = transform.Find("End Stats").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void End(bool isWin, TimeSpan time)
    {
        String diffName = Difficulty.currentDifficulty.name;
        if (isWin)
        {
            if (PlayerPrefs.HasKey(diffName))
            {
                if (TimeSpanFromString(PlayerPrefs.GetString(diffName)) > time) PlayerPrefs.SetString(diffName, StatsScript.FormatDateTime(time));
            } 
            else PlayerPrefs.SetString(diffName, StatsScript.FormatDateTime(time));
        }
        stateText.text = isWin ? "Win" : "Lose";
        endStats.text = "Time:\n" + StatsScript.FormatDateTime(time) + "\n" + "Current Difficulty:\n" + Difficulty.currentDifficulty.name;
        highScoreText.text = "High Score:\n" + (PlayerPrefs.GetString(diffName) == ""? "No High score yet!" : PlayerPrefs.GetString(diffName));
    }

    public static TimeSpan TimeSpanFromString(String timeString)
    {
        String pattern = @"(\d+):(\d+):(\d+)";
        Regex rx = new Regex(pattern);
        Match match = rx.Match(timeString);
        TimeSpan ts = new TimeSpan(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[3].Value));
        
        
        
        return ts;
    }

    public void ToMenu()
    {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
