using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class Letter : MonoBehaviour
{
    public List<GameObject> letterPrefabs; // Referencia a los prefabs de A-Z
    public float destroyAfterSeconds = 10f;
    public string letter;
    private bool isTransformable = true; // Para evitar transformaciones después de colisiones
    private LetterManager letterManager;
    
    private void Start()
    {
        letterManager = FindObjectOfType<LetterManager>();
        // Asegurarse de que el objeto se destruya tras el tiempo límite
        //StartCoroutine(DestroyAfterDelay());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Letter"))
        {
            isTransformable = false;// No se puede transformar después de colisionar
            //StartCoroutine(DestroyAfterDelay());
        }
    }

    public void TransformLetter()
    {
        if (!isTransformable) return; // Salir si no es transformable

        // Buscar el índice actual en la lista de prefabs
        int currentIndex = letterPrefabs.FindIndex(prefab => prefab.GetComponent<Letter>().letter == letter);
        if (currentIndex == -1 || currentIndex >= letterPrefabs.Count - 1) return; // No se encuentra o ya es la última letra

        // Cambiar al siguiente prefab
        GameObject nextLetterPrefab = letterPrefabs[currentIndex + 1];
        GameObject nextLetter = Instantiate(nextLetterPrefab, transform.position, Quaternion.identity);

        // Configurar la letra transformada
        Letter nextLetterScript = nextLetter.GetComponent<Letter>();
        if (nextLetterScript != null)
        {
            nextLetterScript.letter = nextLetterPrefab.GetComponent<Letter>().letter;
            
            letterManager.CheckSpecialLetter(nextLetterScript);
        }

        // Eliminar el objeto actual
        Destroy(gameObject);
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





