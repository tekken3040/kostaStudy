using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator anim;
    [SerializeField] float speed = 3f;
    [SerializeField] float jumpSpeed = 4f;

    Vector3 direction = Vector3.zero;
    float fJump = 0;

    void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = this.transform.TransformDirection(direction);
    }

    void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
            //direction.y = -Physics.gravity.y * Time.deltaTime;
            if (Input.GetKey(KeyCode.Space))
            {
                fJump = jumpSpeed;
            }
        }
        fJump -= Physics.gravity.y * Time.deltaTime;
        direction.y = fJump;
        characterController.Move(direction * speed * Time.deltaTime);
        
        SetMoveAnim(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void SetMoveAnim(float _x, float _y)
    {
        if (!characterController.velocity.normalized.x.Equals(0) || !characterController.velocity.normalized.z.Equals(0))
        {
            anim.SetBool("Idle", false);
            anim.SetFloat("Blend", 1);
        }
        else
        {
            anim.SetFloat("Blend", 0);
            anim.SetBool("Idle", true);
        }

        anim.SetFloat("MoveX", _x);
        anim.SetFloat("MoveY", _y);
    }
}
