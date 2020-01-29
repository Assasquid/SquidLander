using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squid : MonoBehaviour
{
    [SerializeField] float speedSwim = 200f;
    [SerializeField] float rcsSwim = 300f;

    Rigidbody rigidBody;
    AudioSource squidSwim;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        squidSwim = GetComponent<AudioSource>();
    }

    void Update()
    {
        Swim();
        Rotate();

    }

    private void Swim()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float speedThisFrame = speedSwim * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * speedThisFrame);
            if (!squidSwim.isPlaying) // so it doesn't layer
            {
                squidSwim.Play();
            }
        }
        // the following is not necessary in our case, the sound is a one shot
        // maybe consider using an array of random sounds to make it sound better
        /*else 
        {
            squidSwim.Stop();
        }*/
    }

    private void Rotate()
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
