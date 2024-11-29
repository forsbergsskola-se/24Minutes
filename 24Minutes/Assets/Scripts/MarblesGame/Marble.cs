using UnityEngine;

public class MarbleBehavior : MonoBehaviour
{
    public Color gris = Color.gray;
    public Color azul = Color.blue;
    public Color rojo = Color.red;

    public MarbleGameManager gameManager; // Referencia al ConversationGameManager
    public GameObject ground; // Objeto Ground para definir el área válida

    private Collider groundCollider;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<MarbleGameManager>();
        }

        if (ground == null)
        {
            ground = gameManager.ground; // Usamos la referencia del ConversationGameManager
        }

        if (ground != null)
        {
            groundCollider = ground.GetComponent<Collider>();
        }
    }

    private void FixedUpdate()
    {
        if (groundCollider == null) return;

        // Obtenemos los límites del suelo
        Bounds groundBounds = groundCollider.bounds;

        // Comprobamos si la canica está fuera del área válida
        Vector3 position = transform.position;

        if (position.x < groundBounds.min.x || position.x > groundBounds.max.x ||
            position.z < groundBounds.min.z || position.z > groundBounds.max.z)
        {
            Debug.Log($"{gameObject.name} salió del área válida. Teletransportando a (0, 0, 0)");
            TeleportToSafeZone();
        }
    }

    private void TeleportToSafeZone()
    {
        // Teletransporta la canica al origen (0, 0, 0) y detiene su movimiento
        transform.position = Vector3.zero;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Detenemos cualquier velocidad
            rb.angularVelocity = Vector3.zero; // Detenemos cualquier rotación
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        Renderer otherRenderer = collision.collider.GetComponent<Renderer>();

        if (thisRenderer == null || otherRenderer == null) return;

        if (gameManager.isPlayerTurn)
        {
            if (otherRenderer.material.color == rojo && gameObject.CompareTag("NeutralMarble"))
            {
                thisRenderer.material.color = rojo;
            }
        }
        else
        {
            if (otherRenderer.material.color == azul && gameObject.CompareTag("NeutralMarble"))
            {
                thisRenderer.material.color = azul;
            }
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        Renderer thisRenderer = GetComponent<Renderer>();
        Renderer otherRenderer = collision.collider.GetComponent<Renderer>();

        if (thisRenderer == null || otherRenderer == null) return;

        if (gameManager.isPlayerTurn)
        {
            if (otherRenderer.material.color == rojo && gameObject.CompareTag("NeutralMarble"))
            {
                thisRenderer.material.color = rojo;
            }
        }
        else
        {
            if (otherRenderer.material.color == azul && gameObject.CompareTag("NeutralMarble"))
            {
                thisRenderer.material.color = azul;
            }
        }
    }
}



