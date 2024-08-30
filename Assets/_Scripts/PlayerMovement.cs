using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public float groundCheckRadius;
    public float jumpForce;
    public float speed;
    public bool grounded;
    public Transform groundCheck;
    public Transform groundCheckg;
    public LayerMask ground;
    public int jumpAmount;
    public int jumpRemaining;
    PlayerAttack pa;
    Rigidbody2D rb;

    public bool touchingL;
    public bool touchingR;
    public CinemachineVirtualCamera cmcam;
    public ParticleSystem jumpEffect;
    public ParticleSystem smashEffect;
    public bool canSingleJump;
    bool timeOver;
    public bool canDoubleJump;
    float t;
    bool jump;
    Quaternion toRotation;
    public float jumpLerpSpeed;
    GrapplingHook gh;
    public bool afterSwing;
    public AudioClip groundHitClip;
    public AudioClip jumpClip;
    PlayerManager pm;
    public GameObject audioPrefab;
    private void Start()
    {
        pa = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody2D>();
        gh = GetComponent<GrapplingHook>();
        pm = GetComponent<PlayerManager>();
        jumpRemaining = jumpAmount;
    }
    bool lastState = false;



    private void Update()
    {
        if (pm.paused)
            return;
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, jumpLerpSpeed * Time.deltaTime);
        if (grounded)
        {
            afterSwing = false;
            jumpRemaining = 1;
        }
        if (t < 0.1f && canSingleJump)
        {
            if (canSingleJump)
            {
                canSingleJump = true;
            }

            t += Time.deltaTime;
            canDoubleJump = false;
        }
        else if ((t >= 0.1f && !grounded) || canSingleJump == false)
        {
            canSingleJump = false;
            canDoubleJump = true;
        }

        if (pa.dash == false && !gh.isGrappling)
        {

            float horizontal = Input.GetAxisRaw("Horizontal");
            if (!afterSwing)
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
                if (horizontal < 0)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
            }

        }
        if (grounded && !jump)
            canSingleJump = true;

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && (canSingleJump || (jumpRemaining > 0 && canDoubleJump)))
        {
            toRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90, 0);
            jump = true;
            StartCoroutine(jumping());
            if (canSingleJump)
            {
                jumpRemaining = jumpAmount;
                canSingleJump = false;
            }

            if (!grounded && jumpAmount != jumpRemaining)
            {
                jumpEffect.Play();
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(jumpClip, 0.7f, 1.3f, .2f * PlayerPrefs.GetFloat("volume"), true, false);
            if (rb.gravityScale > 0)
            {
                rb.AddForce(new Vector2(-rb.velocity.x / 2, jumpForce), ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(-rb.velocity.x / 2, -jumpForce), ForceMode2D.Impulse);
            }


            if (jumpRemaining == jumpAmount)
            {
                t = 0;
            }
            jumpRemaining--;
        }


    }

    private void FixedUpdate()
    {
        Collider2D[] colliders;
        if (rb.gravityScale > 0)
        {
            colliders = Physics2D.OverlapBoxAll(groundCheck.position, new Vector2(groundCheckRadius, 0.1f), ground);
        }
        else
        {
            colliders = Physics2D.OverlapBoxAll(groundCheckg.position, new Vector2(groundCheckRadius, 0.1f), ground);
        }

        Collider2D[] wallL = Physics2D.OverlapBoxAll(transform.position + new Vector3(-0.56f, -0.20f, 0), new Vector2(0.1f, 0.2f), ground);
        Collider2D[] wallR = Physics2D.OverlapBoxAll(transform.position + new Vector3(0.56f, -0.20f, 0), new Vector2(0.1f, 0.2f), ground);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {

                if (!grounded)
                {
                    CameraShake();
                    smashEffect.Play();

                }
                if (rb.gravityScale > 0 && rb.velocity.y <= 0)
                    grounded = true;
                if (rb.gravityScale < 0 && rb.velocity.y >= 0)
                    grounded = true;


            }
        }
        for (int i = 0; i < wallL.Length; i++)
        {
            if (wallL[i].gameObject != gameObject)
            {
                touchingL = true;
            }
        }
        if (colliders.Length == 1)
        {
            if (grounded)
            {
                t = 0;

            }
            grounded = false;

        }

        for (int i = 0; i < wallR.Length; i++)
        {
            if (wallR[i].gameObject != gameObject)
            {
                touchingR = true;
            }
        }

        if (wallR.Length == 1)
        {
            touchingR = false;

        }
        else
        {
            touchingR = true;
        }
        if (wallL.Length == 1)
        {
            touchingL = false;

        }
        else
        {
            touchingL = true;
        }
        if (lastState == false && grounded == true)
        {
            GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
            o.GetComponent<AudioPrefab>().StartClip(groundHitClip, 0.7f, 1.3f, .2f * PlayerPrefs.GetFloat("volume"), true, false);

        }

        lastState = grounded;
    }
    public void CameraShake()
    {
        StartCoroutine(shakeTimer());
    }
    IEnumerator shakeTimer()
    {
        cmcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2;
        yield return new WaitForSeconds(.1f);
        cmcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(groundCheckRadius, 0.1f));
        Gizmos.DrawWireCube(groundCheckg.position, new Vector2(groundCheckRadius, 0.1f));
        Gizmos.DrawWireCube(transform.position + new Vector3(0.56f, -0.20f, 0), new Vector2(0.1f, 0.2f));
        Gizmos.DrawWireCube(transform.position + new Vector3(-0.56f, -0.20f, 0), new Vector2(0.1f, 0.2f));
    }
    void OnCollisionEnter2D()
    {


    }

    IEnumerator jumping()
    {
        yield return new WaitForSeconds(0.2f);
        jump = false;
    }
}
