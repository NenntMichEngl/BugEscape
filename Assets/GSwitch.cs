using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSwitch : MonoBehaviour
{
    public bool goUp;
    public float speed;
    float p;
    public Material matUp;
    public Material matDown;
    public Sprite up;
    public Sprite down;
    void Start()
    {

        if (!goUp)
        {
            GetComponent<SpriteRenderer>().material = matDown;
            speed = -speed;
            GetComponent<SpriteRenderer>().sprite = down;
        }
    }


    void Update()
    {
        p += Time.deltaTime * speed;

        if (goUp)
        {
            matUp.SetTextureOffset("_MainTex", new Vector2(0, p));
        }
        else
        {
            matDown.SetTextureOffset("_MainTex", new Vector2(0, p));
        }
    }
}
