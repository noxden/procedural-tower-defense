using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 10f;

    private bool isRotating = false;
    private float initialRotationX;

    private void Update()
    {
        // Zoom the camera in/out
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        float newZoomDistance = Vector3.Distance(transform.position, target.position) - zoomInput * zoomSpeed;
        newZoomDistance = Mathf.Clamp(newZoomDistance, minZoomDistance, maxZoomDistance);
        Vector3 zoomDirection = (transform.position - target.position).normalized;
        transform.position = target.position + zoomDirection * newZoomDistance;

        // Check if left mouse button is pressed and move the mouse horizontally for rotation
        if (Input.GetMouseButtonDown(2))
        {
            isRotating = true;
            initialRotationX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isRotating = false;
        }

        // Rotate the camera horizontally around the target
        if (isRotating)
        {
            float rotationX = Input.mousePosition.x;
            float rotationDelta = (rotationX - initialRotationX) * rotationSpeed;
            initialRotationX = rotationX;
            transform.RotateAround(target.position, Vector3.up, rotationDelta);
        }

        // Look at the target
        transform.LookAt(target);
    }
}
