using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LetterManager2 : MonoBehaviour
{
    public List<GameObject> letterPrefabs; // Prefabs de las letras (A-Z)
    public Transform spawnArea; // Área donde aparecen las letras
    public List<GameObject> objectsToDisable;
    public float disableAfterSeconds = 5f;
    public float destroyAfterSeconds = 10f;
    public float touchRadius = 0.25f; // Área de detección alrededor del toque
    public string secretWord;
    //private int currentSecretIndex = 0;
    //public float moveSpeed = 5f; // Velocidad de movimiento de la letra especial
    public Transform[] targetPositions; // Posiciones para cada letra de la palabra secreta
    private HashSet<Transform> occupiedSpaces = new HashSet<Transform>();
    
    private Camera mainCamera;
    //private bool canInteract = true; // Controla si se puede interactuar
    private Dictionary<Transform, Letter2> placedLetters = new Dictionary<Transform, Letter2>();
   
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
    
    /*public void CheckSpecialLetter(Letter letterScript)
    {
        if (letterScript.letter2 == secretWord[currentSecretIndex].ToString())
        {
            StartCoroutine(HandleSpecialLetter(letterScript));
        }
    }*/

    private void HandleTouch(Vector2 touchPosition)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        worldPosition.z = 0f;
        
        if (IsPointerOverUIObject()) return;

        Collider2D hitCollider = Physics2D.OverlapCircle(worldPosition, touchRadius, LayerMask.GetMask("Letter"));

        if (hitCollider != null)
        {
            Letter2 letterScript = hitCollider.GetComponent<Letter2>();
            if (letterScript != null)
            {
                if (letterScript.isTransformable)
                    
                {
                    letterScript.TransformLetter();
                }
            }
        }
        else
        {
            SpawnLetter(worldPosition);
        }
    }
    
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Filtrar solo elementos del canvas UI
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Canvas>() != null)
            {
                return true; // El puntero está sobre un elemento UI real
            }
        }

        return false; // No está sobre un elemento UI real
    }


    private void SpawnLetter(Vector2 spawnPosition)
    {
        GameObject newLetter = Instantiate(letterPrefabs[0], spawnPosition, Quaternion.identity);
    }
    
    public void TryPlaceLetter(Letter2 letter)
    {
        foreach (Transform targetSpace in targetPositions)
        {
            if (!placedLetters.ContainsKey(targetSpace) &&
                Vector3.Distance(letter.transform.position, targetSpace.position) < 0.5f)
            {
                // Colocar la letra en el espacio
                letter.transform.position = targetSpace.position;
                letter.transform.rotation = Quaternion.identity;

                // Deshabilitar el Rigidbody si se coloca correctamente
                Rigidbody2D rb = letter.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;

                // Agregar la letra al diccionario
                placedLetters[targetSpace] = letter;

                Debug.Log($"Letra {letter.letter} colocada en {targetSpace.position}");
                return;
            }
        }

        Debug.Log("No se puede colocar la letra aquí.");
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
    
    public void VerifyWord()
    {
        bool isWordCorrect = true;

        // Crear una lista temporal para liberar espacios después
        List<Transform> spacesToFree = new List<Transform>();

        foreach (var entry in placedLetters)
        {
            Transform targetSpace = entry.Key;
            Letter2 letterScript = entry.Value;
            
            List<Transform> targetPositionsList = new List<Transform>(targetPositions);

            if (targetPositionsList.Contains(targetSpace))
            {
                int positionIndex = System.Array.IndexOf(targetPositions, targetSpace);

                if (positionIndex < secretWord.Length && letterScript.letter == secretWord[positionIndex].ToString())
                {
                    // Letra correcta en la posición correcta
                    letterScript.SetColor(Color.blue);
                    letterScript.DisableCollider();
                    // Espacio se mantiene ocupado, no se libera
                    continue;
                }
                else if (secretWord.Contains(letterScript.letter))
                {
                    // Letra en la palabra pero en la posición incorrecta
                    letterScript.SetColor(Color.magenta);
                    letterScript.EnableRigidbody(); // Hacer que caiga

                    // Marcar espacio para liberar
                    spacesToFree.Add(targetSpace);
                }
                else
                {
                    // Letra no pertenece a la palabra
                    letterScript.SetColor(Color.red);
                    letterScript.EnableRigidbody();

                    // Marcar espacio para liberar
                    spacesToFree.Add(targetSpace);
                    isWordCorrect = false;
                }
            }
            else
            {
                Debug.Log($"Espacio en {targetSpace.position} está vacío.");
                isWordCorrect = false;
            }
        }

        // Liberar los espacios marcados
        foreach (Transform space in spacesToFree)
        {
            placedLetters.Remove(space);
        }

        if (isWordCorrect)
        {
            Debug.Log("¡Palabra secreta completada!");
        }
    }






    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("Rain");
    }
}





