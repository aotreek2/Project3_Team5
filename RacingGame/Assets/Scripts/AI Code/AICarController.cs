using UnityEngine;
using Cinemachine;

public class AICarController : MonoBehaviour
{
    public CinemachineSmoothPath path;
    public float maxSpeed = 20f;
    public float acceleration = 500f;
    public float steeringSensitivity = 0.5f;
    public float brakeForce = 1000f;
    public float waypointReachThreshold = 5f;
    public float turnSpeedModifier = 0.4f;
    public float maxSteerAngle = 35f;

    public float obstacleAvoidanceRange = 10f; // Range for obstacle detection
    public float obstacleAvoidanceSteerAngle = 15f; // Extra steering for avoidance
    public LayerMask obstacleLayer; // Layer to specify obstacles

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
    public string aiName = "AI Racer";

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0); // Lower center of mass for stability
        currentSpeed = maxSpeed * Random.Range(0.2f, 1.0f);

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

        // Calculate turn angle and speed adjustment
        float turnAngle = Mathf.Abs(localTarget.x / localTarget.magnitude);
        float turnModifier = Mathf.Lerp(1f, turnSpeedModifier, turnAngle);
        currentSpeed = maxSpeed * turnModifier;

        ApplySteering(localTarget);
        ApplyMotor();

        // Apply braking force during sharp turns
        if (turnAngle > 0.2f)
        {
            ApplyBraking();
        }
        else
        {
            ReleaseBraking();
        }

        // Check if close enough to the waypoint to proceed
        if (Vector3.Distance(transform.position, targetPosition) < waypointReachThreshold)
        {
            // Stop at the final waypoint
            if (currentWaypointIndex == path.m_Waypoints.Length - 1)
            {
                currentSpeed = 0;
                ApplyBraking();
               // enabled = false; // Disable the script to prevent further updates
            }
            else
            {
                currentWaypointIndex++;
            }
        }
    }

    private void ApplySteering(Vector3 localTarget)
    {
        // Standard path-following steering
        float steerAngle = Mathf.Clamp(localTarget.x / localTarget.magnitude * maxSteerAngle, -maxSteerAngle, maxSteerAngle);

        // Check for obstacles in front
        if (Physics.Raycast(transform.position, transform.forward, obstacleAvoidanceRange, obstacleLayer))
        {
            // If obstacle detected, adjust steering
            if (!Physics.Raycast(transform.position, transform.forward + transform.right * 0.5f, obstacleAvoidanceRange, obstacleLayer))
            {
                // Obstacle on the left side, steer slightly right
                steerAngle += obstacleAvoidanceSteerAngle;
            }
            else if (!Physics.Raycast(transform.position, transform.forward - transform.right * 0.5f, obstacleAvoidanceRange, obstacleLayer))
            {
                // Obstacle on the right side, steer slightly left
                steerAngle -= obstacleAvoidanceSteerAngle;
            }
        }

        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    private void ApplyMotor()
    {
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
            asymptoteSlip = 0.8f,
            asymptoteValue = 0.5f,
            stiffness = 6f
        };

        WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = 0.2f,
            extremumValue = 1f,
            asymptoteSlip = 0.4f,
            asymptoteValue = 0.8f,
            stiffness = 3f
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
            wheel.suspensionDistance = 0.05f;
            wheel.mass = 20f;
        }
    }
}

