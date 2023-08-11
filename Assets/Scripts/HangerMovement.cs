using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerMovement : MonoBehaviour
{

    public float moveForce = 10f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 moveDirection = new Vector2(horizontalInput, 0f).normalized;

        rb.AddForce(moveDirection * moveForce * Time.deltaTime);
    }
}
