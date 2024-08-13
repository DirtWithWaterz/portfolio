using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BatteryInteraction : Interactable
{
    public TMP_Text pickUpText;
    private GameObject flashlight;

    public AudioSource pickUpSound;

    void Start()
    {
        pickUpText.enabled = false;
        flashlight = GameObject.Find("Flashlight");
    }

    public override void OnFocus(GameObject i)
    {
        if(i.tag == "Player"){
            if(!pickUpText.enabled){
                pickUpText.enabled = true;
                if(pickUpText.text == "")
                    pickUpText.text = "Pick Up\n" + "[" + i.GetComponent<FirstPersonController>().playerControls.interact.ToString() + "]";
            }
        }
    }

    public override void OnHoldInteract(GameObject i)
    {
        return;
    }

    public override void OnInteract(GameObject i)
    {
        if(i.tag == "Player"){
            flashlight.GetComponent<FlashlightAdvanced>().batteries += 1;
            pickUpSound.Play();
            HearingManager.Instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EPickUp_Small, 0.06f);
            pickUpText.text = "";
            pickUpText.enabled = false;
            Destroy(gameObject);
        }
    }

    public override void OnLoseFocus(GameObject i)
    {
        if(i.tag == "Player"){
            pickUpText.text = "";
            pickUpText.enabled = false;
        }
    }

    public override void OnReleaseInteract(GameObject i)
    {
        return;
    }
}
