using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrapCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float speed = 25;
    [SerializeField] private int minFOV = 15;
    [SerializeField] private int maxFOV = 60;


    [Header("Game References")]
    [SerializeField] private GameObject trapPreview;
    [SerializeField] private GameObject[] traps;
    [SerializeField] private int trapsPlaced;
    [SerializeField] private TMP_Text trapsLeft, currentTrap;
    private Camera cam;
    private int trapIndex;

    private void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.visible = true;
        trapsLeft.text = "Traps Left: " + (5 - trapsPlaced);
        PreviewTrap();
    }

    void Update()
    {
        CamMovement();
        PlaceTrap();
        SwitchTraps();
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

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        cam.fieldOfView -= scrollInput * 10f;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
    }

    private void SwitchTraps()
    {
        trapIndex = Mathf.Clamp(trapIndex, 0, traps.Length);

        if (trapsPlaced < 5)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                trapIndex = (trapIndex - 1) % traps.Length;
                trapIndex = Mathf.Clamp(trapIndex, 0, traps.Length);
                Destroy(trapPreview);
                trapPreview = Instantiate(traps[trapIndex]);
                //currentTrap.text = "Trap: " + traps[trapIndex].name;
                trapPreview.GetComponent<Collider>().enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                trapIndex = (trapIndex + 1) % traps.Length;
                trapIndex = Mathf.Clamp(trapIndex, 0, traps.Length);
                Destroy(trapPreview);
                trapPreview = Instantiate(traps[trapIndex]);
                //currentTrap.text = "Trap: " + traps[trapIndex].name;
                trapPreview.GetComponent<Collider>().enabled = false;
            }
        }
    }

    private void PreviewTrap()
    {
        trapPreview = traps[trapIndex];
        //currentTrap.text = "Trap: " + traps[trapIndex].name;

        if (trapPreview != null)
        {
            trapPreview = Instantiate(traps[trapIndex]);
            trapPreview.GetComponent<Collider>().enabled = false;
        }
    }
    private void PlaceTrap()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            float objectHeight = traps[trapIndex].GetComponent<Collider>().bounds.size.y;
            float spawnOffset = objectHeight / 2f;
            Vector3 spawnPosition = hitInfo.point + new Vector3(0, spawnOffset, 0);
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            trapPreview.transform.position = spawnPosition;

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                if (trapsPlaced < 5)
                {
                    Instantiate(traps[trapIndex], spawnPosition, spawnRotation);
                    trapsPlaced += 1;
                    trapsLeft.text = "Traps Left: " + (5 - trapsPlaced);
                }
                if(trapsPlaced == 5)
                {
                    trapPreview.SetActive(false);
                }
            }
        }
    }
}

