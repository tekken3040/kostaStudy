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

    private int _animStateRight = 0;
    private int _animStateLeft = 0;

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
        ControllerListener.Instance.CheckPrimaryController();
        ControllerListener.Instance.CheckSecondryController();
        GetRightHandAniState();
        GetLeftHandAniState();
        animator.SetInteger("HandGestureRight", _animStateRight);
        animator.SetInteger("HandGestureLeft", _animStateLeft);
    }

    private int GetLeftHandAniState()
    {
        //int _state = 0;

        if(!ControllerListener.Instance.isPrimaryGripPress && !ControllerListener.Instance.isPrimaryTriggerTouch && !ControllerListener.Instance.isPrimaryThumbstickTouch && 
            !ControllerListener.Instance.isPrimaryOneTouch && !ControllerListener.Instance.isPrimaryTwoTouch)
            _animStateLeft = 1;
        else if(!ControllerListener.Instance.isPrimaryTriggerTouch && ControllerListener.Instance.isPrimaryGripPress && (ControllerListener.Instance.isPrimaryThumbstickTouch || 
            ControllerListener.Instance.isPrimaryOneTouch || ControllerListener.Instance.isPrimaryTwoTouch))
            _animStateLeft = 2;
        else if((ControllerListener.Instance.isPrimaryTwoTouch || ControllerListener.Instance.isPrimaryOneTouch || ControllerListener.Instance.isPrimaryThumbstickTouch) && 
            !ControllerListener.Instance.isPrimaryTriggerTouch && !ControllerListener.Instance.isPrimaryGripPress)
            _animStateLeft = 3;
        else if(ControllerListener.Instance.isPrimaryTriggerPress && !ControllerListener.Instance.isPrimaryGripPress && (ControllerListener.Instance.isPrimaryTwoTouch || 
            ControllerListener.Instance.isPrimaryOneTouch || ControllerListener.Instance.isPrimaryThumbstickTouch))
            _animStateLeft = 4;
        else if(ControllerListener.Instance.isPrimaryGripPress && !ControllerListener.Instance.isPrimaryTriggerTouch && !ControllerListener.Instance.isPrimaryThumbstickTouch && 
            !ControllerListener.Instance.isPrimaryOneTouch && !ControllerListener.Instance.isPrimaryTwoTouch)
            _animStateLeft = 5;
        else if(ControllerListener.Instance.isPrimaryGripPress && ControllerListener.Instance.isPrimaryTriggerPress && !ControllerListener.Instance.isPrimaryThumbstickTouch && 
            !ControllerListener.Instance.isPrimaryOneTouch && !ControllerListener.Instance.isPrimaryTwoTouch)
            _animStateLeft = 6;
        else
            _animStateLeft = 0;

        return _animStateLeft;
    }

    private int GetRightHandAniState()
    {
        //int _state = 0;

        if(!ControllerListener.Instance.isSecondaryGripPress && !ControllerListener.Instance.isSecondaryTriggerTouch && !ControllerListener.Instance.isSecondaryThumbstickTouch &&
            !ControllerListener.Instance.isSecondaryOneTouch && !ControllerListener.Instance.isSecondaryTwoTouch)
            _animStateRight = 1;
        else if(!ControllerListener.Instance.isSecondaryTriggerTouch && ControllerListener.Instance.isSecondaryGripPress && (ControllerListener.Instance.isSecondaryThumbstickTouch ||
            ControllerListener.Instance.isSecondaryOneTouch || ControllerListener.Instance.isSecondaryTwoTouch))
            _animStateRight = 2;
        else if((ControllerListener.Instance.isSecondaryTwoTouch || ControllerListener.Instance.isSecondaryOneTouch || ControllerListener.Instance.isSecondaryThumbstickTouch) &&
            !ControllerListener.Instance.isSecondaryTriggerTouch && !ControllerListener.Instance.isSecondaryGripPress)
            _animStateRight = 3;
        else if(ControllerListener.Instance.isSecondaryTriggerPress && !ControllerListener.Instance.isSecondaryGripPress && (ControllerListener.Instance.isSecondaryTwoTouch ||
            ControllerListener.Instance.isSecondaryOneTouch || ControllerListener.Instance.isSecondaryThumbstickTouch))
            _animStateRight = 4;
        else if(ControllerListener.Instance.isSecondaryGripPress && !ControllerListener.Instance.isSecondaryTriggerTouch && !ControllerListener.Instance.isSecondaryThumbstickTouch &&
            !ControllerListener.Instance.isSecondaryOneTouch && !ControllerListener.Instance.isSecondaryTwoTouch)
            _animStateRight = 5;
        else if(ControllerListener.Instance.isSecondaryGripPress && ControllerListener.Instance.isSecondaryTriggerPress && !ControllerListener.Instance.isSecondaryThumbstickTouch &&
            !ControllerListener.Instance.isSecondaryOneTouch && !ControllerListener.Instance.isSecondaryTwoTouch)
            _animStateRight = 6;
        else
            _animStateRight = 0;

        return _animStateRight;
    }
}
