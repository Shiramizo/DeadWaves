using DeadWaves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController_TurnHelper : StateMachineBehaviour<CharacterControl>
{
    public CharacterControl charControl;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override protected void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (charControl == null) charControl = Context;
    }

    override protected void OnInitialize(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Implement code that processes and affects root motion
        //Debug.Log("ZOMBIE MOVE!");
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
