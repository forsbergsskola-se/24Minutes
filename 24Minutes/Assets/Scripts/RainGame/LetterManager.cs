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
    public float touchRadius = 0.5f; // Área de detección alrededor del toque
    
    
    private Camera mainCamera;

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
        Rigidbody2D rb = newLetter.GetComponent<Rigidbody2D>();
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




