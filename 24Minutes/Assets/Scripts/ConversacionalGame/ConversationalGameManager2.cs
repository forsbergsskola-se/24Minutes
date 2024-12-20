using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Input = UnityEngine.Windows.Input;

public class ConversationGameManager2 : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI storyText; // Texto principal
    public Button[] optionButtons; // Botones para las opciones

    [Header("Story")]
    public StoryBlockData initialBlock; // Bloque inicial de la historia
    private StoryBlockData currentBlock;

    private void Start()
    {
        // Cargar el bloque inicial
        DisplayBlock(initialBlock);
    }

    private void DisplayBlock(StoryBlockData block)
    {
        
        
        // Actualiza el texto principal de la historia
        storyText.text = block.storyText;

        // Actualiza las opciones dinámicamente
        UpdateOptionButtons(block);

        currentBlock = block; // Actualiza el bloque actual
    }

    private void UpdateOptionButtons(StoryBlockData block)
    {
        // Configura las opciones y enlaza los botones
        string[] optionTexts = { block.option1Text, block.option2Text, block.option3Text, block.option4Text };
        bool[] optionTransitionals =
        {
            block.option1IsTransition, block.option2IsTransition, block.option3IsTransition, block.option4IsTransition
        };
        string[] optionScenes =
        {
            block.option1TransitionScene, block.option2TransitionScene, block.option3TransitionScene,
            block.option4TransitionScene
        };
        StoryBlockData[] optionBlocks = { block.option1Block, block.option2Block, block.option3Block, block.option4Block };

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (!string.IsNullOrEmpty(optionTexts[i]) && optionBlocks[i] != null)
            {
                // Activa el botón si hay texto y un bloque asociado
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = optionTexts[i];

                // Elimina listeners anteriores y agrega uno nuevo
                int index = i; // Copia el índice para evitar problemas de closures
                optionButtons[i].onClick.RemoveAllListeners();
                if (optionTransitionals[i])
                {
                    optionButtons[i].onClick.AddListener( () => SceneManager.LoadScene(optionScenes[index]));
                }
                else
                {
                    optionButtons[i].onClick.AddListener(() => DisplayBlock(optionBlocks[index]));
                }
                
            }
            else
            {
                // Desactiva el botón si no hay opción
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
