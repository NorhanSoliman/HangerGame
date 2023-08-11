using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveBlanket : MonoBehaviour
{

    public float targetYPosition; // The desired Y position for the blanket to move up
    public float movementSpeed = 2f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        targetYPosition = 10.7f;

        // using lerp
        Vector3 newPosition = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetYPosition, Time.deltaTime * movementSpeed), transform.position.z);

        transform.position = newPosition;
    }

}
