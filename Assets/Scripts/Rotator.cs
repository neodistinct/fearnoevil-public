using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private bool isActive = true;
    [SerializeField]
    private int rotationSpeed = 32;

    // Update is called once per frame
    void Update()
    {
        if (isActive) { 
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}
