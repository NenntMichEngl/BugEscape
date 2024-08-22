using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float dashSpeed;
    public float dashTime;
    public int directionX = 1;
    public float coolDown = 1;
    Rigidbody2D rb;
    public float t;
    public bool dash;
    bool timerStarted;
    PlayerMovement pm;
    bool checkdir = true;
    bool canDash = true;
    bool dashTimer = true;
    public bool dashEnenabled;

    float g;
    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        g = rb.gravityScale;
    }
    private void Update()
    {
        DashAttack();
    }
    public void DashAttack()
    {
        float horizonzal = Input.GetAxisRaw("Horizontal");
        if(checkdir)
        {
            if (horizonzal > 0)
            {
                directionX = 1;
            }
            if (horizonzal == 0)
            {
                directionX = 0;
            }
            if (horizonzal < 0)
            {
                directionX = -1;
            }

        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && dashEnenabled)
        {
            StartCoroutine(timer());
            StartCoroutine(Dashtimer());
            
        }
        if (dash)
        {
            rb.velocity = new Vector2(directionX * dashSpeed, rb.velocity.y);
        }
        if(dashTimer)
        {
            canDash = true;
        }
    }
    IEnumerator timer()
    {
        checkdir = false;
        //rb.velocity = Vector2.zero;
        dash = true;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = g;
        dash = false;
        checkdir = true;
    }
    IEnumerator Dashtimer()
    {
        dashTimer = false;
        canDash = false;
        yield return new WaitForSeconds(coolDown);
        dashTimer = true;
    }
}
