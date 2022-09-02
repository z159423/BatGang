using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Collider bodyCollider;
    public bool isAttacking = false;

    public int batSwingForce = 1000;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && isAttacking == false)
        {
            isAttacking = true;
            animator.SetTrigger("Swing");
            print("Attack!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && isAttacking == false)
        {
            if(other.GetComponentInParent<Enemy>() != null)
            {
                StartCoroutine(Attack(other.GetComponentInParent<Enemy>()));
            }
        }
    }

    private IEnumerator Attack(Enemy enemy)
    {
        isAttacking = true;
        animator.SetTrigger("Swing");

        yield return null;

        enemy.TakeDamage(1, bodyCollider.bounds.center, batSwingForce);

        //Time.timeScale = 0.5f;
        TimeScaler.instance.ChangeTimeScale(0.5f);

        //Time.timeScale = 1f;

    }
}
