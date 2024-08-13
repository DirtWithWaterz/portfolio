using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.GroundFitter;

public abstract class Interactable : MonoBehaviour
{
    protected PlayerControls controls;
    GameObject objectToFind;

    public virtual void Awake()
    {
        objectToFind = GameObject.FindGameObjectWithTag("MainCamera");
        controls = objectToFind.GetComponent<PlayerControls>();
        gameObject.layer = 9;
    }
    public abstract void OnHoldInteract(GameObject i);
    public abstract void OnInteract(GameObject i);
    public abstract void OnReleaseInteract(GameObject i);
    public abstract void OnFocus(GameObject i);
    public abstract void OnLoseFocus(GameObject i);
}
