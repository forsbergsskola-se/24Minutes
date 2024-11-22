using UnityEngine;

public class MarbleBehavior : MonoBehaviour
{
    public Color gris = Color.gray;
    public Color azul = Color.blue;
    public Color rojo = Color.red;

    public MarbleGameManager gameManager; // Asegúrate de que coincida con el nombre del script del GameManager

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<MarbleGameManager>();
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
}


