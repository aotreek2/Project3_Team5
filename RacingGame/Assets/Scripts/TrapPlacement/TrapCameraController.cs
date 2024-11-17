using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float speed = 25;
    

    [Header("Game References")]
    [SerializeField] private GameObject[] aiRacers;
    [SerializeField] private GameObject trap;
    [SerializeField] private GameObject player;

    private Camera cam;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.visible = true;

        foreach (GameObject ai in aiRacers)
        {
           AIRacer aiScript = ai.GetComponent<AIRacer>();
           aiScript.enabled = false;
        }

        CarMoveScr playerScript = player.GetComponent<CarMoveScr>();
        InputManager playerInput = player.GetComponent<InputManager>();
        playerInput.enabled = false;
        playerScript.enabled = false;
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
            speed = 50f;
        }
        else
        {
            speed = 25f;
        }


        transform.position += direction * speed * Time.deltaTime;
    }

    private void PlaceTrap()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
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

