using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleScript : StateMachineBehaviour
{

    [SerializeField] PlayerCombat pc;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(CombatManager.instance.isLightAttacking && !pc.pM.crouch){

            CombatManager.instance.myAnim.Play("Atk1");
            pc.EnAtk1();
        }
        if(CombatManager.instance.isHeavyAttacking && !pc.pM.crouch && pc.cc2D.m_Grounded && !CombatManager.instance.myAnim.GetCurrentAnimatorStateInfo(0).IsName("Atk4") && !CombatManager.instance.myAnim.GetCurrentAnimatorStateInfo(0).IsName("Transition4") && !pc.animator.GetCurrentAnimatorStateInfo(0).IsName("SkyHT4")){

            CombatManager.instance.myAnim.Play("Atk3");
            pc.EnAtk3();
        }
        else if(CombatManager.instance.isHeavyAttacking && !pc.pM.crouch && !pc.cc2D.m_Grounded){

            pc.animator.Play("SkyHT4");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatManager.instance.isLightAttacking = false;
        CombatManager.instance.isHeavyAttacking = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
