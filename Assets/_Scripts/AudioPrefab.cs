using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPrefab : MonoBehaviour
{
    public AudioSource source;
    void Start()
    {

    }
    public void StartClip(AudioClip clip, float pitchstart, float pitchend, float volume, bool destroy, bool loop)
    {
        source.clip = clip;
        source.pitch = Random.Range(pitchstart, pitchend);
        source.volume = volume;
        if (destroy)
            Destroy(gameObject, 2);
        if (loop)
            source.loop = true;
        source.Play();
    }
}
