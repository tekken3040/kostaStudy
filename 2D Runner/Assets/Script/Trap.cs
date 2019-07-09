using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] RectTransform transform;
    [SerializeField] float fSpeed = -5f;

    bool bPlay = true;

    void FixedUpdate()
    {
        if (!bPlay)
            return;
        transform.localPosition += new Vector3(Time.deltaTime * fSpeed, 0);
        if (transform.localPosition.x < -400)
            this.gameObject.SetActive(false);
    }
}
