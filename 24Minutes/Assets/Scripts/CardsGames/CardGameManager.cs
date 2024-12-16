using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;

public class CardGameManager : MonoBehaviour
{
    public static CardGameManager Instance;

    [Header("Game Setup")]
    public Transform cardContainer; 
    public GameObject cardPrefab; 
    public List<Sprite> cardFrontImages; 

    [Header("UI")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI iaScoreText;
    public TextMeshProUGUI playerRoundScoreText;
    public TextMeshProUGUI iaRoundScoreText;
    public TextMeshProUGUI turnIndicatorText;
    public TextMeshProUGUI roundCounterText;
    public TextMeshProUGUI endGameMessageText;
    public Button retireButton;

    private List<Card> cardsInGame = new List<Card>();
    private int playerScore = 0;
    private int iaScore = 0;
    private int playerRoundScore = 0;
    private int iaRoundScore = 0;
    private HashSet<CardType> trapsRevealed = new HashSet<CardType>();
    private int roundCounter = 1; // Contador de rondas
    private bool isGameOver = false;

    public bool isPlayerTurn = true;
    private bool isPlayerOut = false;
    private bool isIaOut = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("Initializing game...");

        trapsRevealed.Clear();
        isPlayerOut = false;
        isIaOut = false;
        isPlayerTurn = true;
        playerRoundScore = 0;
        iaRoundScore = 0;
        
        cardsInGame.Clear();

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        List<CardData> cardDataList = GenerateCardData();
        foreach (CardData data in cardDataList)
        {
            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            Card card = newCard.GetComponent<Card>();
            if (card == null)
            {
                Debug.LogError("Card component not found on prefab!");
                continue;
            }

            Sprite frontImage = cardFrontImages[(int)data.type];
            card.SetupCard(data, frontImage);

            Button cardButton = newCard.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => OnCardClicked(card));
            }
            else
            {
                Debug.LogError("Button component missing on card prefab!");
            }

            cardsInGame.Add(card);
        }

        retireButton.onClick.AddListener(OnPlayerRetire);
        UpdateUI();
    }

    private List<CardData> GenerateCardData()
    {
        List<CardData> cardDataList = new List<CardData>();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
                cardDataList.Add(new CardData { type = (CardType)(i + 2), value = 0 });
        }

        cardDataList.Add(new CardData { type = CardType.Treasure, value = 6 });

        int remainingDiamonds = 18 - cardDataList.Count;
        for (int i = 0; i < remainingDiamonds; i++) 
        {
            cardDataList.Add(new CardData { type = CardType.Diamond, value = Random.Range(1, 4) });
        }

        ShuffleList(cardDataList);

        return cardDataList;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void OnCardClicked(Card card)
    {
        if (!card.isRevealed && isPlayerTurn && !isPlayerOut)
        {
            Debug.Log("Player clicked on card.");
            card.RevealCard();
            HandlePlayerTurn(card);
            EndTurn();
        }
    }

    private void HandlePlayerTurn(Card card)
    {
        if (card.data.value > 0) // Diamantes o Tesoro
        {
            playerRoundScore += card.data.value;
        }
        else if (trapsRevealed.Contains(card.data.type)) // Trampa duplicada
        {
            playerRoundScore = 0;
            isPlayerOut = true;
            turnIndicatorText.text = "Player Eliminated!";
            playerRoundScoreText.text = "Player Eliminated";
            
        }
        else // Primera trampa de este tipo
        {
            trapsRevealed.Add(card.data.type);
        }

        UpdateUI();
    }

    public void OnPlayerRetire()
    {
        if (isPlayerOut) return;

        playerScore += playerRoundScore;
        playerRoundScore = 0;
        isPlayerOut = true;

        playerRoundScoreText.text = "Player Retired";
        turnIndicatorText.text = "Player Retired!";
        UpdateUI();
        EndTurn();
    }

    public void StartIATurn()
    {
        if (!isPlayerTurn && !isIaOut)
        {
            StartCoroutine(HandleIATurnWithDelay());
        }
    }

    private IEnumerator HandleIATurnWithDelay()
    {
        Debug.Log("IA Turn started.");
        yield return new WaitForSeconds(2f);

        RevealRandomCardForIA();

        yield return new WaitForSeconds(1f);
        EndTurn();
    }

    private void RevealRandomCardForIA()
    {
        List<Card> hiddenCards = cardsInGame.FindAll(card => !card.isRevealed);

        if (hiddenCards.Count > 0)
        {
            Debug.Log("IA is revealing a card.");
            int randomIndex = Random.Range(0, hiddenCards.Count);
            Card chosenCard = hiddenCards[randomIndex];
            chosenCard.RevealCard();
            HandleIATurn(chosenCard);
        }
        else
        {
            Debug.LogError("No cards available for IA to reveal.");
        }
    }

    public void HandleIATurn(Card card)
    {
        if (card.data.value > 0) // Diamantes o Tesoro
        {
            iaRoundScore += card.data.value;
        }
        else if (trapsRevealed.Contains(card.data.type)) // Trampa duplicada
        {
            iaRoundScore = 0;
            isIaOut = true;
            iaRoundScoreText.text = "IA Eliminated";
            turnIndicatorText.text = "IA Eliminated!";
        }
        else // Primera trampa de este tipo
        {
            trapsRevealed.Add(card.data.type);
        }

        UpdateUI();
    }

    private void EndTurn()
    {
        if (isPlayerOut && isIaOut)
        {
            // Si ambos jugadores están fuera, termina la ronda
            Debug.Log("Both players are out. Ending round.");
            EndRound();
            return;
        }

        if (isPlayerTurn)
        {
            // Es el turno del jugador
            if (isIaOut)
            {
                // Si la IA está fuera, el jugador continúa jugando
                Debug.Log("IA is out. Player continues their turn.");
                isPlayerTurn = true; // No cambia el turno
            }
            else
            {
                // Si la IA no está fuera, pasa el turno a la IA
                Debug.Log("Player's turn ends. Passing to IA.");
                isPlayerTurn = false;
                StartIATurn();
            }
        }
        else
        {
            // Es el turno de la IA
            if (isPlayerOut)
            {
                // Si el jugador está fuera, la IA continúa jugando
                Debug.Log("Player is out. IA continues their turn.");
                isPlayerTurn = false; // No cambia el turno
                StartIATurn();
            }
            else
            {
                // Si el jugador no está fuera, pasa el turno al jugador
                Debug.Log("IA's turn ends. Passing to Player.");
                isPlayerTurn = true;
            }
        }

        UpdateUI();
    }


    private void EndRound()
    {
        playerScore += playerRoundScore;
        iaScore += iaRoundScore;
        roundCounter++;

        if (roundCounter > 3) // Fin del juego tras 3 rondas
        {
            DetermineWinner();
            return;
        }

        InitializeGame();
    }
    private void DetermineWinner()
    {
        isGameOver = true;
        if (playerScore > iaScore)
        {
            endGameMessageText.text = "Player Wins!";
        }
        else if (iaScore > playerScore)
        {
            endGameMessageText.text = "IA Wins!";
        }
        else
        {
            endGameMessageText.text = "It's a Tie!";
        }

        Debug.Log("Game Over!");
    }

    private void UpdateUI()
    {
        playerScoreText.text = $"Player: {playerScore}";
        iaScoreText.text = $"AI: {iaScore}";
        playerRoundScoreText.text = isPlayerOut ? "Player Eliminated" : $"Player Round: {playerRoundScore}";
        iaRoundScoreText.text = isIaOut ? "IA Eliminated" : $"AI Round: {iaRoundScore}";
        roundCounterText.text = $"Round: {roundCounter}";
        turnIndicatorText.text = isPlayerTurn ? "Player's Turn" : "AI's Turn";
    }
}












