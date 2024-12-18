using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    CarMoveScr moveScr;
    CarCamHandler camHandler;
    PlayerActions playerActions;
    PlayerActions.CarActions carActions;

    private void Awake()
    {
        playerActions = new PlayerActions();
        carActions = playerActions.Car;
        moveScr = gameObject.GetComponent<CarMoveScr>();
        camHandler = GameObject.Find("PlayerCam").GetComponent<CarCamHandler>();

        carActions.Stop.started += ctx => moveScr.ApplyBrake(true);
        carActions.Stop.canceled += ctx => moveScr.ApplyBrake(false);
    }
    private void Start()
    {
        camHandler = GameObject.Find("Virtual Camera").GetComponent<CarCamHandler>();
    }
    private void FixedUpdate()
    {
        Vector2 moveInput = carActions.Move.ReadValue<Vector2>();

        moveScr.GetMove(moveInput);

    }
    private void Update()
    {
        Vector2 LookInput = carActions.Look.ReadValue<Vector2>();
        camHandler.HandleCamRot(LookInput);
    }
    private void OnEnable()
    {
        carActions.Enable();
    }
    private void OnDisable()
    {
        carActions.Disable();
    }
}
