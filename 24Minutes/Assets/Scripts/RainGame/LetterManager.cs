using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LetterManager : MonoBehaviour
{
    public List<GameObject> letterPrefabs; // Prefabs de las letras (A-Z)
    public Transform spawnArea; // Área donde aparecen las letras
    public List<GameObject> objectsToDisable;
    public float disableAfterSeconds = 5f;
    public float destroyAfterSeconds = 10f;
    public float touchRadius = 0.25f; // Área de detección alrededor del toque
    public string secretWord;
    private int currentSecretIndex = 0;
    public float moveSpeed = 5f; // Velocidad de movimiento de la letra especial
    public Transform[] targetPositions; // Posiciones para cada letra de la palabra secreta
    
    private Camera mainCamera;
    private bool canInteract = true; // Controla si se puede interactuar

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(DisableObjectsAfterDelay());
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTouch(Input.GetTouch(0).position);
        }
    }
    
    public void CheckSpecialLetter(Letter letterScript)
    {
        if (letterScript.letter == secretWord[currentSecretIndex].ToString())
        {
            StartCoroutine(HandleSpecialLetter(letterScript));
        }
    }

    private void HandleTouch(Vector2 touchPosition)
    {
        // Convertir posición de la pantalla a coordenadas del mundo
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPosition.z = 0f;

        // Buscar si hay una letra cerca del punto de toque
        Collider2D hitCollider = Physics2D.OverlapCircle(worldPosition, touchRadius, LayerMask.GetMask("Letter"));

        if (hitCollider != null)
        {
            // Si se toca una letra, transformarla
            Letter letterScript = hitCollider.GetComponent<Letter>();
            if (letterScript != null)
            {
                letterScript.TransformLetter();

                if (letterScript.letter == secretWord[currentSecretIndex].ToString())
                {
                    StartCoroutine(HandleSpecialLetter(letterScript));
                }
            }
        }
        else
        {
            // Si no hay ninguna letra, generar una nueva
            SpawnLetter(worldPosition);
        }
    }

    private void SpawnLetter(Vector2 spawnPosition)
    {
        GameObject newLetter = Instantiate(letterPrefabs[0], spawnPosition, Quaternion.identity);
        //Rigidbody2D rb = newLetter.GetComponent<Rigidbody2D>();
        Letter letterScript = newLetter.GetComponent<Letter>();
        
        if (letterScript != null && letterScript.letter == secretWord[currentSecretIndex].ToString())
        {
            StartCoroutine(HandleSpecialLetter(letterScript));
        }
    }
    
    private IEnumerator HandleSpecialLetter(Letter letterScript)
    {
        canInteract = false;
        
        // Deshabilitar transformaciones y colisiones de la letra especial
        Rigidbody2D rb = letterScript.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Collider2D col = letterScript.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Mover la letra especial a su posición objetivo
        Transform targetPosition = targetPositions[currentSecretIndex];
        yield return StartCoroutine(MoveToPosition(letterScript.transform, targetPosition.position));

        // Avanzar al siguiente índice de la palabra secreta
        currentSecretIndex++;

        // Verificar si se completó la palabra secreta
        if (currentSecretIndex >= secretWord.Length)
        {
            Debug.Log("¡Palabra secreta completada!");
        }

        canInteract = true;
    }

    private IEnumerator MoveToPosition(Transform letterTransform, Vector3 targetPosition)
    {
        while (Vector3.Distance(letterTransform.position, targetPosition) > 0.01f)
        {
            letterTransform.position = Vector3.MoveTowards(letterTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Asegurar posición final exacta
        letterTransform.position = targetPosition;
    }
    
    private IEnumerator DisableObjectsAfterDelay()
    {
        yield return new WaitForSeconds(disableAfterSeconds);

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false); // Desactiva el objeto
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar el radio del área de detección (para debug)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(mainCamera.ScreenToWorldPoint(Input.mousePosition), touchRadius);
    }
}




