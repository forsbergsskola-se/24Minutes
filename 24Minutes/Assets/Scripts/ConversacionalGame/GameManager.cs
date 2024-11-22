using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Windows.Input;

class StoryBlock
{
    public string story;
    public string option1Text;
    public string option2Text;
    public string option3Text;
    public string option4Text;
    public StoryBlock option1Block;
    public StoryBlock option2Block;
    public StoryBlock option3Block;
    public StoryBlock option4Block;
    
    public StoryBlock(string story, string option1Text = "", string option2Text = "", string option3Text = "", 
        string option4Text = "", StoryBlock option1Block = null, StoryBlock option2Block = null, StoryBlock option3Block = null, StoryBlock option4Block = null)
    {
        this.story = story;
        this.option1Text = option1Text;
        this.option2Text = option2Text;
        this.option3Text = option3Text;
        this.option4Text = option4Text;
        this.option1Block = option1Block;
        this.option2Block = option2Block;
        this.option3Block = option3Block;
        this.option4Block = option4Block;
    }
}

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI conversation;
    public Button option1;
    public Button option2;
    public Button option3;
    public Button option4;
    public TMP_InputField playerInput;
    public int hug = 0;

    private StoryBlock currentBlock;
    private bool hasAskedName = false;

    static StoryBlock block0 = new StoryBlock(
        "It's raining, but you don't feel the cold. Except for your feet, they're frozen. You walk towards a silhouette on the ground. " +
        "It's a child playing marbles under the rain.",
        "Hello, what is your name?", 
        "Ey! What are you doing here?", 
        "I need help!", 
        "You observe the child");

    static StoryBlock block1 = new StoryBlock("My name is Sam.", 
        "Hello, what is your name?", 
        "Ey Sam! What are you doing?", 
        "I need help!", 
        "You observe the child");

    static StoryBlock block2 = new StoryBlock("I've been waiting for you.", 
        "Sure... What is this place?", 
        "Are you alone here?", 
        "I don't know you", 
        "It is raining!");
    static StoryBlock block2_1 = new StoryBlock("You should know...",
        "Sure... What is this place?", 
        "Are you alone here?", 
        "I don't know you", 
        "It is raining!");
    static StoryBlock block2_2 = new StoryBlock("No", 
        "I don't see anybody", 
        "I'm scared");
    static StoryBlock block2_2_1 = new StoryBlock("I see you", 
        "I don't see anybody", 
        "I'm scared");
    static StoryBlock block2_2_2 = new StoryBlock("Do you need a hug?",
        "yes", // Yes = GEt something positive
        "no"); //No = Reset Conversation 
    static StoryBlock block2_3 = new StoryBlock("The kid cries. -You never want to play with me..."); //Reset conversation.

    private static StoryBlock block2_4 = new StoryBlock("-Yes, it makes it more fun!", 
        "-Let's have fun then!", // Play the game
        "Get out of here"); // Exit the Minigame

    static StoryBlock block3 = new StoryBlock("The child looks at you blankly and says nothing.", 
        "-Help me!", 
        "Shake the child by the shoulders");
    
    static StoryBlock block3_1 = new StoryBlock("Let's play a game then", 
        "Play with him", // Play the game hard mode activated
        "No! I'm tired of your stupid game."); //Leave to main menu
    static StoryBlock block3_2 = new StoryBlock("The child hums a tune, ignoring your words completely.", 
        "Cry", 
        "Slap the child");

    static StoryBlock block3_2_1 = new StoryBlock("The child cries with you"); // Coroutine Observe

    static StoryBlock block3_2_2 = new StoryBlock("The child hugs you. " +
                                                           "You can't move. " +
                                                           "You want to scream. " +
                                                           "No sound comes out your of mouth." +
                                                           "Your heart is about to explode."); // Game Over
    
    
    static StoryBlock block4 = new StoryBlock("...");

    void Start()
    {
        DisplayBlock(block0);
    }

    void DisplayBlock(StoryBlock block)
    {
        conversation.text = block.story;

        option1.gameObject.SetActive(!hasAskedName);
        option1.GetComponentInChildren<TextMeshProUGUI>().text = block.option1Text;
        option2.GetComponentInChildren<TextMeshProUGUI>().text = block.option2Text;
        option3.GetComponentInChildren<TextMeshProUGUI>().text = block.option3Text;
        option4.GetComponentInChildren<TextMeshProUGUI>().text = block.option4Text;

        currentBlock = block;
    }

    public void Button1Clicked() // Pregunta el nombre
    {
        if (currentBlock == block0)
        {
            hasAskedName = true;
            DisplayBlock(block1);
            option1.gameObject.SetActive(false); // Desactiva el botón 1
        }

        if (currentBlock == block2)
            DisplayBlock(block2_1);
            option1.gameObject.SetActive(false); // Desactiva el botón 1
            
        if (currentBlock == block2_2)
            DisplayBlock(block2_2_1);
            option1.gameObject.SetActive(false); // Desactiva el botón 1

        if (currentBlock == block2_2_2)
        {
            hug++; //El player consigue algo positivo
        }
        
        //if (currentBlock == block2_3)
            //Wait and reset conversation
        
        //if (currentBlock == block2_4)
            //Play the game charge the scene
            
        //if (currentBlock == block3_1)
            //Play the game hard mode activated
            
            if (currentBlock == block3_2)
            DisplayBlock(block3_2_1);
            
            //yield return new WaitForSeconds(5); Aqui tengo que esperar x segundos antes de seguir ejecutando el codifo (como en observar)
            
    }

    public void Button2Clicked() // Laberinto hacia el minijuego
    {
        option1.gameObject.SetActive(true);
        if (currentBlock == block0 || currentBlock == block1)
            DisplayBlock(block2_1);
        else if (currentBlock == block2_1)
            DisplayBlock(block2_2);
        else if (currentBlock == block2_2)
            DisplayBlock(block2_4); // Llega al minijuego
    }

    public void Button3Clicked() // Conversación sin retorno
    {
        option1.gameObject.SetActive(true);
        if (currentBlock == block0 || currentBlock == block1)
            DisplayBlock(block3);
        else if (currentBlock == block3)
            DisplayBlock(block3_2);
        else if (currentBlock == block3_2)
            DisplayBlock(block3_2);
        else if (currentBlock == block3_2)
            DisplayBlock(block0); // Vuelve al inicio
    }

    public void Button4Clicked() // Observar
    {
        option1.gameObject.SetActive(true);
        if (currentBlock == block0 || currentBlock == block1)
        DisplayBlock(block4);
        StartCoroutine(ObserveRoutine());
    }

    private IEnumerator ObserveRoutine()
    {
        conversation.text = "...";
        option1.gameObject.SetActive(false);
        option2.gameObject.SetActive(false);
        option3.gameObject.SetActive(false);
        option4.gameObject.SetActive(false);

        yield return new WaitForSeconds(5);

        if (hasAskedName)
        {
            option1.gameObject.SetActive(false);
            option2.gameObject.SetActive(true);
            option3.gameObject.SetActive(true);
            option4.gameObject.SetActive(true);
        }
        else
        {
            option1.gameObject.SetActive(true);
            option2.gameObject.SetActive(true);
            option3.gameObject.SetActive(true);
            option4.gameObject.SetActive(true);
        }
        

        DisplayBlock(block0); // Regresa al estado inicial sin la opción de nombre si ya se eligió
    }
}




