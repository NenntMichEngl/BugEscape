using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    AudioSource source;
    [Header("All Clips hear")]
    public AudioClip welcomeVoiceLine;
    public AudioClip wellThatWasEasy;
    public AudioClip doNotTouchRed;
    public AudioClip bigGap;
    public AudioClip whatImtalkingAbout;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = PlayerPrefs.GetFloat("volume");

        PlayVoiceLine(welcomeVoiceLine, 2);
    }

    IEnumerator playVoicelinewithTimer(AudioClip line, float delay)
    {
        source.Stop();
        yield return new WaitForSeconds(delay);

        source.clip = line;
        source.Play();
    }

    void PlayVoiceLine(AudioClip line, float delay)
    {
        StartCoroutine(playVoicelinewithTimer(line, delay));
    }
    public void WhatImtalkingAbout()
    {
        PlayVoiceLine(whatImtalkingAbout, 0f);
    }
    public void SecondLevelReached()
    {
        PlayVoiceLine(wellThatWasEasy, .5f);
    }
    public void TwoDeathsInBodyLevel()
    {
        PlayVoiceLine(bigGap, 1.5f);
    }
    public void FirstDeath()
    {
        PlayVoiceLine(doNotTouchRed, .3f);
    }


}
