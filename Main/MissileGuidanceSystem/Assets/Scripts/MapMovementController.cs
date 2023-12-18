using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovementController : MonoBehaviour
{
    public GameObject mainPlate;
    
    private float leftShift;
    private float rightShift;
    private void FixedUpdate()
    {
        if (Input.mousePosition.x < 1280f)
        {
            rightShift = 0f;
            leftShift = 1280f - Input.mousePosition.x;
            if (Input.GetMouseButton(3))
            {
                if (mainPlate.transform.position.x > -5.3)
                    transform.Translate(Vector3.left * (Time.deltaTime * leftShift * 0.01f));
            }
        }
        else
        {
            leftShift = 0f;
            rightShift = Input.mousePosition.x - 1280f;
            if (Input.GetMouseButton(3))
            {
                if (mainPlate.transform.position.x < 5.3)
                    transform.Translate(Vector3.right * (Time.deltaTime * rightShift * 0.01f));
            }
        }
    }
}