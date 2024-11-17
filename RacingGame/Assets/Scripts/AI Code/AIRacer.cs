using System.Collections;
using UnityEngine;

public class AIRacer : MonoBehaviour
{
    // Waypoints
    public Transform[] waypoints;
    public int currentWaypointIndex = 0;

    // Speed and Movement
    public float maxSpeed = 10f;
    public float maxDraftingSpeed = 12f;
    public float turnSpeed = 3f;  // Slower turn speed
    public float turningAngleThreshold = 15f;
    public float currentSpeed = 0f;
    public float accelerationRate = 1.25f;
    public float decelerationRate = 16f;
    public float accelerationCurveFactor = 0.6f;

    // Drafting
    public LayerMask carLayer;
    public float draftingSpeedBoost = 2f;
    private float draftingBoost = 0f;
    public float draftingDuration = 3f;
    private bool isDraftingActive = false;
    private float draftingEndTime = 0f;
    public float draftingDelay = 3f;
    private float raceStartTime;

    // Maneuvering
    public float obstacleDetectionDistance = 5f;
    public float sideRayDistance = 3f;

    // Ground Alignment
    public Transform frontRayOrigin;
    public Transform backRayOrigin;
    public float raycastDistance = 2f;
    public LayerMask groundLayer;
    public float slopeAdjustmentSpeed = 5f;

    // Wheels and Steering
    public Transform wheelFL, wheelFR, wheelRL, wheelRR;
    public float wheelRotationSpeed = 10f;
    private float currentSteeringAngle = 0f;
    private float frontLeftWheelRotationX = 0f;
    private float frontRightWheelRotationX = 0f;
    private float rearLeftWheelRotationX = 0f;
    private float rearRightWheelRotationX = 0f;

    // Audio
    public AudioSource engineAudioSource;
    public float maxEnginePitch = 2f;
    public float maxEngineVolume = 1f;

    private Rigidbody rb;
    private bool hasReachedEnd = false;

    public string aiName = "AI Racer";
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = 0f;
        raceStartTime = Time.time;

        if (engineAudioSource != null)
        {
            engineAudioSource.loop = true;
            engineAudioSource.Play();
        }

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        float delay = Random.Range(0.1f, 0.75f);
        yield return new WaitForSeconds(delay);
        SetNextWaypoint();
    }

    void Update()
    {
        if (hasReachedEnd)
        {
            DecelerateToStop();
        }
        else
        {
            MoveTowardsWaypointWithOffset();
            AlignToGroundSlope();
            AdjustSpeedBasedOnAngle();
        }

        SteerFrontWheels();
        RotateWheels();
        AdjustEngineSound();
    }

    void SetNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        if (currentWaypointIndex < waypoints.Length - 1)
        {
            currentWaypointIndex++;
        }
        else
        {
            hasReachedEnd = true;  // Stop moving after reaching the last waypoint
        }
    }

    void MoveTowardsWaypointWithOffset()
    {
        Vector3 targetPos = waypoints[currentWaypointIndex].position;

        // Check for drafting or obstacle ahead
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, obstacleDetectionDistance, carLayer))
        {
            if (!isDraftingActive && Time.time - raceStartTime > draftingDelay && currentSpeed > maxSpeed * 0.75f)
            {
                ActivateDrafting();
            }
            AttemptOvertake(ref targetPos);
        }

        Vector3 directionToTarget = (targetPos - transform.position).normalized;
        Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Slower rotation speed during turns
        float dynamicTurnSpeed = Mathf.Lerp(turnSpeed, turnSpeed * 0.5f, Vector3.Angle(transform.forward, directionToTarget) / turningAngleThreshold);
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dynamicTurnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 10f)
        {
            SetNextWaypoint();
        }

        // Disable drafting if time expired
        if (isDraftingActive && Time.time > draftingEndTime)
        {
            DeactivateDrafting();
        }
    }

    void AttemptOvertake(ref Vector3 targetPos)
    {
        Vector3 leftRayOrigin = transform.position - transform.right * 0.5f;
        Vector3 rightRayOrigin = transform.position + transform.right * 0.5f;

        bool leftClear = !Physics.Raycast(leftRayOrigin, transform.forward, sideRayDistance, carLayer);
        bool rightClear = !Physics.Raycast(rightRayOrigin, transform.forward, sideRayDistance, carLayer);

        if (rightClear) targetPos += transform.right * 40f;
        else if (leftClear) targetPos += transform.right * -40f;
        else currentSpeed -= decelerationRate * Time.deltaTime;
    }

    void AlignToGroundSlope()
    {
        if (Physics.Raycast(frontRayOrigin.position, Vector3.down, out RaycastHit frontHit, raycastDistance, groundLayer) &&
            Physics.Raycast(backRayOrigin.position, Vector3.down, out RaycastHit backHit, raycastDistance, groundLayer))
        {
            Vector3 slopeDirection = (frontHit.point - backHit.point).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(slopeDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slopeAdjustmentSpeed * Time.deltaTime);
        }
    }

    void AdjustSpeedBasedOnAngle()
    {
        Vector3 directionToWaypoint = waypoints[currentWaypointIndex].position - transform.position;
        float angleToWaypoint = Vector3.Angle(transform.forward, directionToWaypoint);

        float baseTargetSpeed = maxSpeed + (isDraftingActive ? draftingBoost : 0f);
        float turnSpeedAdjustment = Mathf.Clamp01(1f - (angleToWaypoint / turningAngleThreshold));
        float targetSpeed = Mathf.Lerp(turnSpeed, baseTargetSpeed, turnSpeedAdjustment);

        float minimumTurnSpeed = 4f;
        targetSpeed = Mathf.Max(targetSpeed, minimumTurnSpeed);

        if (currentSpeed < targetSpeed)
        {
            currentSpeed += accelerationRate * Mathf.Pow((targetSpeed - currentSpeed), accelerationCurveFactor) * Time.deltaTime;
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= (decelerationRate * 0.5f) * Time.deltaTime;
        }

        float maxPossibleSpeed = isDraftingActive ? maxDraftingSpeed : maxSpeed;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxPossibleSpeed);
    }

    void DecelerateToStop()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Max(0f, currentSpeed);
            Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + moveDirection);
        }
    }

    void ActivateDrafting()
    {
        isDraftingActive = true;
        draftingBoost = draftingSpeedBoost;
        draftingEndTime = Time.time + draftingDuration;
    }

    void DeactivateDrafting()
    {
        isDraftingActive = false;
        draftingBoost = 0f;
    }

    void SteerFrontWheels()
    {
        Vector3 directionToWaypoint = waypoints[currentWaypointIndex].position - transform.position;
        float targetSteeringAngle = Vector3.SignedAngle(transform.forward, directionToWaypoint, Vector3.up);
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, Time.deltaTime * 15f);

        Quaternion steeringRotation = Quaternion.Euler(0f, currentSteeringAngle, 0f);
        wheelFL.localRotation = steeringRotation * Quaternion.Euler(frontLeftWheelRotationX, 0f, 0f);
        wheelFR.localRotation = steeringRotation * Quaternion.Euler(frontRightWheelRotationX, 0f, 0f);
    }

    void RotateWheels()
    {
        float wheelRotationAmount = currentSpeed * Time.deltaTime * wheelRotationSpeed;
        frontLeftWheelRotationX += wheelRotationAmount;
        frontRightWheelRotationX += wheelRotationAmount;
        rearLeftWheelRotationX += wheelRotationAmount;
        rearRightWheelRotationX += wheelRotationAmount;

        wheelRL.localRotation = Quaternion.Euler(rearLeftWheelRotationX, 0f, 0f);
        wheelRR.localRotation = Quaternion.Euler(rearRightWheelRotationX, 0f, 0f);
    }

    void AdjustEngineSound()
    {
        if (engineAudioSource == null) return;

        float maxPitch = isDraftingActive ? 1.5f : 1.25f;
        float currentMaxSpeed = isDraftingActive ? maxDraftingSpeed : maxSpeed;
        float normalizedSpeed = currentSpeed / currentMaxSpeed;

        engineAudioSource.pitch = Mathf.Lerp(0.75f, maxPitch, normalizedSpeed);
        engineAudioSource.volume = Mathf.Lerp(0.3f, maxEngineVolume, normalizedSpeed);
    }
}
