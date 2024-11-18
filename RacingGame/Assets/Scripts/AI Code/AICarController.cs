using UnityEngine;
using Cinemachine;

public class AICarController : MonoBehaviour
{
    public CinemachineSmoothPath path;
    public float maxSpeed = 20f;
    public float acceleration = 500f;
    public float steeringSensitivity = 0.5f; // Further increased for sharper turns
    public float brakeForce = 1000f;
    public float waypointReachThreshold = 5f;
    public float turnSpeedModifier = 0.4f; // Slower speeds for tighter turns
    public float maxSteerAngle = 35f; // Slightly increased steering angle

    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    public float currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Lower center of mass for stability
        currentSpeed = maxSpeed * Random.Range(0.2f, 1.0f);

        // Adjust wheel collider settings
        SetupWheelColliders();
    }

    private void Update()
    {
        FollowPath();
        UpdateWheelTransforms();
    }

    private void FollowPath()
    {
        if (path == null || path.m_Waypoints.Length == 0) return;

        Vector3 targetPosition = path.m_Waypoints[currentWaypointIndex].position;
        Vector3 localTarget = transform.InverseTransformPoint(targetPosition);

        // Calculate turn angle
        float turnAngle = Mathf.Abs(localTarget.x / localTarget.magnitude);

        // Dynamically reduce speed based on the sharpness of the turn
        float turnModifier = Mathf.Lerp(1f, turnSpeedModifier, turnAngle);
        currentSpeed = maxSpeed * turnModifier;

        ApplySteering(localTarget);
        ApplyMotor();

        // Apply braking force during sharp turns
        if (turnAngle > 0.5f) // Threshold for sharp turn
        {
            ApplyBraking();
        }
        else
        {
            ReleaseBraking();
        }

        // Move to the next waypoint if close enough
        if (Vector3.Distance(transform.position, targetPosition) < waypointReachThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % path.m_Waypoints.Length;
        }
    }

    private void ApplySteering(Vector3 localTarget)
    {
        // Calculate steering angle and apply it
        float steerAngle = Mathf.Clamp(localTarget.x / localTarget.magnitude * maxSteerAngle, -maxSteerAngle, maxSteerAngle);
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    private void ApplyMotor()
    {
        // Adjust motor torque based on current speed
        float targetTorque = rb.velocity.magnitude < currentSpeed ? acceleration : 0;
        frontLeftWheel.motorTorque = targetTorque;
        frontRightWheel.motorTorque = targetTorque;
    }

    private void ApplyBraking()
    {
        rearLeftWheel.brakeTorque = brakeForce;
        rearRightWheel.brakeTorque = brakeForce;
    }

    private void ReleaseBraking()
    {
        rearLeftWheel.brakeTorque = 0;
        rearRightWheel.brakeTorque = 0;
    }

    private void UpdateWheelTransforms()
    {
        UpdateWheelTransform(frontLeftWheel, frontLeftTransform);
        UpdateWheelTransform(frontRightWheel, frontRightTransform);
        UpdateWheelTransform(rearLeftWheel, rearLeftTransform);
        UpdateWheelTransform(rearRightWheel, rearRightTransform);
    }

    private void UpdateWheelTransform(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
    private void SetupWheelColliders()
    {
        
        WheelFrictionCurve forwardFriction = new WheelFrictionCurve
        {
            extremumSlip = 12f,
            extremumValue = 15f,
            asymptoteSlip = 0.8f, // Reduced slightly for better grip
            asymptoteValue = 0.5f,
            stiffness = 6f
        };

        WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = 0.2f,
            extremumValue = 1f,
            asymptoteSlip = 0.6f, // Tighter grip for sharper turns
            asymptoteValue = 0.8f,
            stiffness = 6f
        }; 

        foreach (WheelCollider wheel in new[] { frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel })
        {
            wheel.forwardFriction = forwardFriction;
            wheel.sidewaysFriction = sidewaysFriction;

            JointSpring suspensionSpring = new JointSpring
            {
                spring = 30000f,
                damper = 4500f,
                targetPosition = 0.5f
            };
            wheel.suspensionSpring = suspensionSpring;
            wheel.suspensionDistance = 0.3f;
            wheel.mass = 20f;
        }
    }
}

