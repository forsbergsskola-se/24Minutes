using UnityEngine;
using TMPro;

public class ChurchGameManager2 : MonoBehaviour
{
    public float opacityIncreasePercentage = 0.1f; // Incremento de opacidad en porcentaje
    private Camera mainCamera;                // La cámara principal

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        HandleTouchInput();
    }

    // Maneja el toque sobre los objetos para aumentar la opacidad de sus hijos
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Verifica si el objeto tocado tiene un hijo con el componente TextMeshPro
                if (hit.collider.transform.childCount > 0)
                {
                    Transform childTransform = hit.collider.transform.GetChild(0);
                    TextMeshPro childTextMeshPro = childTransform.GetComponent<TextMeshPro>();

                    // Si el hijo tiene el componente TextMeshPro
                    if (childTextMeshPro != null)
                    {
                        // Obtener el valor de opacidad actual
                        Color currentColor = childTextMeshPro.color;
                        float currentAlpha = currentColor.a;

                        // Incrementa la opacidad en un 10% del valor máximo (1)
                        float maxAlpha = 1f;
                        float alphaIncrement = maxAlpha * opacityIncreasePercentage;

                        // Calculamos el nuevo valor de opacidad y lo actualizamos
                        float newAlpha = Mathf.Clamp(currentAlpha + alphaIncrement, 0f, 1f);
                        childTextMeshPro.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                    }
                }
            }
        }
    }
}








