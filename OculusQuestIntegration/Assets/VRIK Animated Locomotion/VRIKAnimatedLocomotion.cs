using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class VRIKAnimatedLocomotion : MonoBehaviour {

	[Tooltip("Offset of the root position relative to the HMD in root rotation space.")] 
	public Vector3 rootOffset;

	[Tooltip("Increase this to start moving sooner and move faster.")] 
	public float distanceMlp = 1f;

	[Tooltip("The amount of HMD velocity used in the calculations.")] 
	public float velocityAdd = 1f;

	[Tooltip("The smoothing speed of HMD velocity")] 
	public float velocitySmoothSpeed = 25f;

	[Tooltip("'Time' is the horizontal distance of the HMD from the root of the character, 'Value' is Animator.speed.")] 
	public AnimationCurve animatorSpeedByDistance;

	[Tooltip("Animator.speed multiplier for walking backwards.")] 
	public float walkBwdSpeedMlp = 1f;

	[Tooltip("'Time' is the horizontal distance of the HMD from the root of the character. 'Value' is multiplier for the Animator's 'VRIK_Horizontal' and 'VRIK_Vertical' parameters.")] 
	public AnimationCurve stepLengthByDistance;

	[Tooltip("Starts moving if horizontal distance of the HMD from the root of the character exceeds this value.")] 
	public float startMoveDistanceThreshold = 0.3f;

	[Tooltip("Starts moving if the horizontal velocity magnitude of the HMD exceeds this value.")] 
	public float startMoveVelocityThreshold = 0.3f;

	[Tooltip("If hands are held higher, increases start move thresholds by this value.")][Range(0f, 1f)] 
	public float increaseStartMoveThresholdsByHandHeight = 0.3f;

	[Tooltip("Keeps the root of the character within this horizontal distance from the HMD.")] 
	public float maxDistanceFromHMD = 0.5f;

	private Animator animator;
	private VRIK ik;
	private Rigidbody r;
	private float stopMoveDistanceBuffer = 0.9f;
	private float stopMoveVelocityBuffer = 0.5f;
	private Vector3 com;
	private float hYNormalized;
	private Vector3 dir;
	private float distance;
	private Vector3 lastCOM;
	private Vector3 velocitySmooth;
	private bool isMoving;
	private float height;
	float acceleration = 0f;
	private float isMovingWeight, isMovingWeightV;
	private float turn;

	void Awake() {
		animator = GetComponent<Animator>();
		ik = GetComponent<VRIK>();

		lastCOM = GetCOM();
		height = ik.references.head.position.y - ik.references.root.position.y;
	}

	public void Reset() {
		velocitySmooth = Vector3.zero;
		isMoving = false;
		animator.SetBool("VRIK_IsMoving", isMoving);
		acceleration = 0;
		com = ik.references.head.position;
		lastCOM = com;
	}


	void Update() {
		AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

		// Center of mass
		com = GetCOM();

		transform.position = Vector3.Lerp(transform.position, new Vector3(com.x, transform.position.y, com.z), Time.deltaTime * Mathf.Max(acceleration, isMovingWeight));

		// Direction to COM
		dir = com - animator.transform.position;

		// Max distance from COM
		Vector3 dirHor = dir;
		dirHor.y = 0f;
		float dirHorMag = dirHor.magnitude;
		if (dirHorMag > 0f && dirHorMag > maxDistanceFromHMD)
            transform.position += (dirHor / dirHorMag) * (dirHorMag - maxDistanceFromHMD);

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
		if (isMoving)
            hYMlp = 1f;
		dir += velocitySmooth * velocityAdd * hYMlp;

		// Convert direction to local space
		dir.y = 0f;
		Vector3 dirLocal = Quaternion.Inverse(transform.rotation) * dir;
		distance = dirLocal.magnitude;

		// Start with left or right leg
		Vector3 dirLocalNormalized = dirLocal.normalized;
		float rDot = Vector3.Dot(dirLocalNormalized, Vector3.right);
		float fDot = Vector3.Dot(dirLocalNormalized, Vector3.forward);

		animator.SetBool("VRIK_Start R", (fDot > -0.8f? rDot > 0f: rDot < 0f));

		// Start and stop moving
		Vector3 velocitySmoothHor = new Vector3(velocitySmooth.x, 0f, velocitySmooth.z);
		float velMag = velocitySmoothHor.magnitude;

		float distanceS = startMoveDistanceThreshold;
		float velocityS = startMoveVelocityThreshold;

		if (isMoving)
        {
			distanceS *= stopMoveDistanceBuffer;
			velocityS *= stopMoveVelocityBuffer;
		}

		// Increase start move thresholds if hands are higher
		float hAdd = hY * height * increaseStartMoveThresholdsByHandHeight;
		distanceS += hAdd;
		velocityS += hAdd;

		if (isMoving)
        {
			// Stop moving
			if (distance < distanceS && velMag < velocityS)
                isMoving = false;
		}
        else
        {
			// Start moving
			if (distance >= distanceS || velMag >= velocityS)
                isMoving = true;
		}

		float isMovingWeightTarget = isMoving? 1f: 0f;
		isMovingWeight = Mathf.SmoothDamp(isMovingWeight, isMovingWeightTarget, ref isMovingWeightV, 0.2f);

		// Lerp position to COM when stopping
		bool isStopping = nextInfo.IsName("VRIK_Idle");
		float accTarget = isStopping? 1f: 0f;
		acceleration = Mathf.MoveTowards(acceleration, accTarget, Time.deltaTime * 7f);

		animator.SetBool("VRIK_IsMoving", isMoving);

		// Turning
		Vector3 headForward = ik.solver.spine.headTarget.forward;
		headForward.y = 0f;
		Vector3 headForwardLocal = Quaternion.Inverse(transform.rotation) * headForward;
		float angle = Mathf.Atan2(headForwardLocal.x, headForwardLocal.z) * Mathf.Rad2Deg;
		float turnTarget = angle / 180f;
		if (turnTarget < 0.2f)
            turnTarget = 0f;
		turn = Mathf.Lerp(turn, turnTarget, Time.deltaTime * 3f);
		animator.SetFloat("VRIK_Turn", turn * 2f);

		// Animator speed
		float bDot = Mathf.Max(-fDot, 0f);
		animator.speed = animatorSpeedByDistance.Evaluate(distance) * (1f + bDot * walkBwdSpeedMlp);

		// Animator input
		float sLength = stepLengthByDistance.Evaluate(distance);
		animator.SetFloat("VRIK_Horizontal", dirLocal.x * sLength);
		animator.SetFloat("VRIK_Vertical", dirLocal.z * sLength);
	}

	private Vector3 GetCOM() {
		Vector3 com = ik.solver.spine.headTarget.position;

		com += transform.rotation * rootOffset;
		return com;
	}
}
