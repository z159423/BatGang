using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private FixedTouchField touchField;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Animator animator;

    [Space]
    private Vector3 moveDir;
    private float angle;
    public float moveSpeed;
    

    // Update is called once per frame
    void Update()
    {
        moveDir = touchField.joystickDir.normalized * touchField.distBetweenJoystickBodyToHandle;

        float nomalizeMoveSpeed = touchField.distBetweenJoystickBodyToHandle;

        // print(touchField.joystickDir + " " + nomalizeMoveSpeed);

        transform.Translate(new Vector3(moveDir.x, 0, moveDir.y) * moveSpeed * Time.deltaTime, Space.World);

        if(nomalizeMoveSpeed == 0)
        {
            animator.SetBool("Moving", false);
        }
        else
        {
            animator.SetBool("Moving", true);
            animator.SetFloat("MoveAnimationSpeed", nomalizeMoveSpeed);
        }

        // if(nomalizeMoveSpeed == 0)
        // {
        //     animator.SetBool("Walk", false);
        //     animator.SetBool("Run", false);
        // }
        // else if(nomalizeMoveSpeed < .8f)
        // {
        //     animator.SetBool("Walk", true);
        //     animator.SetBool("Run", false);
        // }
        // else if(nomalizeMoveSpeed > 0.8f)
        // {
        //     animator.SetBool("Walk", false);
        //     animator.SetBool("Run", true);
        // }
        // else
        // {
        //     animator.SetBool("Walk", false);
        //     animator.SetBool("Run", false);
        // }

        

        if (Mathf.Abs(touchField.joystickDir.normalized.x) > 0 || Mathf.Abs(touchField.joystickDir.normalized.y) > 0)
        {
            angle = Mathf.Atan2((touchField.joystickDir.normalized.y + transform.position.y) - transform.position.y,
            (touchField.joystickDir.normalized.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

            //isMoving = true;
        }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.down), 10 * Time.deltaTime);

    }

    private void FixedUpdate() {
        
    }
}