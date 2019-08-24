using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGrab : OVRGrabbable
{
    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        this.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        this.GetComponent<Renderer>().material.color = Color.white;
    }
}
