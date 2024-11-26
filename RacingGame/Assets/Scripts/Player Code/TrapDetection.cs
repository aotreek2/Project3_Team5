using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDetection : MonoBehaviour
{
    private float flashInterval = 0.1f;
    private float disableDuration = 2.0f;
    private float upwardsAccl = 25;
    private Renderer[] modelRenderers;
    private bool isFlashing = false;

    Rigidbody rb;
    void Start()
    {
        modelRenderers = GetComponentsInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

   

    private void StartFlashing(float dragAmount)
    {
        if (!isFlashing)
        {
            isFlashing = true;
            StartCoroutine(FlashEffect());
            StartCoroutine(AddDrag(dragAmount));
        }
    }

    private void StopFlashing()
    {
        if (isFlashing)
        {
            isFlashing = false;
            StopCoroutine(FlashEffect());
            TurnVisible(true); 
        }
    }

    private IEnumerator FlashEffect()
    {
        while (isFlashing)
        {
            TurnVisible(false); 
            yield return new WaitForSeconds(flashInterval);
            TurnVisible(true); 
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void TurnVisible(bool isVisible)
    {
        foreach (Renderer renderer in modelRenderers)
        {
            renderer.enabled = isVisible;
        }
    }

    private IEnumerator AddDrag(float dragAmount)
    {
        rb.drag = dragAmount;
        yield return new WaitForSeconds(disableDuration);
        rb.drag = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rocks")
            {
                StartFlashing(20f);
                Invoke("StopFlashing", 3f);
            }

        else if (collision.gameObject.tag == "Wall")
            {
                Debug.Log("Collided with Wall!");
                StartFlashing(20f);
                Invoke("StopFlashing", 3f);
            }
            else if (collision.gameObject.tag == "Tires")
            {
                Debug.Log("Collided with Tires!");
                StartFlashing(20f);
                Invoke("StopFlashing", 3f);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
         if (other.gameObject.tag == "Spike")
        {
            Debug.Log("Collided with Spikes!");
            StartFlashing(30f);
            Invoke("StopFlashing", 3f);
        }
        else if (other.gameObject.tag == "Mine")
        {
            float totalAcceleration = 9.8f + upwardsAccl;
            float force = rb.mass * totalAcceleration;
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            rb.AddTorque(Vector3.up * force, ForceMode.Impulse);
            StartFlashing(0f);
            Invoke("StopFlashing", 3f);
            Destroy(other.gameObject);
        }
    }
}
