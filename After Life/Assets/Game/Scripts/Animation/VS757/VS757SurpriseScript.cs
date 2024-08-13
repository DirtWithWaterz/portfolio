using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VS757SurpriseScript : StateMachineBehaviour
{

    public int ID;

    VS757 vs;
    GameObject[] l;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        l = GameObject.FindGameObjectsWithTag("Flamer");

        for(int i = 0; i < l.Length; i++){

            if(l[i].transform.GetChild(0).GetComponent<VS757>().ID == ID){

                vs = l[i].transform.GetChild(0).GetComponent<VS757>();
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vs.attack = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vs.surprised = false;
        vs.takingDamage = false;
        if(vs.HitSurprised)
            vs.Turn(0);
        vs.HitSurprised = false;
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
