using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition1Script : StateMachineBehaviour
{

    [SerializeField] PlayerCombat pc;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
        pc.ExAtk1();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(CombatManager.instance.isLightAttacking && !pc.pM.crouch){

            if(!pc.cc2D.m_Grounded)
                pc.cc2D.DoubleJump(pc.cc2D.m_JumpForce);
            CombatManager.instance.myAnim.Play("Atk2");
            pc.EnAtk2();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatManager.instance.isLightAttacking = false;
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
