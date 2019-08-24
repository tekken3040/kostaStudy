using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAvatar : MonoBehaviour
{
    [SerializeField] Transform _head;
    [SerializeField] PhotonView myView;
    [SerializeField] Animator animator;

    [SerializeField] Transform HeadTarget;
    [SerializeField] Transform LeftHandTarget;
    [SerializeField] Transform RightHandTarget;

    public Transform getHeadTarget { get { return HeadTarget; } }
    public Transform getLeftHandTarget { get { return LeftHandTarget; } }
    public Transform getRightHandTarget { get { return RightHandTarget; } }

    private void Start()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.UserId);
        if(myView.IsMine)
        {
            _head.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        if(!OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && !OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.RTouch) &&
           !OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.RTouch) && !OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.RTouch))
        {
            animator.SetInteger("HandGestureRight", 1);
        }
        else if(OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.RTouch) || OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.RTouch) ||
            OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.RTouch) && !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {

        }
        else
        {
            animator.SetInteger("HandGestureRight", 0);
        }
    }
}
