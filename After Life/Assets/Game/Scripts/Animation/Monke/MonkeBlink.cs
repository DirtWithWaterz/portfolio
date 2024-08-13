using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeBlink : MonoBehaviour
{
    [SerializeField] float min;
    [SerializeField] float max;
    [SerializeField] float blinkTime;

    [SerializeField] Sprite opened;
    [SerializeField] Sprite closed;

    [SerializeField] SpriteRenderer sprite;

    bool blinking = false;

    void Update(){
        
        if(!blinking)
            StartCoroutine(Blink());
    }

    IEnumerator Blink(){

        blinking = true;
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(min, max));

        sprite.sprite = closed;
        yield return new WaitForSeconds(blinkTime);
        sprite.sprite = opened;

        blinking = false;
        
        yield return true;
    }
}
