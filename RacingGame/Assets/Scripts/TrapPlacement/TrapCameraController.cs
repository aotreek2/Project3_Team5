using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCameraController : MonoBehaviour
{
    [SerializeField] private float speed = 25;
    [SerializeField] private float zoomSpeed = 5f; 
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float minZoom = 5f;     // Minimum zoom distance
    [SerializeField] private float maxZoom = 20f;    // Maximum zoom distance

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); 
        float vertical = Input.GetAxis("Vertical"); 

        
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        
        transform.position += direction * speed * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newZoom = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }
}

