using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickerControl : MonoBehaviour
{
    public bool isFlickering = false;
    public float timeDelay;

    void Update()
    {
        if(!isFlickering){
            StartCoroutine(FlickeringLight());
        }
    }

    IEnumerator FlickeringLight(){
        isFlickering = true;
        this.gameObject.GetComponent<Light2D>().enabled = false;
        timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light2D>().enabled = true;
        timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;
    }
}
