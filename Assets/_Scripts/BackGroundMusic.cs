using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    void Update()
    {
        source.volume = PlayerPrefs.GetFloat("musicvolume") * 0.1f;
    }
}
