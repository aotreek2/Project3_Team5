using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] List<Wheel> wheels; //the wheels

    //physics vars
    [SerializeField] float maxAcceleration = 30.0f;
    [SerializeField] float brakeAcceleration = 50.0f;
    [SerializeField] float maxSpeed = 100f;
    float accelerationDamper = 1.5f;

    Coroutine steerRoutine;

    [SerializeField, Range(0,1f) ] float turnSensitivity = 1.0f;
    [SerializeField] float maxSteerAngle = 30.0f;

    [SerializeField] Vector3 _centerOfMass;

    float moveInput;
    float steerInput;
    float currentTurn;
    float mph; //for the car's real speed, for use in UI.

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        foreach (Wheel wheel in wheels)
        {
            wheel.wheelCollider.ConfigureVehicleSubsteps(5, 12, 15);
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //Move();
        RotateWheelMesh();
        UpdateDamper();
        if(currentTurn != steerInput)
        {
            if (steerRoutine != null) { StopCoroutine(steerRoutine); }
            steerRoutine = StartCoroutine(TurnTires());
        }
    }


    public void GetMove(Vector2 input)
    {
        moveInput = input.y; // acceleration & deceleration -1 to 1
        steerInput = input.x;// turning -1 to 1
        Move();
    }

    void Move()
    {
        foreach (Wheel wheel in wheels)
        {
            if (accelerationDamper < 1)
            {
                wheel.wheelCollider.motorTorque = moveInput * 500 * maxAcceleration * Time.deltaTime;
            }
            else
            {
                wheel.wheelCollider.motorTorque = moveInput * 500 * (maxAcceleration - accelerationDamper) * Time.deltaTime;
            }
        }
        mph = Mathf.Round(carRb.velocity.magnitude * 2.237f * 10) / 10;
        //Debug.Log(mph);
    }
    void UpdateDamper() //used to modify multipliers as the car's speed changes
    {
        //Debug.Log(carRb.velocity.magnitude);

        float currentSpeed = Mathf.Clamp(carRb.velocity.magnitude, 0, maxSpeed - 0.01f); //ensure rational functions dont break.

        //a float that goes from -0.8 upto to around 0.93 based on the car's speed compared to its max.
        float damper = (-1 / ((0.1f * currentSpeed) - maxSpeed * 0.1f)) - (1f / (0.2f * maxSpeed * 0.1f));

        //Acceleration Damper, to set max speed in a more realistic way
        accelerationDamper = maxAcceleration * damper;
        accelerationDamper = Mathf.Clamp(accelerationDamper, -500f, maxAcceleration);

        //Steering Damper, to reduce turning ability at higher speeds


    }

    void Steer()// turning the wheels
    {
        foreach (Wheel wheel in wheels)
        {
            if (wheel.axel == Axel.Front) //only front wheels
            {
                float steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 5.6f);
            }
        }
    }
    IEnumerator TurnTires()
    {
        currentTurn = steerInput;
        float elapsedTime = 0;
        while (elapsedTime < 40)
        {
            float t = elapsedTime / 40;
            foreach (Wheel wheel in wheels)
            {
                if (wheel.axel == Axel.Front) //only front wheels
                {
                    float steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                    wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, t);
                }
            }
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }
    }

    public void Brake(bool braking)
    {
        if (braking)
        {
            foreach (Wheel wheel in wheels)
            {
                if (wheel.axel == Axel.Rear) //only front wheels
                {
                    wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
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
}
