using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrapCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float speed = 25;
    
    [Header("Game References")]
    [SerializeField] private GameObject trap;

    private Camera cam;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.visible = true;
    }

    void Update()
    {
        CamMovement();
        PlaceTrap();
    }

    private void CamMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 100f;
        }
        else
        {
            speed = 50f;
        }


        transform.position += direction * speed * Time.deltaTime;
    }

    private void PlaceTrap()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Vector3 mousePosition = Input.mousePosition;

            // Convert mouse position to world position
            Ray ray = cam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                float objectHeight = trap.GetComponent<Collider>().bounds.size.y;
                float spawnOffset = objectHeight / 2f;
                Vector3 spawnPosition = hitInfo.point + new Vector3(0, spawnOffset, 0);
                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                Instantiate(trap, spawnPosition, spawnRotation);
            }
        }
    }
}

