using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovementController : MonoBehaviour
{
    public GameObject drone;
    private float testValue = 0;
    public Rigidbody _rb;
    public float speed = 30f;
    void FixedUpdate()
    {
        Vector3 rotate = transform.eulerAngles;
        testValue += 0.004f;
        rotate.z = 20*(float)Math.Sin(testValue);
        transform.rotation = Quaternion.Euler(rotate);
        _rb.velocity = new Vector3(speed, 0f, 0f);
    }
}
