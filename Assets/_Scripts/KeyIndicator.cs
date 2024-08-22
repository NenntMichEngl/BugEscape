using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyIndicator : MonoBehaviour
{
    public string key;
    public Color indicatorColor;
    Color idleColor;
    Color nextColor;
    SpriteRenderer render;
    public float lerpSpeed = 1f;
    private void Start() {
        render = GetComponent<SpriteRenderer>();
        nextColor = render.color;
        idleColor = nextColor;
    }
    private void Update() {
        if(Input.GetKeyDown(key))
        {
            nextColor = indicatorColor;
        }
        if(Input.GetKeyUp(key))
        {
            nextColor = idleColor;
        }
        render.color = Color.Lerp(render.color, nextColor, lerpSpeed * Time.deltaTime);
    }
}
