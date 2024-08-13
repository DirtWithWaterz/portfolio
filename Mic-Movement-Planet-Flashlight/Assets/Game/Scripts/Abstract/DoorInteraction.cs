using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : Interactable
{
    Animator anim;
    [SerializeField] BoxCollider boxCollider1;
    [SerializeField] BoxCollider boxCollider2;
    [SerializeField] BoxCollider boxCollider3;
    [SerializeField] BoxCollider boxCollider4;
    void Start(){
        anim = this.GetComponent<Animator>();
        anim.SetBool("character_nearby", false);
    }
    public override void OnFocus(GameObject i)
    {
        
    }

    public override void OnHoldInteract(GameObject i)
    {
        
    }

    public override void OnInteract(GameObject i)
    {
        Debug.Log(i.name + " interacted with " + this.name);
        anim.SetBool("character_nearby", !anim.GetBool("character_nearby"));
        boxCollider1.enabled = !anim.GetBool("character_nearby");
        boxCollider2.enabled = !anim.GetBool("character_nearby");
        boxCollider3.enabled = !anim.GetBool("character_nearby");
        boxCollider4.enabled = !anim.GetBool("character_nearby");
    }

    public override void OnLoseFocus(GameObject i)
    {
        
    }

    public override void OnReleaseInteract(GameObject i)
    {
        
    }
}
