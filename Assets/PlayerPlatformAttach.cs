using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformAttach : MonoBehaviour
{
    ThirdPersonMovement movement;

    private void Start()
    {
        movement = GetComponentInParent<ThirdPersonMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Platform"))
        {
            Debug.Log("TOUCHED PLATFORM");
            movement.gravity = 0;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Platform"))
        {
            Debug.Log("OFF PLATFORM");
            movement.gravity = -9f;
        }
    }
}
