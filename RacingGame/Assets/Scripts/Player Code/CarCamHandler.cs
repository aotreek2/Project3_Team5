using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CarMoveScr;

public class CarCamHandler : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    CinemachineOrbitalTransposer camPoser;

    Vector2 look;
    float lookTime;
    Coroutine rotateCamRoutine;
    float[,] lookAngle = new float[3, 3]{
        {-135,-90,-45 },
        {179.9f,0,0 },
        {135,90,45 }
    };

    //coroutine vars
    [SerializeField] AnimationCurve camRotCurve;
    float elapsedTime = 0;
    float duration = 1; //makes duration be based on how much the cam has to rotate,
    float desiredAngle;
    float targetAngle;
    bool looking;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        camPoser = cam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        rotateCamRoutine ??= StartCoroutine(SmoothCamRot());
        Invoke("DelayedStart", .1f);
    }

    void DelayedStart()
    {
        cam.Follow = GameObject.Find("CamFollowRef").transform;
        cam.LookAt = GameObject.Find("CamFollowRef").transform;
    }

    public void HandleCamRot(Vector2 input)
    {
        int x = Mathf.RoundToInt(input.x) + 1; // without +1, left or down will be -1 and of index for lookangle
        int y = Mathf.RoundToInt(input.y) + 1;
        //Debug.Log(x + "  " + y);
        //if you have no input, turn on recentering, if not, disable it
        if (x == 1 && y == 1)
        {
            camPoser.m_RecenterToTargetHeading.m_enabled = true;
            looking = false;
            //if (rotateCamRoutine != null) { StopCoroutine(rotateCamRoutine); }
        }
        else
        {
            camPoser.m_RecenterToTargetHeading.m_enabled = false;
            desiredAngle = lookAngle[x, y];
        }
    }
    IEnumerator SmoothCamRot()
    {
        while (true)
        {
            float rotDiff = Mathf.Abs(camPoser.m_XAxis.Value - desiredAngle);
            if (camPoser.m_XAxis.Value != desiredAngle)
            {
                looking = true;
                //Debug.Log("starting rot");
                targetAngle = desiredAngle;
                elapsedTime = 0;
                duration = (rotDiff/ 90) * 2; //makes duration be based on how much the cam has to rotate 
            }
            while (elapsedTime < duration && looking)
            {
                float t = elapsedTime / duration;
                t = camRotCurve.Evaluate(t);

                camPoser.m_XAxis.Value = Mathf.Lerp(camPoser.m_XAxis.Value, targetAngle, t);
                //Debug.Log(camPoser.m_XAxis.Value);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }

    public void ConfineCursor()
    {
        // Confine the cursor to the game window and make it invisible
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Confine the cursor when the game window is focused
        if (hasFocus)
        {
            ConfineCursor();
        }
    }
}
