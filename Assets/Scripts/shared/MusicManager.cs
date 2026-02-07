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

    [Range(0.0f, 1.0f)]
    public float menuMusicVolume = 1.0f;

    public AudioClip gameMusic;

    [Range(0.0f, 1.0f)]
    public float gameMusicVolume = 1.0f;


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
        if (newScene.name == "Menu")
        {
            source.clip = menuMusic;
            source.volume = menuMusicVolume;
        }
        else
        {
            source.clip = gameMusic;
            source.volume = gameMusicVolume;
        }

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
