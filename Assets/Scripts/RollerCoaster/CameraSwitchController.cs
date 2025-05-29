using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitchController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;
    public CinemachineVirtualCamera vcam3;
    public CinemachineVirtualCamera vcam4;

    public float transitionTime = 1.0f;
    private bool isSwitching = false;
    private float timeSinceLastSwitch = 0f;


    // Start is called before the first frame update
    void Start()
    {
        SetCameraPriority(vcam1, 10);  // Set high priority for vcam1
        SetCameraPriority(vcam2, 0);   // Set low priority for vcam2
        SetCameraPriority(vcam3, 0);   // Set low priority for vcam3
        SetCameraPriority(vcam4, 0);   // Set low priority for vcam4
    }

    // Update is called once per frame
    void Update()
    {
        // Track the time since the last camera switch
        timeSinceLastSwitch += Time.deltaTime;
        Debug.Log($"Time since last switch: {timeSinceLastSwitch} with !isSwitching is ${!isSwitching}");

        // Check time intervals and switch cameras accordingly
        if (timeSinceLastSwitch >= 0f && timeSinceLastSwitch < 2f && !isSwitching)
        {
            SwitchCamera(vcam1);  // Switch to vcam1 from 0s to 3s
            Debug.Log($"Switching cam at: {timeSinceLastSwitch}");
        }
        else if (timeSinceLastSwitch >= 2f && timeSinceLastSwitch < 4f && !isSwitching)
        {
            SwitchCamera(vcam2);  // Switch to vcam2 from 3s to 6s
            Debug.Log($"Switching cam at: {timeSinceLastSwitch}");
        }
        else if (timeSinceLastSwitch >= 4f && timeSinceLastSwitch < 6f && !isSwitching)
        {
            SwitchCamera(vcam3);  // Switch to vcam3 from 6s to 9s
            Debug.Log($"Switching cam at: {timeSinceLastSwitch}");
        }
        else if (timeSinceLastSwitch >= 6f && timeSinceLastSwitch < 12f && !isSwitching)
        {
            SwitchCamera(vcam4);  // Switch to vcam4 from 9s to 12s
            Debug.Log($"Switching cam at: {timeSinceLastSwitch}");
        }

        // Reset the time and cycle after 12 seconds
        if (timeSinceLastSwitch >= 12f)
        {
            timeSinceLastSwitch = 0f;  // Restart the cycle
        }
    }

    // Method to switch cameras by adjusting priority
    void SwitchCamera(CinemachineVirtualCamera targetVcam)
    {
        isSwitching = true;  // Prevent multiple switches at once

        // Find the currently active camera by checking priorities
        CinemachineVirtualCamera currentVcam = GetCurrentActiveVcam();

        // Start the transition only if the target camera is different from the current one
        if (currentVcam != targetVcam)
        {
            StartCoroutine(SmoothTransition(currentVcam, targetVcam));
        }

        isSwitching = false;
    }

    // Coroutine to handle smooth camera transition over time
    IEnumerator SmoothTransition(CinemachineVirtualCamera fromVcam, CinemachineVirtualCamera toVcam)
    {
        Debug.Log($"Switching from {fromVcam.Name} to {toVcam.Name}");
        float elapsedTime = 0f;

        // Lower the priority of the current active camera
        SetCameraPriority(fromVcam, 0);  // Lower priority to make it inactive
        SetCameraPriority(toVcam, 10);   // Raise priority to make the new camera active

        // Wait for the transition to complete smoothly over the specified time
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // End of transition, mark switching as complete
        isSwitching = false;
    }

    // Helper method to get the currently active camera by checking priorities
    CinemachineVirtualCamera GetCurrentActiveVcam()
    {
        // Check for the currently active camera (highest priority)
        if (vcam1.Priority > vcam2.Priority && vcam1.Priority > vcam3.Priority && vcam1.Priority > vcam4.Priority)
            return vcam1;
        else if (vcam2.Priority > vcam1.Priority && vcam2.Priority > vcam3.Priority && vcam2.Priority > vcam4.Priority)
            return vcam2;
        else if (vcam3.Priority > vcam1.Priority && vcam3.Priority > vcam2.Priority && vcam3.Priority > vcam4.Priority)
            return vcam3;
        else
            return vcam4;  // Default return vcam4 if it has the highest priority
    }

    // Helper method to set the priority of a Cinemachine Virtual Camera
    void SetCameraPriority(CinemachineVirtualCamera vcam, int priority)
    {
        vcam.Priority = priority;
    }
}
