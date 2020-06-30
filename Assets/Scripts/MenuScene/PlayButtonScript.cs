using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButtonScript : MonoBehaviour
{
    private PopupScript popup;

    private TMP_Dropdown diffDropdown;
    // Start is called before the first frame update
    void Awake()
    {
        popup = GameObject.Find("Darkener").transform.Find("Popup").GetComponent<PopupScript>();
        diffDropdown = transform.parent.Find("DifficultyDropdown").GetComponent<TMP_Dropdown>();
        diffDropdown.value = PlayerPrefs.GetInt("WantedDiff");

    }

    private void Start()
    {
        try
        {
            GameObject.Find("Darkener").SetActive(false);
        }
        catch (NullReferenceException)
        {
            
        }

    }

    public void PlayButton()
    {
        Difficulty.ChangeDifficultyByName(transform.parent.Find("DifficultyDropdown").Find("Label").GetComponent<TextMeshProUGUI>().text);
        PlayerPrefs.SetInt("WantedDiff", diffDropdown.value);
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    public void RulesButton()
    {
        popup.gameObject.transform.parent.gameObject.SetActive(true);
        popup.SetTitle("Rules:");
        popup.SetContent("The goal of Minesweeper is to uncover all the tiles on a grid that do not contain mines without being \"blown up\" by clicking on a tile with a mine underneath.\n" +
                         "The location of most mines is discovered through a logical process, but some require guessing, usually with a 50-50 chance of being correct.\n\n Clicking on a tile " +
                         "will reveal what is hidden underneath it. If a tile has no bombs near it (marked as a tile without a number when revealed) all adjacent tiles to it will " +
                         "also automatically be revealed. This can trigger a chain reaction of empty tiles that are being revealed together.\n" +
                         "other tiles contain numbers (from 1 to 6), with each number being the number of mines adjacent to the uncovered tile.\n\n" +
                         "To help the player avoid hitting a mine, the location of a suspected mine can be marked by flagging it with the right mouse button. The game is won once all blank or numbered " +
                         "squares have been uncovered by the player and all mines were flagged." +
                         "Middle clicking (clicking on the scroll wheel) on a tile with the same amount of adjacent flags as bombs nearby will reveal all tiles near it that aren't flagged. Including bombs if the adjacent flags were placed incorrectly.\n\n" +
                         "The game board comes in three set sizes with a predetermined number of mines: \"beginner\", \"intermediate\", and \"expert\".");
    }

    public void CreditsButton()
    {
        popup.gameObject.transform.parent.gameObject.SetActive(true);
        popup.SetTitle("Credits:");
        popup.SetContent("Copyright (C) 2020  Nimrod Rappaport\nThis work is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License. To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-nd/4.0/" +
                         "\n\nArt:\nFlag By Font Awesome by Dave Gandy - https://fortawesome.github.com/Font-Awesome, CC BY-SA 3.0, https://commons.wikimedia.org/w/index.php?curid=24230921");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
