using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geiger : MonoBehaviour
{
    bool lerpIn;
    bool lerpOut;
    AudioSource geiger;
    void Awake(){
        geiger = GameObject.Find("Geiger").GetComponent<AudioSource>();
        geiger.volume = 0;
        geiger.Play();
    }
    void Update(){
        float targetVol = -1f;
        if(lerpIn){
            targetVol = 0.2f;
            geiger.volume = Mathf.Lerp(geiger.volume, targetVol, Time.fixedDeltaTime);
        }
        if(lerpOut){
            targetVol = 0f;
            geiger.volume = Mathf.Lerp(geiger.volume, targetVol, Time.fixedDeltaTime);
        }
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Radioactive"){
            lerpIn = true;
            lerpOut = false;
        }
    }
    void OnTriggerExit(Collider other){
        if(other.gameObject.tag == "Radioactive"){
            lerpOut = true;
            lerpIn = false;
        }
    }
}
