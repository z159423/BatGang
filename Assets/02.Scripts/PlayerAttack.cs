using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool isAttacking = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && isAttacking == false)
        {
            isAttacking = true;
            animator.SetTrigger("Swing");

            print("Attack!");
        }
    }
}
