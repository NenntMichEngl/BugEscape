using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{


    public bool isOpen;
    public float posChangeAmount;
    Vector3 goToPos;
    Vector3 pressedPos;
    public float lerpSpeed;
    public GameObject door;
    SpriteRenderer doorRenderer;
    BoxCollider2D doorCol;
    Color goToColor;
    Color doorColor;
    Vector3 startPos;
    void Start()
    {
        doorRenderer = door.GetComponent<SpriteRenderer>();
        doorCol = door.GetComponent<BoxCollider2D>();
        doorColor = doorRenderer.color;
        goToColor = doorColor;
        goToPos = transform.position;
        startPos = transform.position;
        pressedPos = transform.position + (transform.up * -posChangeAmount);
    }

    void Update()
    {
        goToColor = isOpen ? new Color(0, 0, 0, 0) : doorColor;
        goToPos = isOpen ? pressedPos : startPos;
        doorRenderer.color = Color.Lerp(doorRenderer.color, goToColor, Time.deltaTime * lerpSpeed);
        transform.position = Vector3.Lerp(transform.position, goToPos, Time.deltaTime * lerpSpeed);
        doorCol.enabled = !isOpen;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        goToPos = pressedPos;
        isOpen = true;
    }
}
