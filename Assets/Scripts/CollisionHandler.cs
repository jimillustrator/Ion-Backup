using System;
using UnityEngine;
using UnityEngine.SceneManagement; //SceneManagement needed to be added because it's a different name space than Unity Engine

public class CollisionHandler : MonoBehaviour //created by Visual Studio when the new C# script is created in Unity
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip crash;

    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionDisabled = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  
    }

    void Update()
    {
        RespondToDebugKeys();   
    }

    void RespondToDebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; //toggles collision
        }
    }


    void OnCollisionEnter(Collision other) //a prebuilt method from Unity, detecting collisions. The more recent syntax replaces "other" with "collision" as the parameter 
    {
        if (isTransitioning || collisionDisabled) { return; } //since this checks to see if isTransitioning is true, if it is, the following code in the method (the switch) gets bypassed

        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence()
    {
        isTransitioning = true; //this doesn't need to be reset manually back to false, because the initial state (false) returns when the scene is relaoded
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartCrashSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(crash);
        crashParticles.Play();
        GetComponent<Movement>().enabled = false;//Turn off Movement script to disable player control after crash
        Invoke("ReloadLevel", levelLoadDelay);
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; //buildIndex requires the scene to be added to the Build Settings
        SceneManager.LoadScene(currentSceneIndex);
    }

}
