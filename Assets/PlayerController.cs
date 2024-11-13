using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 25f;  // Movement speed
    private float runSpeed = 100f;  // Movement speed
    private float usedSpeed;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            usedSpeed = runSpeed;
        }
        else
        {
            usedSpeed = moveSpeed;
        }

        // Move forward when 'W' is pressed
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * usedSpeed * Time.deltaTime);
        }

        // Move backward when 'S' is pressed
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * usedSpeed * Time.deltaTime);
        }
        // Rotate around the Y-axis when 'A' is pressed (left)
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * usedSpeed * Time.deltaTime);
        }

        // Rotate around the Y-axis when 'D' is pressed (right)
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * usedSpeed * Time.deltaTime);
        }
    }
}