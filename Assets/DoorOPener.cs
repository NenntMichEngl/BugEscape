using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOPener : MonoBehaviour
{

    public GameObject door;
    SpriteRenderer doorRenderer;

    Color doorStartColor;
    Color goToColor;
    public float lerpSpeed;
    public BoxCollider2D col;
    void Start()
    {
        doorRenderer = door.GetComponent<SpriteRenderer>();
        doorStartColor = doorRenderer.color;
    }

    public bool isOpen;

    void Update()
    {
        goToColor = isOpen ? new Color(0, 0, 0, 0) : doorStartColor;
        doorRenderer.color = Color.Lerp(doorRenderer.color, goToColor, lerpSpeed * Time.deltaTime);
        col.enabled = !isOpen;

    }
}
