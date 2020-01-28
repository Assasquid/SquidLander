using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squid : MonoBehaviour
{
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
            rigidBody.AddRelativeForce(Vector3.up);
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
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward);
        }
    }
}
