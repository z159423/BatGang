using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableWall : MonoBehaviour
{
    [SerializeField] private Rigidbody[] partRigid;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponentInParent<Enemy>() != null)
        {
            if(collision.transform.GetComponentInParent<Enemy>().flyingSpeed > 4f)
            {
                GetComponent<Collider>().enabled = false;
                for(int i = 0; i < partRigid.Length; i++)
                {
                    
                    partRigid[i].isKinematic = false;
                }
            }

        }
    }
}
