using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CarMoveScr;

public class CarMoveScr : MonoBehaviour
{

    [SerializeField]
    public enum Axel //so both wheels can be called together
    {
        Front,
        Rear
    }

    [Serializable] //setting up each wheel
    public struct Wheel
    {
        public GameObject wheelObj;
        public WheelCollider wheelCollider;
        public Axel axel;
    }
    [Serializable] //setting up each gear in the gearbox
    public struct Gears
    {
        public float gearTorque;
        public float shiftSpeed;
    }

    [SerializeField] List<Wheel> wheels; //the wheels
    [SerializeField] List<Gears> gearBox;

    //Car vars
    [SerializeField] float maxAcceleration = 30.0f;
    [SerializeField] float brakeAcceleration = 50.0f;
    [SerializeField] float maxSpeed = 100f;
    [SerializeField] float bonusGravity = 2;
    [SerializeField] AnimationCurve steerCurve;
    int currentGear = 1;
    float accelerationDamper = 1.5f;
    float gravMultiplier = 1;

    Coroutine steerRoutine;
    Coroutine gravRoutine;

    [SerializeField, Range(0,1f) ] float turnSensitivity = 1.0f;
    [SerializeField] float maxSteerAngle = 30.0f;

    [SerializeField] Vector3 _centerOfMass;
    [SerializeField] float speedOffset;

    float moveInput;
    float steerInput;
    float currentTurn;
    float dynamicSteerMax;
    float mph; //for the car's real speed, for use in UI.

    private Rigidbody carRb;

    public bool start = false;
    public AudioSource engineAudioSource;
    public float minPitch, maxPitch;

    //Car UI vars
    [SerializeField] GameObject mphNeedle;
    public string carName = "Player";
    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.ConfigureVehicleSubsteps(5, 12, 15);
            wheel.wheelCollider.motorTorque = 0;
        }
    }

    void Update()
    {
        if (currentTurn != steerInput)
        {
            if (steerRoutine != null) { StopCoroutine(steerRoutine); }
            steerRoutine = StartCoroutine(TurnTires());
        }
    }

    void FixedUpdate()
    {
        AdjustEngineSound();
        RotateWheelMesh();
        UpdateDamper();
        UpdateGear();
        ApplyGravity();
    }


    public void GetMove(Vector2 input)
    {
        moveInput = input.y; // acceleration & deceleration -1 to 1
        steerInput = input.x;// turning -1 to 1
       if(start)
       {
          Move();
       }
    }

    void Move()
    {
        foreach (Wheel wheel in wheels)
        {
            float wheelRPM = wheel.wheelCollider.rpm;
            float direction = Mathf.Clamp(wheelRPM, -1, 1);
            if (moveInput == 0)
            {
                if (mph > 1f)
                {
                    wheel.wheelCollider.motorTorque = -direction * 0.5f * (maxAcceleration - accelerationDamper); //changed from deltatime to fixed for physics
                }
                else
                {
                    wheel.wheelCollider.motorTorque = 0;
                    wheel.wheelCollider.rotationSpeed = 0;
                }
            }
            else
            {
                if (wheel.axel == Axel.Rear)
                {
                    wheel.wheelCollider.motorTorque = moveInput * 500 * (maxAcceleration - accelerationDamper) * Time.fixedDeltaTime; //changed from deltatime to fixed for physics
                }
            }
        }
        mph = Mathf.Round(carRb.velocity.magnitude * 2.237f * 10) / 10;
        if (mphNeedle != null)
        {
            mphNeedle.transform.rotation = Quaternion.Euler(0,0,-mph*1.375f + 110);
        }
    }

    void ApplyGravity()
    {
        int wheelsOnGround = 0;
        foreach (Wheel wheel in wheels)
        {
            if (wheel.wheelCollider.isGrounded)
            {
                wheelsOnGround++;
            }
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
        carRb.velocity += new Vector3(0, -9.81f * Time.fixedDeltaTime * gravMultiplier, 0);
    }

    void UpdateDamper() //used to modify multipliers as the car's speed changes
    {

        float currentSpeed = Mathf.Clamp(carRb.velocity.magnitude, 0, maxSpeed - 0.01f); //ensure rational functions dont break.

        //a float that goes from -0.8 upto to around 0.93 based on the car's speed compared to its max.
        float damper = (-1 / ((0.1f * currentSpeed) - maxSpeed * 0.1f)) - (1f / (0.2f * maxSpeed * 0.1f) + 0.0001f);

        //Acceleration Damper, to set max speed in a more realistic way
        accelerationDamper = maxAcceleration * damper;
        accelerationDamper = Mathf.Clamp(accelerationDamper, -500f, maxAcceleration);
        //Steering Damper, to reduce turning ability at higher speeds

        float steerDamper = steerCurve.Evaluate(mph/(maxSpeed * 2.237f));
        dynamicSteerMax = maxSteerAngle * steerDamper;
    }
    void UpdateGear()
    {
        if (mph > gearBox[currentGear].shiftSpeed && currentGear < gearBox.Count - 1)
        {
            currentGear++;
        }
        else if (currentGear > 1)
        {
            if (mph <= gearBox[currentGear - 1].shiftSpeed)
            {
                currentGear--;
            }
        }
    }
    IEnumerator TurnTires()
    {
        currentTurn = steerInput;
        float elapsedTime = 0;
        float duration = 3;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            foreach (Wheel wheel in wheels)
            {
                if (wheel.axel == Axel.Front) //only front wheels
                {
                    if (dynamicSteerMax < 1)
                    {
                        float steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                        wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, t);
                    }
                    else
                    {
                        float steerAngle = steerInput * turnSensitivity * (maxSteerAngle - dynamicSteerMax);
                        wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, t);
                    }
                }
            }
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
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
    public void ApplyBrake(bool braking)
    {
        if (braking)
        {
            foreach (Wheel wheel in wheels)
            {
                if (wheel.axel == Axel.Rear) //only front wheels
                {
                    wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.fixedDeltaTime; //changed from deltatime to fixed for physics
                }
            }
        }
        else
        {
            foreach (Wheel wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    void RotateWheelMesh()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelObj.transform.SetPositionAndRotation(pos, rot);
        }
    }

    public void BoundaryReset()
    {
        foreach(Wheel wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0f;
            wheel.wheelCollider.brakeTorque = 0f;
        }

        carRb.velocity = Vector3.zero;
    }

    private void AdjustEngineSound()
    {
        if (engineAudioSource == null) return;

        // Calculate the pitch based on the car's current speed
        float speedRatio = carRb.velocity.magnitude / maxSpeed;
        engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
    }
}
