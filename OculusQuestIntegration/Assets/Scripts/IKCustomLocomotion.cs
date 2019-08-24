using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class IKCustomLocomotion : MonoBehaviour
{
    [Tooltip("Offset of the root position relative to the HMD in root rotation space.")]
    public Vector3 rootOffset;

    [Tooltip("Increase this to start moving sooner and move faster.")]
    public float distanceMlp = 1f;

    [Tooltip("The amount of HMD velocity used in the calculations.")]
    public float velocityAdd = 1f;

    [Tooltip("The smoothing speed of HMD velocity")]
    public float velocitySmoothSpeed = 25f;

    [Tooltip("'Time' is the horizontal distance of the HMD from the root of the character. 'Value' is multiplier for the Animator's 'VRIK_Horizontal' and 'VRIK_Vertical' parameters.")]
    public AnimationCurve stepLengthByDistance;

    [Tooltip("If hands are held higher, increases start move thresholds by this value.")]
    [Range(0f, 1f)]
    public float increaseStartMoveThresholdsByHandHeight = 0.3f;

    private Animator animator;
    private VRIK ik;
    private Vector3 com;
    private float hYNormalized;
    private Vector3 dir;
    private float distance;
    private Vector3 lastCOM;
    private Vector3 velocitySmooth;
    private bool isMoving;
    private float height;

    void Awake()
    {
        animator = GetComponent<Animator>();
        ik = GetComponent<VRIK>();

        lastCOM = GetCOM();
        height = ik.references.head.position.y - ik.references.root.position.y;
    }

    void Update()
    {
        // Center of mass
        com = GetCOM();

        // Direction to COM
        dir = com - animator.transform.position;

        dir *= distanceMlp;

        // Get hand height relative to head height
        Vector3 handsCenter = Vector3.Lerp(ik.solver.leftArm.target.position, ik.solver.rightArm.target.position, 0.5f);
        float handsY = handsCenter.y;
        float headY = ik.solver.spine.headTarget.position.y;
        float armLength = height * 0.4f;
        float hY = Mathf.Clamp((handsY - headY) + armLength, 0f, armLength);
        hYNormalized = hY / armLength;

        // Velocity
        Vector3 velocity = (com - lastCOM) / Time.deltaTime;
        lastCOM = com;
        velocitySmooth = Vector3.Lerp(velocitySmooth, velocity, Time.deltaTime * velocitySmoothSpeed);

        float hYMlp = Mathf.Lerp(1f, (1f - hYNormalized), increaseStartMoveThresholdsByHandHeight);
        if(isMoving)
            hYMlp = 1f;
        dir += velocitySmooth * velocityAdd * hYMlp;

        // Convert direction to local space
        dir.y = 0f;
        Vector3 dirLocal = Quaternion.Inverse(transform.rotation) * dir;
        distance = dirLocal.magnitude;

        // Animator input
        float sLength = stepLengthByDistance.Evaluate(distance);
        animator.SetFloat("MovementX", dirLocal.x * sLength);
        animator.SetFloat("MovementZ", dirLocal.z * sLength);
    }

    private Vector3 GetCOM()
    {
        Vector3 com = ik.solver.spine.headTarget.position;

        com += transform.rotation * rootOffset;
        return com;
    }
}
