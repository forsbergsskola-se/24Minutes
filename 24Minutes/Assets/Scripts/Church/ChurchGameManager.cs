using UnityEngine;

public class CircleRevealGame : MonoBehaviour
{
    public Camera mainCamera;            // La cámara principal
    public GameObject imageObject;       // La imagen que el jugador puede hacer zoom (SpriteRenderer)
    public float minZoom = 0.5f;         // Zoom mínimo
    public float maxZoom = 1f;           // Zoom máximo (tamaño original de la imagen)
    public float zoomSpeed = 2f;         // Velocidad de zoom (suave)
    public float minScreenCoverage = 0.1f; // Porcentaje mínimo de pantalla que debe cubrir un círculo
    public int requiredSwipes = 4;       // Número de swipes requeridos para revelar el número

    private Vector2 dragOrigin;
    private SpriteRenderer imageSpriteRenderer;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        imageSpriteRenderer = imageObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleZoom();
        HandleSwipeInput();
        HandleCameraPan(); // Ahora movemos la cámara
    }

    // Maneja el zoom con el gesto de pellizco
    void HandleZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
            float previousTouchDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

            float zoomChange = (currentTouchDistance - previousTouchDistance) * zoomSpeed * Time.deltaTime;
            float newZoom = mainCamera.orthographicSize - zoomChange;

            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    // Detecta los swipes para revelar círculos
    void HandleSwipeInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                if (Vector2.Distance(touch.deltaPosition, Vector2.zero) > 30f)
                {
                    requiredSwipes--;
                    if (requiredSwipes <= 0)
                    {
                        DetectCircleAtTouch();
                        requiredSwipes = 4; // Reinicia el contador de swipes
                    }
                }
            }
        }
    }

    // Detecta si el jugador tocó un círculo
    void DetectCircleAtTouch()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Circle"))
            {
                hit.collider.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    // Maneja el movimiento de la cámara cuando está en zoom
    void HandleCameraPan()
    {
        if (Input.touchCount == 1 && mainCamera.orthographicSize < maxZoom)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = mainCamera.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 difference = dragOrigin - (Vector2)mainCamera.ScreenToWorldPoint(touch.position);
                mainCamera.transform.position = ClampCamera(mainCamera.transform.position + difference);
            }
        }
    }

    // Limita la posición de la cámara para que no se salga de los bordes de la imagen
    Vector3 ClampCamera(Vector3 targetPosition)
    {
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;

        Bounds bounds = imageSpriteRenderer.bounds;

        float minX = bounds.min.x + cameraWidth;
        float maxX = bounds.max.x - cameraWidth;
        float minY = bounds.min.y + cameraHeight;
        float maxY = bounds.max.y - cameraHeight;

        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }
}






