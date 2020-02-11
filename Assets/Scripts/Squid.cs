using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Squid : MonoBehaviour
{
    [SerializeField] float speedSwim = 200f;
    [SerializeField] float rcsSwim = 300f;
    [SerializeField] float levelLoadDelay = 3f;

    [SerializeField] AudioClip mainSwim;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip victoryTune;
    [SerializeField] float pitchControl = 1f;
    // [SerializeField] float volumeControl = 1f;

    [SerializeField] ParticleSystem swimParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem victoryParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        pitchControl = 0.25f;
    }

    void Update()
    {
        if (!isTransitioning)
        {
            RespondToSwimInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild) // check that this work as intended
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }

        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // toggle
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (isTransitioning || collisionsDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                print("Friendly Hug"); // todo remove this line
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    void OnTriggerEnter(Collider collided)
    {

        if (isTransitioning) { return; }

        if (collided.tag == "Finish")
        {
            StartSuccessSequence();
        }

    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        pitchControl = 1;
        audioSource.pitch = pitchControl;
        audioSource.PlayOneShot(victoryTune);
        victoryParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        pitchControl = 1;
        audioSource.pitch = pitchControl;
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to first level
        }
        SceneManager.LoadScene(nextSceneIndex); // todo allow for more than 2 levels
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToSwimInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                print("speed is reduced");
                ApplySwim(speedSwim / 4);
            }

            else
            { 
                ApplySwim(speedSwim); 
            }
             
        }

        else
        {
            StopApplyingThrust();
        }

    }

    private void StopApplyingThrust()
    {
        // the following is not necessary in our case, the sound is a one shot
        // maybe consider using an array of random sounds to make it sound better
        //squidSwim.Stop();
        swimParticles.Stop();
    }

    private void ApplySwim(float actualSpeed)
    {
        float speedThisFrame = actualSpeed * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * speedThisFrame);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainSwim);
        }
        swimParticles.Play();
    }

    private void RespondToRotateInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rcsSwim * Time.deltaTime);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rcsSwim * Time.deltaTime);
        }
    }

    private void RotateManually(float rotationThisFrame)
    {
        FreezeRotation(true);
        transform.Rotate(Vector3.forward * rotationThisFrame);
        FreezeRotation(false);
    }

    private void FreezeRotation(bool isFrozen) // Freeze and unfreeze Z rotation
    {
        if (isFrozen)
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        else
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }
}
