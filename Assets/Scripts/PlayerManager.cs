using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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
    GameObject[] doorAnims;
    bool levelStartAnim;
    public TMP_Text abilityCountText;
    public List<GameObject> deactivatedGswitches = new List<GameObject>();
    int bodysLeft;
    public GameObject audioPrefab;
    public AudioClip gforceClip;
    public AudioClip dieClip;
    public AudioClip setBlockClip;
    List<GameObject> placedBodys = new List<GameObject>();
    private void Start()
    {
        Application.targetFrameRate = 200;
        rb = GetComponent<Rigidbody2D>();
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
        abilityCountText.text = "0" + bodysLeft.ToString();
        StartCoroutine(spawnTrail());
        g = rb.gravityScale;
        levels[m_levelIndex].doorAnim.SetTrigger("open");
        levelStartAnim = false;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Spike")
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            respawn = true;
            print("die");
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(dieClip, 0.7f, 1.3f, 1);
            rb.gravityScale = Mathf.Abs(g);
            g = rb.gravityScale;
            rb.velocity = Vector2.zero;
            for (int i = 0; i < deactivatedGswitches.Count; i++)
            {
                deactivatedGswitches[i].GetComponent<SpriteRenderer>().enabled = true;
                deactivatedGswitches[i].GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "door")
        {
            NextLevel();
        }
        if (other.tag == "gswitch")
        {
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(gforceClip, 0.2f, .4f, .6f);
            Debug.Log("change gravity");
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
        placedBodys.Clear();
        Camera.main.GetComponent<CameraPosition>().NextLevel();
        transform.position = levels[m_levelIndex].startPos.position;
        rb.velocity = Vector2.zero;
        bodysLeft = levels[m_levelIndex].bodys;
        levels[m_levelIndex].doorAnim.SetTrigger("open");
        abilityCountText.text = "0" + bodysLeft.ToString();
        foreach (GrabblingPoint p in levels[m_levelIndex].grapps)
        {
            p.available = true;
        }
        foreach (PlatformEffector2D e in effector2Ds)
        {
            Debug.Log(e + " is now 0");
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
    private void Update()
    {
        float distanceToSpawn;
        distanceToSpawn = Vector2.Distance(transform.position, levels[m_levelIndex].startPos.position);
        canability = levels[m_levelIndex].ablity;

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = levels[m_levelIndex].startPos.position;
            foreach (GameObject x in placedBodys)
            {
                Destroy(x);
            }
            bodysLeft = levels[m_levelIndex].bodys;
            abilityCountText.text = "0" + bodysLeft.ToString();
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
        if (Input.GetKeyDown(KeyCode.Space))
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
        abilityCountText.enabled = levels[m_levelIndex].ablity;
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
            for (int i = 0; i < deactivatedGswitches.Count; i++)
            {
                deactivatedGswitches[i].GetComponent<SpriteRenderer>().enabled = true;
                deactivatedGswitches[i].GetComponent<BoxCollider2D>().enabled = true;
            }
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(setBlockClip, 0.7f, 1.3f, 1);
            bodysLeft--;
            abilityCountText.text = "0" + bodysLeft.ToString();
            GameObject body = new GameObject();
            body.AddComponent<SpriteRenderer>();
            body.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            body.GetComponent<SpriteRenderer>().color = Color.yellow;
            body.AddComponent<BoxCollider2D>();
            body.layer = 6;
            body.transform.localScale = gameObject.transform.localScale;
            body.transform.position = transform.position;
            transform.position = levels[m_levelIndex].startPos.position;
            placedBodys.Add(body);
            rb.gravityScale = Mathf.Abs(g);
            g = Mathf.Abs(g);
            rb.velocity = Vector2.zero;
        }
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
        public Level(Vector3 pos, bool ab, Animator anim, Transform dPos, int b, GrabblingPoint[] grapps)
        {
            startPos.position = pos;
            ablity = ab;
            doorAnim = anim;
            dPos = doorPos;
            bodys = b;
            this.grapps = grapps;
        }
    }
}
