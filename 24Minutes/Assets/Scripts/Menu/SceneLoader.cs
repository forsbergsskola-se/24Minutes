using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad; // Nombre de la escena que se cargará

    private void Start()
    {
        // Añade un componente Button si no está presente
        if (GetComponent<Button>() == null)
        {
            Button button = gameObject.AddComponent<Button>();
            button.onClick.AddListener(LoadScene);
        }
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name not set in the inspector!");
        }
    }
}

