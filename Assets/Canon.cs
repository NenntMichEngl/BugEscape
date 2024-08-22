using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletTime;

    public GameObject bulletPrefab;
    public Transform shootPoint;
    float p = 99999;
    void Update() {
        p += Time.deltaTime;

        if(p > bulletTime)
        {
            p = 0;
            GameObject g = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
            g.GetComponent<Bullet>().StartBullet(transform.up * bulletSpeed);
        }
    }
    
}
