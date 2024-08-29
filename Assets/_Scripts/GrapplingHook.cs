using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public DistanceJoint2D distanceJoint;
    public Transform grapplingPoint;
    public LayerMask grappleableLayer;
    public float maxGrappleDistance = 10f;
    public float grappleLineSpeed = 0.3f;
    public float slowDownForce = 10f;
    private Vector2 grappleTarget;
    public bool isGrappling = false;
    PlayerMovement pm;
    Rigidbody2D rb;
    GrabblingPoint[] points;
    public GrabblingPoint nearestPoint;
    public Vector2 grabPoint;

    GrabblingPoint activePoint;
    void Start()
    {
        points = GameObject.FindObjectsOfType<GrabblingPoint>();
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        getNearestGrabblingPoint();
        if (Input.GetKeyDown(KeyCode.K) && !pm.grounded)
        {
            StartGrapple();
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            StopGrapple();
        }
        if (pm.grounded)
        {
            StopGrapple();
        }
        if (isGrappling)
        {
            lineRenderer.SetPosition(0, grapplingPoint.position);
            Vector2 oppositeForce = -rb.velocity.normalized * slowDownForce;



            rb.AddForce(oppositeForce);

            if (rb.velocity.magnitude < 0.01f)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public float maxDistanceToGrapple;
    void getNearestGrabblingPoint()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float nearestDistance = Mathf.Infinity;
        mousePosition = transform.position;
        int nearestIndex = -1;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].available)
            {
                float d = Vector2.Distance(points[i].transform.position, mousePosition);
                if (d < nearestDistance && d < maxDistanceToGrapple)
                {
                    nearestDistance = d;
                    nearestIndex = i;
                }
            }

        }
        if (nearestIndex < 0)
        {
            nearestPoint = null;
        }
        else
        {

            nearestPoint = points[nearestIndex];
        }
        foreach (GrabblingPoint p in points)
        {
            p.nearest = false;
        }
        if (nearestPoint != null)
            nearestPoint.nearest = true;
    }
    public GameObject audioPrefab;
    public AudioClip hookClip;
    void StartGrapple()
    {
        if (nearestPoint == null)
            return;

        GameObject o = Instantiate(audioPrefab, transform.position, Quaternion.identity) as GameObject;
        o.GetComponent<AudioPrefab>().StartClip(hookClip, 0.6f, .8f, 1 * PlayerPrefs.GetFloat("volume"), true, false);
        activePoint = nearestPoint;
        activePoint.active = true;

        Vector2 direction = (Vector2)activePoint.transform.position - (Vector2)grapplingPoint.position;

        RaycastHit2D hit = Physics2D.Raycast(grapplingPoint.position, direction, maxGrappleDistance, grappleableLayer);

        if (hit.collider != null)
        {
            isGrappling = true;
            grappleTarget = nearestPoint.transform.position;

            lineRenderer.enabled = true;
            StartCoroutine(GrappleLineRoutine());
        }
    }

    IEnumerator GrappleLineRoutine()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = grapplingPoint.position;

        while (elapsedTime < grappleLineSpeed)
        {
            if (!isGrappling)
                yield return null;
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / grappleLineSpeed);
            Vector2 currentPosition = Vector2.Lerp(startPosition, grappleTarget, t);
            pm.afterSwing = true;
            lineRenderer.SetPosition(1, currentPosition);
            yield return null;
        }
        if (isGrappling)
        {
            distanceJoint.enabled = true;
            distanceJoint.connectedAnchor = grappleTarget;
            distanceJoint.distance = Vector2.Distance(grapplingPoint.position, grappleTarget);
            distanceJoint.autoConfigureDistance = false;

            if (rb.angularVelocity < 100)

                rb.AddTorque(10);
            // Ensure the final position is set correctly
            lineRenderer.SetPosition(1, grappleTarget);
        }
        // Once the line has reached the target, enable the distance joint

    }

    void StopGrapple()
    {
        if (activePoint != null)
            activePoint.active = false;
        activePoint = null;

        isGrappling = false;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;


    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        StopGrapple();
    }
}
