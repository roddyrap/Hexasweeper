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
        popup.SetTitle("RULES:");
        popup.SetContent("CONTROLS:\n\nZoom with the scroll wheel or the + and - keys.\nMove the board with the arrows or the WASD keys.\nLeft clicking on tiles reveals whats under them.\nRight clicking on tiles sets flags on them.\nMiddle clicking (clicking on the scroll wheel) on a tile with the same amount of adjacent flags as bombs nearby will reveal all tiles near it that aren't flagged. Including bombs if the adjacent flags were placed incorrectly.\n\n" + 
                         "Rules:\n\nThe goal of Minesweeper is to uncover all the tiles on a grid that do not contain mines without being \"blown up\" by clicking on a tile with a mine underneath.\n" +
                         "The location of most mines is discovered through a logical process, but some require guessing, usually with a 50-50 chance of being correct.\n\n Clicking on a tile " +
                         "will reveal what is hidden underneath it. If a tile has no bombs near it (marked as a tile without a number when revealed) all adjacent tiles to it will " +
                         "also automatically be revealed. This can trigger a chain reaction of empty tiles that are being revealed together.\n" +
                         "other tiles contain numbers (from 1 to 6), with each number being the number of mines adjacent to the uncovered tile.\n\n" +
                         "To help the player avoid hitting a mine, the location of a suspected mine can be marked by flagging it with the right mouse button. The game is won once all blank or numbered " +
                         "squares have been uncovered by the player and all mines were flagged." + 
                         "\n\nThe game board comes in three set sizes with a predetermined number of mines: \"beginner\", \"intermediate\", and \"expert\"." + 
                         "\n\nNote: If you have no access to a proper mouse, clicking on the logo in the screen of the game would allow every button to function like a middle button when clicking on a revealed tile. ");
    }

    public void CreditsButton()
    {
        popup.gameObject.transform.parent.gameObject.SetActive(true);
        popup.SetTitle("Credits:");
        popup.SetContent("Developed by:\nNimrod Rappaport" +
                         "\n\nArt:\nFlag By Font Awesome by Dave Gandy - https://fortawesome.github.com/Font-Awesome, CC BY-SA 3.0, https://commons.wikimedia.org/w/index.php?curid=24230921" + 
                         "\n\nMusic:\nLuminist - Plastic Sea (Korg Volca Keys + MS20 Mini).\nChad Crouch - Algorithms.");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
