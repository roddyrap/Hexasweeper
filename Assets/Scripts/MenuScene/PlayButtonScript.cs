using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        Difficulty.ChangeDifficultyByName(transform.parent.Find("DifficultyDropdown").Find("Label").GetComponent<TextMeshProUGUI>().text);
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
