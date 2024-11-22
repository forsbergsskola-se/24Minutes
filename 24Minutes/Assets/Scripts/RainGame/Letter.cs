using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Letter : MonoBehaviour
{
    public List<GameObject> letterPrefabs; // Referencia a los prefabs de A-Z
    public float destroyAfterSeconds = 10f;

    private bool isTransformable = true; // Para evitar transformaciones después de colisiones

    private void Start()
    {
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
        if (!isTransformable) return; // Salir si no se puede transformar

        // Cambiar al siguiente prefab
        int currentIndex = letterPrefabs.IndexOf(gameObject);
        if (currentIndex < letterPrefabs.Count - 1)
        {
            GameObject nextLetter = letterPrefabs[currentIndex + 1];
            Destroy(gameObject); // Eliminar la letra actual
            Instantiate(nextLetter, transform.position, Quaternion.identity);
        }
    }

    /*private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);
        Destroy(gameObject);
    }*/
}





