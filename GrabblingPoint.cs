using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabblingPoint : MonoBehaviour
{
    public bool active;
    public bool nearest;
    public float rotationSpeed;
    public Vector3 nearstSize;
    Vector3 idleSize;
    public float lerpSpeed;
    Vector3 goToSize;
    public Color activeColor;
    Color idleColor;
    SpriteRenderer re;
    public Color goToColor;
    private void Start()
    {
        re = GetComponent<SpriteRenderer>();
        idleColor = re.color;
        idleSize = transform.localScale;
        goToColor = idleColor;
    }
    void Update()
    {

        if (nearest)
        {
            goToSize = nearstSize;
        }
        else
        {
            goToSize = idleSize;
        }

        if (active)
        {
            goToColor = activeColor;
            goToSize = nearstSize;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, goToSize, lerpSpeed * Time.deltaTime);
        re.color = Color.Lerp(re.color, goToColor, lerpSpeed * Time.deltaTime);

    }
}
