using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Letter2 : MonoBehaviour
{
    public List<GameObject> letterPrefabs; // Referencia a los prefabs de A-Z
    public float destroyAfterSeconds = 10f;
    public string letter;
    public bool isTransformable = true;
    private bool isBeingDragged = false;
    private Vector3 offset;// Para evitar transformaciones después de colisiones
    private LetterManager2 letterManager2;
    private TMPro.TextMeshPro textMesh;
    
    private void Start()
    {
        letterManager2 = FindObjectOfType<LetterManager2>();
        textMesh = GetComponent<TMPro.TextMeshPro>();
        // Asegurarse de que el objeto se destruya tras el tiempo límite
        //StartCoroutine(DestroyAfterDelay());
    }
    
    private void Update()
    {
        // Si está siendo arrastrada, seguir el movimiento del dedo
        if (isBeingDragged)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Mantener en el plano 2D
            transform.position = mousePosition + offset;
        }
    }

    public void OnMouseDown()
    {
        if (!isTransformable)
        {
            // Comenzar a arrastrar si la letra no es transformable
            offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset.z = 0f;
            isBeingDragged = true;

            // Deshabilitar gravedad mientras se arrastra
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;
            
            return;
        }
    }

    private void OnMouseUp()
    {
        isBeingDragged = false;

        // Rehabilitar gravedad si no se colocó en un espacio válido
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        if (!isTransformable)
        {
            // Intentar colocar en un espacio de destino
            letterManager2.TryPlaceLetter(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            if (gameObject.CompareTag("Riddle"))
                
            {
                Destroy(gameObject);
            }
        
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Letter"))
        {
            if (textMesh.color == Color.magenta) return;
            isTransformable = false;// No se puede transformar después de colisionar
            //StartCoroutine(DestroyAfterDelay());
            
            textMesh.color = Color.red;
        }
    }

    public void TransformLetter()
    {
        if (!isTransformable) return; // Salir si no es transformable

        // Buscar el índice actual en la lista de prefabs
        int currentIndex = letterPrefabs.FindIndex(prefab => prefab.GetComponent<Letter2>().letter == letter);
        if (currentIndex == -1 || currentIndex >= letterPrefabs.Count - 1) return; // No se encuentra o ya es la última letra

        // Cambiar al siguiente prefab
        GameObject nextLetterPrefab = letterPrefabs[currentIndex + 1];
        GameObject nextLetter = Instantiate(nextLetterPrefab, transform.position, Quaternion.identity);

        // Configurar la letra transformada
        Letter2 nextLetterScript = nextLetter.GetComponent<Letter2>();
        if (nextLetterScript != null)
        {
            nextLetterScript.letter = nextLetterPrefab.GetComponent<Letter2>().letter;
        }

        // Eliminar el objeto actual
        Destroy(gameObject);
    }
    
    public void SetColor(Color color)
    {
        if (textMesh != null)
        {
            textMesh.color = color;
        }
    }
    
    public void DisableCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void EnableRigidbody()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;
    }
    

    /*private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }*/
    
    public void SetTransformable(bool value)
    {
        isTransformable = value;
    }
}





