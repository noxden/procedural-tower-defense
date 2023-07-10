using UnityEngine;

public class CameraController : MonoBehaviour
{
    // [SerializeField] private GameEventManagerScriptableObject gameEventManager;
    [SerializeField] private Transform anchor;
    [SerializeField] private bool isMovementEnabled;
    [SerializeField] private bool isRotationEnabled;
    [SerializeField] private bool isZoomEnabled;
    [Space(10)]
    new private Camera camera;
    public float movementSpeed = 10f;
    public float rotationSpeed = 20f;
    public float zoomSpeed = 0.02f;
    [Space(10)]
    public float minZoomDistance = 5f;
    public float maxZoomDistance = 30f;

    private float currentZoomLevel { get => camera.orthographicSize / minZoomDistance; }

    private void OnEnable()
    {
        InputHandler.OnMovementInput += Move;
        InputHandler.OnRotationInput += Rotate;
        InputHandler.OnZoomInput += Zoom;
        GenerationHandler.OnGridSizeChanged.AddListener(ResetPositionToGridCenter);
    }
    private void OnDisable()
    {
        InputHandler.OnMovementInput -= Move;
        InputHandler.OnRotationInput -= Rotate;
        InputHandler.OnZoomInput -= Zoom;
        GenerationHandler.OnGridSizeChanged.RemoveListener(ResetPositionToGridCenter);
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        // ResetPositionToGridCenter(GenerationHandler.instance.gridSize);
    }

    private void Move(Vector2 inputVector)
    {
        if (!isMovementEnabled)
            return;

        Vector3 movementDirection = anchor.transform.forward * inputVector.y + anchor.transform.right * inputVector.x;
        anchor.transform.position += (movementDirection * movementSpeed * (currentZoomLevel / 2)) * Time.unscaledDeltaTime;
    }

    private void Rotate(Vector2 inputVector)
    {
        if (!isRotationEnabled)
            return;

        float rotationDelta = inputVector.x * rotationSpeed;
        anchor.transform.RotateAround(anchor.position, Vector3.up, rotationDelta * Time.unscaledDeltaTime);
    }

    private void Zoom(Vector2 inputVector)
    {
        if (!isZoomEnabled)
            return;

        float zoomInput = inputVector.y;
        float newCameraSize = camera.orthographicSize - zoomInput * zoomSpeed;
        newCameraSize = Mathf.Clamp(newCameraSize, minZoomDistance, maxZoomDistance);
        camera.orthographicSize = newCameraSize;
        // Debug.Log($"New zoom level is {currentZoomLevel}");
    }

    private void ResetPositionToGridCenter(Vector2Int newGridSize)
    {
        Vector2 tileExtends = NodeManager.instance.tileExtends;
        float tileSpacerThickness = NodeManager.instance.tileSpacerThickness;
        Vector2 gridSizeInNodes = newGridSize;

        Vector2 gridExtends = new Vector2(gridSizeInNodes.x * (tileExtends.x + tileSpacerThickness), gridSizeInNodes.y * (tileExtends.y + tileSpacerThickness));
        Vector2 gridCenter = gridExtends * (0.5f - (1f / Mathf.Max(gridSizeInNodes.x + gridSizeInNodes.y, 0.000001f)) / 2);  //< This very weird looking calculation helps camera adapt center based on amount of nodes

        anchor.transform.position = new Vector3(gridCenter.x, anchor.transform.position.y, gridCenter.y);

        maxZoomDistance = (Mathf.Max(newGridSize.x + newGridSize.y, 0.000001f) / 2) * 1.7f;
        zoomSpeed = (1f / 120f) * 0.2f * (maxZoomDistance - minZoomDistance);
        camera.orthographicSize = maxZoomDistance;
    }
}
