using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    public float speed = 150;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    ThirdPersonMovement player;
    BezierFollow bezierFollow;
    bool touchedBottom;
    private void Start()
    {
        oldPosition = transform.position;
        bezierFollow = GetComponent<BezierFollow>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            player = collision.transform.GetComponent<ThirdPersonMovement>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (player)
            {
                player.externalForce = Vector3.zero;
                player = null;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            touchedBottom = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("RELEASE");
           if(player)
            {
                player.externalForce = Vector3.zero;
            }
            touchedBottom = false;
            player = null;
        }
    }

    private void FixedUpdate()
    {
        newPosition =  (transform.position - oldPosition) * speed;
        oldPosition = transform.position;
        if (player)
        {
            player.externalForce = newPosition;
        }
    }
}
