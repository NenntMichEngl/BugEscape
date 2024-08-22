using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   Rigidbody2D rb;
    public GameObject trailPrefab;
    public void StartBullet(Vector2 vel)
    {
        StartCoroutine(spawnTrail());
         rb = GetComponent<Rigidbody2D>();
        rb.velocity = vel;
    }
    IEnumerator spawnTrail()
    {
        yield return new WaitForSeconds(0.2f);
        var trailO = Instantiate(trailPrefab, transform.position, Quaternion.identity) as GameObject;
        Destroy(trailO, 1f);
        StartCoroutine(spawnTrail());
    }
    void Start()
    {
       
    }

    void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }
}
