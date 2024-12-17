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
    private bool isPlayerRetired = false;
    private bool isIaRetired = false;
    private bool isIaEliminated = false;
    private bool isPlayerEliminated = false;

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
        isPlayerRetired = false;
        isPlayerEliminated = false;
        isIaRetired = false;
        isIaEliminated = false;
        isPlayerTurn = true;
        playerRoundScore = 0;
        iaRoundScore = 0;
        
        cardsInGame.Clear();

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        List<CardData> cardDataList = GenerateCardData();
        
        // Inicializar las cartas con animación
        StartCoroutine(InitializeCardsWithAnimation(cardDataList));

        retireButton.onClick.AddListener(OnPlayerRetire);
        UpdateUI();
    }

    private IEnumerator InitializeCardsWithAnimation(List<CardData> cardDataList)
    {
        float animationDuration = 4f;
        float delayPerCard = animationDuration / cardDataList.Count;

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

            yield return new WaitForSeconds(delayPerCard);
        }
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
        if (!card.isRevealed && isPlayerTurn && (!isPlayerRetired || !isPlayerEliminated))
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
            isPlayerEliminated = true;
        }
        else // Primera trampa de este tipo
        {
            trapsRevealed.Add(card.data.type);
        }

        UpdateUI();
    }

    public void OnPlayerRetire()
    {
        if (isPlayerRetired || isPlayerEliminated) return;

        playerScore += playerRoundScore;
        playerRoundScore = 0;
        isPlayerRetired = true;
        
        UpdateUI();
        EndTurn();
    }

    public void StartIATurn()
    {
        if (!isPlayerTurn && (!isIaRetired || !isIaEliminated))
        {
            StartCoroutine(HandleIATurnWithDelay());
        }
    }

    private IEnumerator HandleIATurnWithDelay()
    {
        Debug.Log("IA Turn started.");
        yield return new WaitForSeconds(1f);
        
        if (DecideIfIARetires())
        {
            OnIARetire();
        }
        else
        {
            RevealRandomCardForIA();
        }

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
            isIaEliminated = true;
        }
        else // Primera trampa de este tipo
        {
            trapsRevealed.Add(card.data.type);
        }

        UpdateUI();
    }
    
    private bool DecideIfIARetires()
    {
        int totalCards = cardsInGame.Count;
        int revealedCards = cardsInGame.FindAll(card => card.isRevealed).Count;
        int remainingCards = totalCards - revealedCards;
        int revealedTraps = cardsInGame.FindAll(card => card.isRevealed && card.data.value == 0).Count;

        if (roundCounter == 3)
        {
            if (playerRoundScore + playerScore > iaScore + iaRoundScore) return false;
            if ((isPlayerRetired || isPlayerEliminated) && iaScore + iaRoundScore > playerRoundScore + playerScore) return true;
        }
        
        if (iaRoundScore >= 10) return true; 
        if (revealedTraps <=2) return false; 
        if (iaRoundScore <= 4) return false; 
        
        
        // Cálculo del factor de riesgo
        float riskFactor = (float)revealedTraps / remainingCards;

        // Decisión basada en el riesgo
        if (riskFactor > 0.4f) // Umbral ajustable
        {
            Debug.Log($"IA decides to retire. RiskFactor: {riskFactor}, Remaining Traps: {revealedTraps}, Remaining Cards: {remainingCards}");
            return true;
        }

        Debug.Log($"IA decides to continue. RiskFactor: {riskFactor}, Remaining Traps: {revealedTraps}, Remaining Cards: {remainingCards}");
        return false;
    }

    
    public void OnIARetire()
    {
        iaScore += iaRoundScore;
        iaRoundScore = 0;
        isIaRetired = true;
        
        UpdateUI();
    }
    
    private void EndTurn()
    {
        if ((isPlayerRetired || isPlayerEliminated) && (isIaRetired || isIaEliminated))
        {
            // Si ambos jugadores están fuera, termina la ronda
            Debug.Log("Both players are out. Ending round.");
            StartCoroutine(EndRoundWithPause());
            return;
        }

        if (isPlayerTurn)
        {
            // Es el turno del jugador
            if (isIaRetired || isIaEliminated)
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
            if (isPlayerRetired || isPlayerEliminated)
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

    private IEnumerator EndRoundWithPause()
    {
        yield return new WaitForSeconds(2f); // Pausa antes de inicializar la siguiente ronda

        playerScore += playerRoundScore; // Agregar puntuaciones de la ronda al total
        iaScore += iaRoundScore;
        roundCounter++;

        if (roundCounter > 3) // Si se alcanzan 3 rondas, termina el juego
        {
            foreach (var card in cardsInGame)
            {
                Destroy(card.gameObject); // Eliminar cartas del tablero
            }
            DetermineWinner();
            yield break;
        }

        InitializeGame(); // Iniciar la nueva ronda
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
        
        // Actualizar estado del player
        if (isPlayerEliminated)
        {
            playerRoundScoreText.text = "Player Eliminated";
        }
        else if (isPlayerRetired)
        {
            playerRoundScoreText.text = "Player Retired";
        }
        else
        {
            playerRoundScoreText.text = $"Player: {playerRoundScore}";
        }

        // Actualizar estado de la IA
        if (isIaEliminated)
        {
            iaRoundScoreText.text = "IA Eliminated";
        }
        else if (isIaRetired)
        {
            iaRoundScoreText.text = "IA Retired";
        }
        else
        {
            iaRoundScoreText.text = $"AI: {iaRoundScore}";
        }
            
        roundCounterText.text = $"Round: {roundCounter}";
        turnIndicatorText.text = isPlayerTurn ? "Player's Turn" : "AI's Turn";
    }
}












