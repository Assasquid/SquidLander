﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Squid : MonoBehaviour
{
    [SerializeField] float speedSwim = 200f;
    [SerializeField] float rcsSwim = 300f;
    [SerializeField] AudioClip mainSwim;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip victoryTune;
    [SerializeField] float pitchControl = 1f;
    [SerializeField] float volumeControl = 1f;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        pitchControl = 0.25f;
    }

    void Update()
    {
        // todo somewhere stop sound on death
        if (state == State.Alive)
        {
            RespondToSwimInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) 
    {

        if (state != State.Alive) { return; } // ignore collisions when dead
        
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                print("Friendly Hug"); // todo remove this line
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        pitchControl = 1;
        audioSource.pitch = pitchControl;
        audioSource.PlayOneShot(victoryTune);
        Invoke("LoadNextLevel", 3f); // parameterise time
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        pitchControl = 1;
        audioSource.pitch = pitchControl;
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadFirstLevel", 1f); // parameterise time
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // todo allow for more than 2 levels
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToSwimInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplySwim();
        }
        // the following is not necessary in our case, the sound is a one shot
        // maybe consider using an array of random sounds to make it sound better
        /*else 
        {
            squidSwim.Stop();
        }*/
    }

    private void ApplySwim()
    {
        float speedThisFrame = speedSwim * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * speedThisFrame);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainSwim);
        }
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsSwim * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
