using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventHandler : MonoBehaviour {
    Animator animator;
    public PlayerScript player;
    void Start()
    {
        animator = GetComponent<Animator>(); 
    }
    public void ShootingDone(){
        animator.SetBool("shoot", false);
    }

    public void DrawArrowStart(){
    }
    public void DeathSequence(){
        animator.SetBool("death", false);
    }
}
