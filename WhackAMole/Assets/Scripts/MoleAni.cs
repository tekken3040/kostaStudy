using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleAni : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    public void Hide()
    {
        if(Manager.Instance.ActiveCnt == 0)
            return;
        Manager.Instance.ActiveCnt--;
    }

    public void ShowMole()
    {
        animator.SetBool("hit", false);
        animator.SetBool("show", true);
    }

    public void HitMole()
    {
        animator.SetBool("hit", true);
        animator.SetBool("show", false);
    }
}
