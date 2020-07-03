using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioClip menuMusic;

    public AudioClip gameMusic;

    private AudioSource source;

    public bool isPaused;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        source = GetComponent<AudioSource>();
        source.loop = true;
        SceneManager.activeSceneChanged += SwitchScene;
        source.clip = menuMusic;
        if(!isPaused) source.Play();
    }
    
    private void SwitchScene(Scene oldScene, Scene newScene)
    {
        if (newScene.name == "Menu") source.clip = menuMusic;
        else source.clip = gameMusic;
        if(!isPaused) source.Play();
    }

    public void ReversePause()
    {
        if (isPaused)
        {
            isPaused = false;
            source.Play();
        }
        else
        {
            isPaused = true;
            source.Pause();
        }
    }
}
