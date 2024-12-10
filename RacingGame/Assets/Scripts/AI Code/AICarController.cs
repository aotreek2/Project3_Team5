using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using static CarMoveScr;
using Unity.VisualScripting;

public class AICarController : MonoBehaviour
{
    public PathManager pathManager;
    public CinemachineSmoothPath path;
    public float maxSpeed = 20f;
    public float acceleration = 500f;
    public float steeringSensitivity = 0.5f;
    public float brakeForce = 1000f;
    public float waypointReachThreshold = 5f;
    public float turnSpeedModifier = 0.4f;
    public float maxSteerAngle = 35f;

    public float obstacleAvoidanceRange = 10f;
    public float obstacleAvoidanceSteerAngle = 15f;
    public LayerMask obstacleLayer;

    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    private WheelCollider[] wheels;
    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    public float currentSpeed;
    public string aiName = "AI Racer";

    private float stoppedTime = 0f;
    private Vector3 lastWaypointPosition;

    public AudioSource engineAudioSource;
    public float minPitch = 0.8f; // Pitch at zero speed
    public float maxPitch = 2.0f; // Pitch at max speed

    Coroutine gravRoutine;
    float gravMultiplier = 1;
    [SerializeField] float bonusGravity = 2;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1f, 0);
        currentSpeed = maxSpeed * Random.Range(0.2f, 1.0f);

        if(pathManager != null ) 
        {
            path = pathManager.AssignTrack();
        }
        else
        {
            Debug.LogError("RaceManager is not assigned to the car.");
            return;
        }

        if (path != null && path.m_Waypoints.Length > 0)
        {
            Vector3 startPosition = path.m_Waypoints[0].position;
            transform.position = new Vector3(startPosition.x, transform.position.y, startPosition.z);
            lastWaypointPosition = transform.position;
        }

        SetupWheelColliders();
    }

    private void Update()
    {
        FollowPath();
        UpdateWheelTransforms();
        CheckIfStopped();
        AdjustEngineSound();
    }

    private void FollowPath()
    {
        if (path == null || path.m_Waypoints.Length == 0) return;

        Vector3 targetPosition = path.m_Waypoints[currentWaypointIndex].position;
        Vector3 localTarget = transform.InverseTransformPoint(targetPosition);

        float turnAngle = Mathf.Abs(localTarget.x / localTarget.magnitude);
        float turnModifier = Mathf.Lerp(1f, turnSpeedModifier, turnAngle);
        currentSpeed = maxSpeed * turnModifier;

        ApplySteering(localTarget);
        ApplyMotor();

        if (turnAngle > 0.2f)
        {
            ApplyBraking();
        }
        else
        {
            ReleaseBraking();
        }

        if (Vector3.Distance(transform.position, targetPosition) < waypointReachThreshold)
        {
            lastWaypointPosition = transform.position;

            if (currentWaypointIndex == path.m_Waypoints.Length - 1)
            {
                currentSpeed = 0;
                ApplyBraking();
            }
            else
            {
                currentWaypointIndex++;
            }
        }
    }

    private void ApplySteering(Vector3 localTarget)
    {
        float steerAngle = Mathf.Clamp(localTarget.x / localTarget.magnitude * maxSteerAngle, -maxSteerAngle, maxSteerAngle);

        if (Physics.Raycast(transform.position, transform.forward, obstacleAvoidanceRange, obstacleLayer))
        {
            if (!Physics.Raycast(transform.position, transform.forward + transform.right * 0.5f, obstacleAvoidanceRange, obstacleLayer))
            {
                steerAngle += obstacleAvoidanceSteerAngle;
            }
            else if (!Physics.Raycast(transform.position, transform.forward - transform.right * 0.5f, obstacleAvoidanceRange, obstacleLayer))
            {
                steerAngle -= obstacleAvoidanceSteerAngle;
            }
        }

        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    private void ApplyMotor()
    {
        // Calculate a rational function for smooth acceleration
        float speedRatio = rb.velocity.magnitude / maxSpeed;
        float targetTorque = speedRatio < 1f ? acceleration * (1f - Mathf.Pow(speedRatio, 2)) : 0f;

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

    private void CheckIfStopped()
    {
        float speed = rb.velocity.magnitude;

        if (speed < 0.1f)
        {
            stoppedTime += Time.deltaTime;

            if (stoppedTime > 0.5f)
            {
                // Reset car to the last waypoint position
                transform.position = lastWaypointPosition;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                stoppedTime = 0f; // Reset stopped time counter
            }
        }
        else
        {
            stoppedTime = 0f; // Reset if car is moving
        }
    }

    void ApplyGravity()
    {
        int wheelsOnGround = 0;
        if(frontRightWheel.isGrounded)
        {
            wheelsOnGround++;
        }
        if (frontLeftWheel.isGrounded)
        {
            wheelsOnGround++;
        }
        if (rearRightWheel.isGrounded)
        {
            wheelsOnGround++;
        }
        if (rearLeftWheel.isGrounded)
        {
            wheelsOnGround++;
        }
        if (wheelsOnGround > 1) //if on ground, use normal gravity
        {
            if (gravRoutine != null)
            {
                StopCoroutine(gravRoutine);
                gravRoutine = null;
            }
            gravMultiplier = 1;
        }
        else if (gravRoutine == null)
        {
            gravRoutine = StartCoroutine(GravityLerp());
        }
        rb.velocity += new Vector3(0, -9.81f * Time.fixedDeltaTime * gravMultiplier, 0);
    }

    IEnumerator GravityLerp()
    {

        yield return new WaitForSeconds(0.5f);
        float elapsedTime = 0;
        float duration = 3;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            gravMultiplier = Mathf.Lerp(1, bonusGravity, t);
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
    }

    private void AdjustEngineSound()
    {
        if (engineAudioSource == null) return;

        // Calculate the pitch based on the car's current speed
        float speedRatio = rb.velocity.magnitude / maxSpeed;
        engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
    }
}


