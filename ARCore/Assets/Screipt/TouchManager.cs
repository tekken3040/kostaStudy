using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class TouchManager : MonoBehaviour
{
    [SerializeField] GameObject placeObject;
    [SerializeField] Text measureLavel;
    [SerializeField] Camera ARCamera;
    private Anchor preAnchor;

    private void Update()
    {
        if(Input.touchCount==0)
            return;

        Touch touch = Input.GetTouch(0);
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
        if(touch.phase == TouchPhase.Began && Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            if(preAnchor==(null))
            {
                preAnchor = anchor;
            }
            float dist = Vector3.Distance(preAnchor.transform.position, anchor.transform.position);
            measureLavel.text = dist.ToString();
            preAnchor = anchor;
            float dist2 = Vector3.Distance(ARCamera.transform.position, anchor.transform.position);
            if(dist2 < 15f)
            {
                GameObject gameObject = Instantiate(placeObject, hit.Pose.position, Quaternion.identity, anchor.transform);
                var rot = Quaternion.LookRotation(ARCamera.transform.position - hit.Pose.position);
                gameObject.transform.rotation = Quaternion.Euler(ARCamera.transform.position.x, rot.eulerAngles.y, ARCamera.transform.position.z);
            }
        }
    }
}
