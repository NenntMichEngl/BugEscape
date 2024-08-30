using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
public class PlayerManager : MonoBehaviour
{
    public int startLevel;
    PlatformEffector2D[] effector2Ds;
    Vector2 spawnPoint;
    public List<Level> levels = new List<Level>();
    int m_levelIndex = 0;
    public GameObject trailPrefab;
    public bool canability;
    bool respawn;
    Rigidbody2D rb;
    public float lerpSpeed;
    float g;
    BodysLeft bodyManager;
    GameObject[] doorAnims;
    bool levelStartAnim;
    DoorButton[] doorButtons;
    public List<GameObject> deactivatedGswitches = new List<GameObject>();
    int bodysLeft;
    public Textbox textbox;
    public GameObject audioPrefab;
    public AudioClip gforceClip;

    public AudioClip dieClip;
    public AudioClip restartClip;
    public AudioClip setBlockClip;
    public AudioClip newLevelClip;
    public GameObject textObject;
    List<GameObject> placedBodys = new List<GameObject>();
    public AudioClip backgroundMusic;
    public GameObject restartText;
    public bool paused;
    public TMP_Text deathsText;
    public TMP_Text timeText;
    int deaths;
    float totalTime;
    public Transform cam;
    public EventSystem es;
    VoiceLineManager voiceLineManager;
    bool firstDeath = true;
    int deathsInBodyLevel;
    private void Start()
    {
        voiceLineManager = GameObject.FindObjectOfType<VoiceLineManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        bodyManager = GameObject.FindObjectOfType<BodysLeft>();
        Application.targetFrameRate = 200;
        rb = GetComponent<Rigidbody2D>();
        doorButtons = GameObject.FindObjectsOfType<DoorButton>();
        restartText.SetActive(false);
        m_levelIndex = startLevel;
        foreach (GrabblingPoint p in levels[m_levelIndex].grapps)
        {
            p.available = true;
        }
        transform.position = levels[m_levelIndex].startPos.position;
        effector2Ds = FindObjectsOfType<PlatformEffector2D>();
        for (int i = 0; i < startLevel; i++)
        {
            Camera.main.GetComponent<CameraPosition>().NextLevel();

        }
        bodysLeft = levels[m_levelIndex].bodys;

        bodyManager.changeBodys(bodysLeft);
        StartCoroutine(spawnTrail());
        if (levels[m_levelIndex].displayText != "")
        {
            textObject.SetActive(true);
            textbox.textMeshPro.text = levels[m_levelIndex].displayText;
        }
        else
        {
            textObject.SetActive(false);
        }
        g = rb.gravityScale;
        levels[m_levelIndex].doorAnim.SetTrigger("open");
        levelStartAnim = false;
        GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
        o.GetComponent<AudioPrefab>().StartClip(backgroundMusic, 0.4f, 0.5f, .1f * PlayerPrefs.GetFloat("musicvolume"), false, true);
    }
    void Die()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        respawn = true;
        GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
        o.GetComponent<AudioPrefab>().StartClip(dieClip, 0.7f, 1.3f, 1 * PlayerPrefs.GetFloat("volume"), true, false);
        rb.gravityScale = Mathf.Abs(g);
        g = rb.gravityScale;
        levels[m_levelIndex].doorAnim.SetTrigger("open");
        deaths++;
        if (m_levelIndex == 3)
        {
            deathsInBodyLevel++;
            if (deathsInBodyLevel % 3 == 0)
            {
                if (firstTimePassed)
                    voiceLineManager.TwoDeathsInBodyLevel();
            }
        }
        rb.velocity = Vector2.zero;
        for (int i = 0; i < deactivatedGswitches.Count; i++)
        {
            deactivatedGswitches[i].GetComponent<SpriteRenderer>().enabled = true;
            deactivatedGswitches[i].GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Spike")
        {
            if (firstDeath)
            {
                voiceLineManager.FirstDeath();
                firstDeath = false;
            }
            Die();
        }
    }
    bool firstTimePassed = true;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "door")
        {
            NextLevel();
        }
        if (other.tag == "PassedGap")
        {
            if (firstTimePassed)
            {
                firstTimePassed = false;
                voiceLineManager.WhatImtalkingAbout();
            }

        }
        if (other.tag == "gswitch")
        {
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(gforceClip, 0.2f, .4f, .6f * PlayerPrefs.GetFloat("volume"), true, false);

            rb.gravityScale = -g;
            foreach (PlatformEffector2D e in effector2Ds)
            {
                e.rotationalOffset = e.rotationalOffset + 180;
            }
            g = rb.gravityScale;
            deactivatedGswitches.Add(other.gameObject);
            other.GetComponent<SpriteRenderer>().enabled = false;
            other.GetComponent<BoxCollider2D>().enabled = false;
            other.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void NextLevel()
    {

        foreach (GrabblingPoint p in levels[m_levelIndex].grapps)
        {
            p.available = false;
        }

        m_levelIndex++;

        if (m_levelIndex == 1)
            voiceLineManager.SecondLevelReached();

        if (levels[m_levelIndex].displayText != "")
        {
            textObject.SetActive(true);
            textbox.textMeshPro.text = levels[m_levelIndex].displayText;

        }
        else
        {
            textObject.SetActive(false);
        }
        restartText.SetActive(false);
        placedBodys.Clear();
        Camera.main.GetComponent<CameraPosition>().NextLevel();
        transform.position = levels[m_levelIndex].startPos.position;
        rb.velocity = Vector2.zero;
        bodysLeft = levels[m_levelIndex].bodys;
        levels[m_levelIndex].doorAnim.SetTrigger("open");
        bodyManager.changeBodys(bodysLeft);
        GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
        o.GetComponent<AudioPrefab>().StartClip(newLevelClip, 0.4f, 0.5f, .7f * PlayerPrefs.GetFloat("volume"), true, false);
        foreach (GrabblingPoint p in levels[m_levelIndex].grapps)
        {
            p.available = true;
        }
        foreach (PlatformEffector2D e in effector2Ds)
        {
            e.rotationalOffset = 0;
        }
    }
    IEnumerator effector()
    {
        foreach (PlatformEffector2D e in effector2Ds)
        {
            e.rotationalOffset = e.rotationalOffset + 180;
        }
        yield return new WaitForSeconds(.3f);
        foreach (PlatformEffector2D e in effector2Ds)
        {
            e.rotationalOffset = e.rotationalOffset - 180;
        }
    }
    public GameObject gameUI;
    public GameObject pausedUI;
    public Button resumeButton;

    public void resumeGame()
    {
        gameUI.SetActive(true);
        pausedUI.SetActive(false);
        paused = false;

        Time.timeScale = 1;
    }
    bool firstTimeFellOfMap = true;
    private void Update()
    {
        if (!paused)
            totalTime += Time.deltaTime;
        float distanceToSpawn;
        distanceToSpawn = Vector2.Distance(transform.position, levels[m_levelIndex].startPos.position);
        canability = levels[m_levelIndex].ablity;
        if (paused)
        {
            if (es.currentSelectedGameObject == null)
            {
                resumeButton.Select();
            }
        }
        if (Mathf.Abs(cam.position.y - transform.position.y) > 25)
        {
            if (firstTimeFellOfMap)
            {
                firstTimeFellOfMap = false;
                voiceLineManager.FellOfMap();
            }
            Die();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            deaths++;
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(restartClip, 0.7f, 1.3f, 1 * PlayerPrefs.GetFloat("volume"), true, false);
            restartText.SetActive(false);
            transform.position = levels[m_levelIndex].startPos.position;
            foreach (GameObject x in placedBodys)
            {
                Destroy(x);
            }
            foreach (DoorButton b in doorButtons)
            {
                b.isOpen = false;
            }
            bodysLeft = levels[m_levelIndex].bodys;
            bodyManager.changeBodys(bodysLeft);
            placedBodys.Clear();
            foreach (GameObject x in deactivatedGswitches)
            {
                x.SetActive(true);
                x.GetComponent<SpriteRenderer>().enabled = true;
                x.GetComponent<BoxCollider2D>().enabled = true;
                x.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            }
            deactivatedGswitches.Clear();
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(effector());
        }
        if (Input.GetKeyDown(KeyCode.Space) && !paused)
        {
            if (g > 0)
            {
                foreach (PlatformEffector2D e in effector2Ds)
                {
                    e.rotationalOffset = 0;
                }
            }
            else
            {
                foreach (PlatformEffector2D e in effector2Ds)
                {
                    e.rotationalOffset = 180;
                }

            }

        }

        if (distanceToSpawn < 0.25f)
        {
            foreach (GameObject x in deactivatedGswitches)
            {
                x.SetActive(true);
                x.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            }
            respawn = false;
            GetComponent<BoxCollider2D>().enabled = true;
            rb.gravityScale = Mathf.Abs(g);
            g = rb.gravityScale;
        }
        if (respawn)
        {
            rb.gravityScale = 0;

            GetComponent<BoxCollider2D>().enabled = false;
            transform.position = levels[m_levelIndex].startPos.position;
        }
        if (Vector2.Distance(transform.position, levels[m_levelIndex].startPos.position) < 0.15f)
        {
            levelStartAnim = false;
        }
        if (levelStartAnim)
        {
            transform.position = Vector2.Lerp(new Vector2(transform.position.x, levels[m_levelIndex].startPos.position.y),
            levels[m_levelIndex].startPos.position, Time.deltaTime * 2);
        }
        if (Input.GetKeyDown("f") && canability && bodysLeft > 0 && !GetComponent<PlayerManager>().respawn)
        {
            if (Vector2.Distance(transform.position, levels[m_levelIndex].startPos.position) > 0.7f)
            {
                for (int i = 0; i < deactivatedGswitches.Count; i++)
                {
                    deactivatedGswitches[i].GetComponent<SpriteRenderer>().enabled = true;
                    deactivatedGswitches[i].GetComponent<BoxCollider2D>().enabled = true;
                }
                GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
                o.GetComponent<AudioPrefab>().StartClip(setBlockClip, 0.7f, 1.3f, 1 * PlayerPrefs.GetFloat("volume"), true, false);
                bodysLeft--;
                bodyManager.changeBodys(bodysLeft);
                GameObject body = new GameObject();
                levels[m_levelIndex].doorAnim.SetTrigger("open");
                body.AddComponent<SpriteRenderer>();
                body.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                body.GetComponent<SpriteRenderer>().color = Color.yellow;
                body.AddComponent<BoxCollider2D>();

                Rigidbody2D r = body.AddComponent<Rigidbody2D>();
                r.bodyType = RigidbodyType2D.Kinematic;
                r.sleepMode = RigidbodySleepMode2D.NeverSleep;
                body.layer = 6;
                if (bodysLeft == 0)
                {
                    restartText.SetActive(true);
                }
                body.transform.localScale = gameObject.transform.localScale;
                body.transform.position = transform.position;
                transform.position = levels[m_levelIndex].startPos.position;
                placedBodys.Add(body);
                rb.gravityScale = Mathf.Abs(g);
                g = Mathf.Abs(g);
                rb.velocity = Vector2.zero;
            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (!paused)
            {
                resumeButton.Select();
                gameUI.SetActive(false);
                pausedUI.SetActive(true);
                paused = true;
                deathsText.text = "Deaths: " + deaths.ToString();
                string timestring = TimeSpan.FromSeconds(totalTime).Hours.ToString() + ":" + TimeSpan.FromSeconds(totalTime).Minutes.ToString() + ":" + TimeSpan.FromSeconds(totalTime).Seconds.ToString();
                timeText.text = timestring;
                Time.timeScale = 0;
            }



        }

    }
    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    IEnumerator spawnTrail()
    {
        yield return new WaitForSeconds(0.2f);
        var trailO = Instantiate(trailPrefab, transform.position, Quaternion.identity) as GameObject;
        Destroy(trailO, 1f);
        StartCoroutine(spawnTrail());
    }
    [System.Serializable]
    public class Level
    {
        public Transform startPos;
        public bool ablity;
        public Animator doorAnim;
        public Transform doorPos;
        public int bodys;
        public GrabblingPoint[] grapps;
        public string displayText;
        public Level(Vector3 pos, bool ab, Animator anim, Transform dPos, int b, GrabblingPoint[] grapps, string displayText)
        {
            startPos.position = pos;
            ablity = ab;
            doorAnim = anim;
            dPos = doorPos;
            bodys = b;
            this.grapps = grapps;
            this.displayText = displayText;
        }
    }
}
