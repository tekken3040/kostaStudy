using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TokoControl : MonoBehaviour
{
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip snd_Jump;
    [SerializeField] AudioClip snd_Die;

    [SerializeField] float fSpeed = 5f;
    [SerializeField] float fJumpPower = 5;

    [SerializeField] Animator animator;

    CharacterController characterController;

    float fJump = 0f;
    bool bGrounded = true;
    bool bDead = true;

    enum INPUT_BUTTON
    {
        NONE = 0,
        RIGHT = 1,
        LEFT,
    }
    
    void Awake()
    {
        characterController = this.GetComponent<CharacterController>();
    }

    public bool GetDead()
    {
        return bDead;
    }

    public void Init()
    {
        audio.clip = snd_Jump;
        animator.SetBool("Airial", false);
        animator.SetBool("Fall", false);
        animator.SetBool("Ground", true);
        bDead = false;
    }
    /*
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.name.Equals("Dead"))
        {
            audio.clip = snd_Die;
            animator.SetTrigger("Die");
            hit.collider.isTrigger = true;
            bDead = true;
            Debug.Log("Dead trigger hit");
        }
    }
    */
    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag.Equals("Dead"))
        {
            audio.clip = snd_Die;
            animator.SetTrigger("Die");
            bDead = true;
            Debug.Log("Dead trigger hit");
        }
    }

    void FixedUpdate()
    {
        characterController.Move(Vector3.up * fJump);
        fJump += -20 * Time.deltaTime;
        if (fJump <= 0)
        {
            animator.SetBool("Airial", false);
            animator.SetBool("Fall", true);
        }
        if (characterController.isGrounded)
        {
            bGrounded = true;
            animator.SetBool("Fall", false);
            animator.SetBool("Ground", true);
        }

        if (bDead)
            return;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //characterController.Move(Vector3.right * fSpeed * Time.deltaTime);
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //characterController.Move(Vector3.left * fSpeed * Time.deltaTime);
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (bGrounded)
            {
                bGrounded = false;
                fJump = fJumpPower;
                animator.SetBool("Ground", false);
                animator.SetBool("Airial", true);
            }
        }
    }
}
