using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    public bool crouch = false;
    public Animator a;
    public Animator vfxa;
    private Rigidbody2D rb;
    [SerializeField] public bool CeilingAboveHead = false;

    public bool m_Attacking = false;

    void Awake(){
        a = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        a.SetFloat("yVelocity", rb.velocity.y);
        a.SetFloat("xVelocity", rb.velocity.x);

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if(Input.GetButtonDown("Jump") && !crouch){
            jump = true;
        }

        if(m_Attacking || CombatManager.instance.isHeavyAttacking){

            a.SetBool("Jumping", false);
        } else{

            a.SetBool("Jumping", !controller.m_Grounded);
        }

        if(CeilingAboveHead == true){
            crouch = true;
        } else if(Input.GetButtonDown("Crouch") && controller.m_Grounded){
            crouch = true;
        } else if(crouch && !Input.GetButton("Crouch") && !CeilingAboveHead){
            crouch = false;
        }
        
        if(crouch && horizontalMove != 0){
            a.SetBool("CrouchMoving", true);
            a.SetBool("Crouching", false);
        } else{
            a.SetBool("CrouchMoving", false);
            a.SetBool("Crouching", crouch);
        }
        
        if(horizontalMove != 0f && !CeilingAboveHead && !crouch && !controller.m_wasCrouching && !m_Attacking && !AnimatingAttack()){
            a.SetBool("Running", true);
        } else{
            a.SetBool("Running", false);
        }
    }

    void FixedUpdate(){
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch,  jump);
        jump = false;
    }

    public bool AnimatingAttack(){

        if(

            a.GetCurrentAnimatorStateInfo(0).IsName("Player_Slash-01") ||
            a.GetCurrentAnimatorStateInfo(0).IsName("Player_Slash-02") ||
            a.GetCurrentAnimatorStateInfo(0).IsName("Player_Slash-03") ||
            vfxa.GetCurrentAnimatorStateInfo(0).IsName("Atk1") ||
            vfxa.GetCurrentAnimatorStateInfo(0).IsName("Transition1")

        ){

            return true;
        }
        else
            return false;
    }
}
