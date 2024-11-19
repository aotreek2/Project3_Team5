using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public enum TrapType
    {
        Tires,
        Oil
    }

    [SerializeField] private TrapType currentState;

    void Start()
    {
        HandleTraps();
    }

    private void HandleTraps()
    {
        switch (currentState)
        {
            case TrapType.Tires:
                currentState = TrapType.Tires;
                Debug.Log($"{gameObject.name} is tires.");
                break;

            case TrapType.Oil:
                currentState = TrapType.Oil;
                Debug.Log($"{gameObject.name} is an oil spil");
                break;
            default:
                Debug.LogWarning("Trap error");
                break;
        }
    }

    
}
