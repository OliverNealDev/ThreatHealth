using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Drag the Player object here")]
    [SerializeField] private Transform target;
    
    [Header("Smoothing")]
    [Tooltip("Approximate time for the camera to reach the target. Smaller is faster.")]
    [SerializeField] private float smoothTime = 0.25f;
    
    [Header("Offset")]
    [Tooltip("Distance from the player (X, Y). Z should usually be kept negative.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private Vector3 _currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Define where we want to be
        Vector3 targetPosition = target.position + offset;

        // 2. Lock the Z axis
        // We generally want to keep the camera's original Z (usually -10) 
        // so it doesn't clip inside the 2D plane.
        targetPosition.z = -10f; 

        // 3. Smoothly move there
        // SmoothDamp gradually changes a vector towards a desired goal over time.
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref _currentVelocity, 
            smoothTime
        );
    }
}