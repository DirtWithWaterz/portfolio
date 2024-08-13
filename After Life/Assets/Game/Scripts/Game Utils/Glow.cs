using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Glow : MonoBehaviour
{
     public float minIntensity = 0.25f;
     public float maxIntensity = 0.5f;
     Light2D light2D;
 
     float random;
 
     void Start()
     {
        light2D = this.GetComponent<Light2D>();
         random = Random.Range(0.0f, 65535.0f);
     }
 
     void Update()
     {
         float noise = Mathf.PerlinNoise(random, Time.time);
         light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
     }
 }
