using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootIfGrabbed : MonoBehaviour
{
    private SimpleShoot simpleShoot;
    private OVRGrabbable oVRGrabbable;
    public OVRInput.Button shootingButton;
    public int maxNumberOfBullet = 10;

    private void Start()
    {
        simpleShoot = GetComponent<SimpleShoot>();
        oVRGrabbable = GetComponent<OVRGrabbable>();
    }

    private void Update()
    {
        if(oVRGrabbable.isGrabbed && OVRInput.GetDown(shootingButton, oVRGrabbable.grabbedBy.GetController()))
        {
            simpleShoot.ShootTrigger();
        }
    }
}
