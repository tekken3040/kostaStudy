using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerAni : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform trans;

    public void OnRequestHit(Vector3 _pos)
    {
        trans.localPosition = _pos;
        animator.SetTrigger("hit");
        StartCoroutine(SetPosY());
    }

    private IEnumerator SetPosY()
    {
        yield return new WaitForSeconds(0.2f);
        trans.localPosition = new Vector3(trans.localPosition.x, 4, trans.localPosition.z);
    }
}
