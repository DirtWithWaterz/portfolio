using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{

    [SerializeField] GameObject target;
    [SerializeField] float speed = 1;

    void Start()
    {
        
    }

    void Update()
    {
        OrbitAround();
    }

    private void OrbitAround()
    {
        transform.RotateAround(target.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
