using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Camcorder : MonoBehaviour
{

    [SerializeField] RawImage recNotif;
    [SerializeField] RawImage Batt01;
    [SerializeField] Slider Batt02;
    [SerializeField] VideoPlayer videoPlayer;
    
    bool BlinkOn = false;

    bool BattBlinkOn = false;

    void Start(){
#if !UNITY_EDITOR
        videoPlayer.enabled = true;
        videoPlayer.Play();
        videoPlayer.targetCameraAlpha = 1;
#endif
    }

    void Update(){
        if(!videoPlayer.isPlaying){
            StartCoroutine(WaitaSec());
        } else{
            videoPlayer.enabled = true;
            videoPlayer.targetCameraAlpha = 1;
        }
        if(!BlinkOn)
            StartCoroutine(RecordingBlinkOn());
        if(BlinkOn)
            StartCoroutine(RecordingBlinkOff());

        if(Batt02.value == 0)
        {
            if(!BattBlinkOn)
                StartCoroutine(BatteryBlinkOn());
            if(BattBlinkOn)
                StartCoroutine(BatteryBlinkOff());

        } else{Batt01.enabled = true;}
    }

    private IEnumerator WaitaSec()
    {
        yield return new WaitForSeconds(2);
        if(!videoPlayer.isPlaying){
            videoPlayer.enabled = false;
            videoPlayer.targetCameraAlpha = 0;
        }
        else{videoPlayer.targetCameraAlpha = 1;}
    }

    private IEnumerator BatteryBlinkOff()
    {
        yield return new WaitForSeconds(0.3f);
        Batt01.enabled = false;
        BattBlinkOn = false;
    }

    private IEnumerator BatteryBlinkOn()
    {
        yield return new WaitForSeconds(0.3f);
        Batt01.enabled = true;
        BattBlinkOn = true;
    }

    private IEnumerator RecordingBlinkOn()
    {
        yield return new WaitForSeconds(1);
        recNotif.enabled = true;
        BlinkOn = true;
    }

    private IEnumerator RecordingBlinkOff()
    {
        yield return new WaitForSeconds(1);
        recNotif.enabled = false;
        BlinkOn = false;
    }
}
