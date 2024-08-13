using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{

    public Animator myAnim;
    public bool isLightAttacking = false;
    public bool isHeavyAttacking = false;
    public static CombatManager instance;
    
    void Awake(){

        instance = this;
    }
    
    // Start is called before the first frame update
    void Start(){
        
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update(){
        
        Attack();
    }

    void Attack(){
        
        if(Mouse.current.leftButton.wasPressedThisFrame && !isLightAttacking && !isHeavyAttacking)
            isLightAttacking = true;
        
        if(Mouse.current.rightButton.wasPressedThisFrame && !isHeavyAttacking && !isLightAttacking)
            isHeavyAttacking = true;
    }
}
