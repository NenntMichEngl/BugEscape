using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Vector3 nextPos;
    public float lerpSpeed;
    bool shake;
    public Vector3 offset = new Vector3(0, 0, -10);
    private void Start()
    {
        nextPos = transform.position;
    }
    public void NextLevel()
    {
        nextPos = nextPos + new Vector3(0, 29.85f, 0);
    }
    private void Update()
    {

        if (shake)
        {
            transform.position = Vector3.Lerp(transform.position, nextPos + offset, 10 * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, nextPos + offset, lerpSpeed * Time.deltaTime);
        }
    }
    public void CameraShake(float speed)
    {
        shake = true;
        StartCoroutine(shakeCour(speed));
    }
    IEnumerator shakeCour(float speed)
    {
        Vector2 startpos = transform.position;
        nextPos = transform.position + new Vector3(0.5f, 0.5f);
        yield return new WaitForSeconds(speed);
    }
}
