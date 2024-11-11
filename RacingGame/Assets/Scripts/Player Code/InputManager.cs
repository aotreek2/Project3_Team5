using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    CarMoveScr moveScr;
    PlayerActions playerActions;
    PlayerActions.CarActions carActions;

    private void Awake()
    {
        playerActions = new PlayerActions();
        carActions = playerActions.Car;
        moveScr = gameObject.GetComponent<CarMoveScr>();

        carActions.Stop.started += ctx => moveScr.Brake(true);
        carActions.Stop.canceled += ctx => moveScr.Brake(false);
    }
    private void Update()
    {
        Vector2 moveInput = carActions.Move.ReadValue<Vector2>();
        moveScr.GetMove(moveInput);
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
