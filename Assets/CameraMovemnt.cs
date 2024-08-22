using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovemnt : MonoBehaviour
{
    public Vector3 offset;
    public float speed;
    public Transform target;
    bool shake;
    float amount;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed * Time.deltaTime);

    }
    void Start()
    {
    }
}
