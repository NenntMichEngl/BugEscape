using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class VoiceLineManager : MonoBehaviour
{
    AudioSource source;
    public Animator audioVisualizer;
    [Header("All Clips here")]
    public AudioClip welcomeVoiceLine;
    public AudioClip wellThatWasEasy;
    public AudioClip doNotTouchRed;
    public AudioClip bigGap;
    public AudioClip whatImtalkingAbout;
    public AudioClip tryingToGo;
    public AudioClip thatIsStrange;
    public AudioClip gettingThehangOfIt;
    public AudioClip aLotOfFun;
    public AudioClip cantDoThatRightNow;
    public AudioClip dontLookFriendly;
    public AudioClip thanksForPlaying;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = PlayerPrefs.GetFloat("volume");

        PlayVoiceLine(welcomeVoiceLine, 2);
    }
    void Update()
    {
        audioVisualizer.SetBool("playing", source.isPlaying);

    }
    public void gotToCanonLevel()
    {
        PlayVoiceLine(dontLookFriendly, .5f);
    }
    IEnumerator playVoicelinewithTimer(AudioClip line, float delay)
    {
        source.Stop();
        yield return new WaitForSeconds(delay);

        source.clip = line;
        source.Play();
    }
    public void GettingThehangOfIt()
    {
        PlayVoiceLine(gettingThehangOfIt, 1);
    }
    public void ChangedG()
    {
        PlayVoiceLine(thatIsStrange, 0.5f);
    }
    public void FellOfMap()
    {
        PlayVoiceLine(tryingToGo, 0.2f);
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
    public void PassedGrappleGap()
    {
        PlayVoiceLine(aLotOfFun, .2f);
    }
    public void NOBodysLeft()
    {
        PlayVoiceLine(cantDoThatRightNow, 0);
    }
    public void LastLevel()
    {
        PlayVoiceLine(thanksForPlaying, 2);
    }
    public void FirstDeath()
    {
        PlayVoiceLine(doNotTouchRed, .3f);
    }


}
