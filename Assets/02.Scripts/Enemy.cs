using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody hipRigid;

    public Rigidbody[] rigidbodies;


    private int currentHP { get; set; }
    public int MaxHp = 1;

    [Space]

    public float flyingSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = MaxHp;   
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            
            print("111");
        }

        if (currentHP <= 0)
        {
            flyingSpeed = hipRigid.velocity.magnitude;
        }
    }

    public void TakeDamage(int damage, Vector3 player, int force)
    {
        currentHP -= damage;

        if (currentHP <= 0)
            Die(player, force);
    }

    public void Die(Vector3 player, int force)
    {
        animator.enabled = false;

        rigidbodies = GetComponentsInChildren<Rigidbody>();

        var dir = (hipRigid.GetComponent<BoxCollider>().bounds.center - player).normalized + (Vector3.up * 0.5f);

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].AddForce(dir * force);
        }

        //hipRigid.AddForce((transform.position - player.position).normalized * 10000);
    }


}
