using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCamera : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float offsetDistance;
    [SerializeField] float offsetHeight;
    [SerializeField] float smoothing;
    [SerializeField] Vector3 lookAt;

    Vector3 offset;

    void Start()
    {
        offset = new Vector3(target.transform.position.x, target.transform.position.y + offsetHeight, target.transform.position.z - offsetDistance);
    }

    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(0, Vector3.up) * offset;
        this.transform.position = target.transform.position + offset;
        this.transform.LookAt(target.transform.position + lookAt);
    }
}
