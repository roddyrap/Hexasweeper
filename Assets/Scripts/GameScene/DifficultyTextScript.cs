using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyTextScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "Current Difficulty:\n<color=" + Difficulty.currentDifficulty.color + ">" + Difficulty.currentDifficulty.name + "</color>";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
