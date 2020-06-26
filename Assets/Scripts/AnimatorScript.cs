using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    public GameObject[] fromZero;

    public GameObject[] secondaryInitialization;

    public GameObject[] thirdlyInitializtion;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject o in fromZero)
        {
            LeanTween.scale(o, Vector3.zero, 0);
        }
        foreach (GameObject o in secondaryInitialization)
        {
            LeanTween.scale(o, Vector3.zero, 0);
        }

        foreach (GameObject o in thirdlyInitializtion)
        {
            LeanTween.scale(o, Vector3.zero, 0);
        }
        foreach (GameObject o in fromZero)
        {
            LeanTween.scale(o, Vector3.one, 0.5f).setOnComplete(InitializeSecondary);
        }
        
    }

    private void InitializeSecondary()
    {
        foreach (GameObject o in secondaryInitialization)
        {
            LeanTween.scale(o, Vector3.one, 0.5f).setOnComplete(InitializeThirdly);
        }
    }



    private void InitializeThirdly()
    {
        foreach (GameObject o in thirdlyInitializtion)
        {
            LeanTween.scale(o, Vector3.one, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
