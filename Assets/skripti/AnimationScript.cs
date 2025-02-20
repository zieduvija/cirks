using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    public void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    public void RollDice()
    {
        animator.SetBool("isRolling", true);
    }

    public void StopRoll()
    {
        animator.SetBool("isRolling", false);
    }
}
