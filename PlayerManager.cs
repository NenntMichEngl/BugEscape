using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerManager : MonoBehaviour
{
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
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = levels[m_levelIndex].startPos.position;
        effector2Ds = FindObjectsOfType<PlatformEffector2D>();
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
            rb.gravityScale = Mathf.Abs(g);
            g = rb.gravityScale;
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
        }
    }
    public void NextLevel()
    {

        m_levelIndex++;
        Camera.main.GetComponent<CameraPosition>().NextLevel();
        transform.position = levels[m_levelIndex].startPos.position;
        rb.velocity = Vector2.zero;
        bodysLeft = levels[m_levelIndex].bodys;
        levels[m_levelIndex].doorAnim.SetTrigger("open");
        abilityCountText.text = "0" + bodysLeft.ToString();
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
            rb.gravityScale = Mathf.Abs(g);
            g = Mathf.Abs(g);
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
        public Level(Vector3 pos, bool ab, Animator anim, Transform dPos, int b)
        {
            startPos.position = pos;
            ablity = ab;
            doorAnim = anim;
            dPos = doorPos;
            bodys = b;
        }
    }
}
