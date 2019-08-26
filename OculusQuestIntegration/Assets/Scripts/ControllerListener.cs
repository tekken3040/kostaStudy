using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerListener : Singleton<ControllerListener>
{
    [HideInInspector] public bool isPrimaryTriggerTouch = false;
    [HideInInspector] public bool isPrimaryTriggerPress = false;
    [HideInInspector] public bool isPrimaryGripPress = false;
    [HideInInspector] public bool isPrimaryThumbstickTouch = false;
    [HideInInspector] public bool isPrimaryThumbstickPress = false;
    [HideInInspector] public bool isPrimaryOneTouch = false;
    [HideInInspector] public bool isPrimaryOnePress = false;
    [HideInInspector] public bool isPrimaryTwoTouch = false;
    [HideInInspector] public bool isPrimaryTwoPress = false;

    [HideInInspector] public bool isSecondaryTriggerTouch = false;
    [HideInInspector] public bool isSecondaryTriggerPress = false;
    [HideInInspector] public bool isSecondaryGripPress = false;
    [HideInInspector] public bool isSecondaryThumbstickTouch = false;
    [HideInInspector] public bool isSecondaryThumbstickPress = false;
    [HideInInspector] public bool isSecondaryOneTouch = false;
    [HideInInspector] public bool isSecondaryOnePress = false;
    [HideInInspector] public bool isSecondaryTwoTouch = false;
    [HideInInspector] public bool isSecondaryTwoPress = false;

    public void CheckPrimaryController()
    {
        isPrimaryTriggerTouch = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger);
        isPrimaryTriggerPress = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        isPrimaryGripPress = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        isPrimaryThumbstickTouch = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick);
        isPrimaryThumbstickPress = OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
        isPrimaryOneTouch = OVRInput.Get(OVRInput.Touch.Three);
        isPrimaryOnePress = OVRInput.Get(OVRInput.Button.Three);
        isPrimaryTwoTouch = OVRInput.Get(OVRInput.Touch.Four);
        isPrimaryTwoPress = OVRInput.Get(OVRInput.Button.Four);
    }

    public void CheckSecondryController()
    {
        isSecondaryTriggerTouch = OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger);
        isSecondaryTriggerPress = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        isSecondaryGripPress = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        isSecondaryThumbstickTouch = OVRInput.Get(OVRInput.Touch.SecondaryThumbstick);
        isSecondaryThumbstickPress = OVRInput.Get(OVRInput.Button.SecondaryThumbstick);
        isSecondaryOneTouch = OVRInput.Get(OVRInput.Touch.One);
        isSecondaryOnePress = OVRInput.Get(OVRInput.Button.One);
        isSecondaryTwoTouch = OVRInput.Get(OVRInput.Touch.Two);
        isSecondaryTwoPress = OVRInput.Get(OVRInput.Button.Two);
    }
}
