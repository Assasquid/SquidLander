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
    Collider collider;
    

    bool isTransitioning = false;
    bool collisionsDisabled = false;
    Vector3 rotationCenter;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        collider = GetComponentInChildren<CapsuleCollider>();
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
        float actualSpeed = 0.0f;

        if (Input.GetKey(KeyCode.Space))
        {
            actualSpeed = speedSwim;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                print("speed is reduced");
                actualSpeed /= 4.0f;
            }

            ApplySwim(actualSpeed);     
        }

        else
        {
            StopApplyingThrust();
        }

        ComputeRotationCenter(actualSpeed, speedSwim / 4.0f);

    }

    // Replace the center of rotation of the squid according to its spped.
    // The rotation center is interpolated from the center of the bouding volume to the position (the "feet") of the avatar.
    private void ComputeRotationCenter(float current_speed, float max_speed)
    {
        // interpolation_value is a value between 0.f and 1.f which represent "how far from 'center' to 'feet' we are
        // Consider it as a percentage: 0% (0.f) is 'center' and '100%' (1.f) is "feet"
        float interpolation_value = Mathf.Max(current_speed / max_speed, 1.0f);
        Vector3 center = collider.bounds.center;
        Vector3 feet = transform.position;
        rotationCenter = Vector3.Lerp(center, feet, interpolation_value);
        //Debug.Log(current_speed + "/" + max_speed + " (" + interpolation_value + ") : " + center + " -> " + new_center + " -> " + feet);
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
        transform.RotateAround(rotationCenter, Vector3.forward, rotationThisFrame);
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
