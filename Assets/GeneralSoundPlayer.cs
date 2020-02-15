using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundPlayer : MonoBehaviour
{
    private static GameObject musicManagerInstance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (musicManagerInstance == null)
        {
            musicManagerInstance = gameObject;
        }
        else
        {
            Object.Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
