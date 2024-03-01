using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) return;
        isGrounded = true;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        isGrounded = false;
    }
}
