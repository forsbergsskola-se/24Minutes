using System;
using UnityEngine;
using UnityEngine.UI;
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
    public Button retireButton;

    private List<Card> cardsInGame = new List<Card>();
    private int playerScore = 0;
    private int iaScore = 0;
    private int playerRoundScore = 0;
    private int iaRoundScore = 0;
    private HashSet<CardType> trapsRevealed = new HashSet<CardType>();

    private bool isPlayerTurn = true;
    private bool isPlayerOut = false;
    private bool isIaOut = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
        retireButton.onClick.AddListener(OnPlayerRetire); // Vincula el botón de retirarse
    }

    private void InitializeGame()
    {
        // Crear y distribuir las cartas
        List<CardData> cardDataList = GenerateCardData();
        foreach (CardData data in cardDataList)
        {
            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            Card card = newCard.GetComponent<Card>();
            Sprite frontImage = cardFrontImages[(int)data.type];
            card.SetupCard(data, frontImage);
            
            // Asignar la función al botón
            Button cardButton = newCard.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => OnCardClicked(card));
            }
            
            cardsInGame.Add(card);
        }

        //ShuffleCards();
        UpdateUI();
    }

    private List<CardData> GenerateCardData()
    {
        List<CardData> cardDataList = new List<CardData>();

        // Añadir cartas de trampa
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
                cardDataList.Add(new CardData { type = (CardType)(i + 2), value = 0 });
        }

        // Añadir Gran Tesoro
        cardDataList.Add(new CardData { type = CardType.Treasure, value = 6 });

        // Añadir cartas de Diamante restantes para llegar al total (25 cartas)
        int remainingDiamonds = 18 - cardDataList.Count;
        for (int i = 0; i < remainingDiamonds; i++) 
        {
            cardDataList.Add(new CardData { type = CardType.Diamond, value = Random.Range(1, 4) });
        }

        // Barajar la lista para hacer la distribución aleatoria
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

    /*private void ShuffleCards()
    {
        for (int i = 0; i < cardsInGame.Count; i++)
        {
            int randomIndex = Random.Range(0, cardsInGame.Count);
            Transform tempTransform = cardsInGame[i].transform;
            cardsInGame[i].transform.position = cardsInGame[randomIndex].transform.position;
            cardsInGame[randomIndex].transform.position = tempTransform.position;
        }
    }*/

    public void OnCardClicked(Card card)
    {
        
        if (!card.isRevealed && isPlayerTurn) // Revelar solo en el turno del jugador
        {
            card.RevealCard();
            HandlePlayerTurn(card);
        }
    }

    private void HandlePlayerTurn(Card card)
    {
        if (card.data.type == CardType.Diamond || card.data.type == CardType.Treasure)
        {
            playerRoundScore += card.data.value;
            
        }/*
        else if (trapsRevealed.Contains(card.data.type))
        {
            playerRoundScore = 0;
            EndTurn();
            return;
        }*/
        else
        {
            trapsRevealed.Add(card.data.type);
        }

        UpdateUI();
        EndTurn();
    }
    
    

    public void OnPlayerRetire()
    {
        playerScore += playerRoundScore; // Acumula los puntos ganados en la ronda.
        playerRoundScore = 0; // Resetea los puntos de la ronda.
        //trapsRevealed.Clear(); // Limpia las trampas reveladas para la siguiente ronda.

        isPlayerTurn = false; // Cambia el turno a la IA.
        UpdateUI();

        StartIATurn(); // Inicia el turno de la IA.
    }

    private void StartIATurn()
    {
        RevealRandomCardForIA();
    }

    private void HandleIATurn(Card card)
    {
        if (card.data.type == CardType.Diamond || card.data.type == CardType.Treasure)
        {
            iaRoundScore += card.data.value;
        }
        /*
        else if (trapsRevealed.Contains(card.data.type))
        {
            //iaRoundScore = 0; // La IA pierde puntos acumulados.
            EndTurn();
            return; // Termina el turno inmediatamente.
        }
        */
        else
        {
            trapsRevealed.Add(card.data.type);
        }

        // Decisión de la IA: continuar o retirarse
        /*bool shouldRetire = DecideIfIARetires();
        if (shouldRetire)
        {
            EndTurn();
        }
        else
        {
            RevealRandomCardForIA();
        }*/
    
        UpdateUI();
        EndTurn();
    }

    private bool DecideIfIARetires()
    {
        int playerPotentialScore = playerScore + playerRoundScore;
        int trapsCount = 0;

        foreach (var card in cardsInGame)
        {
            if (!card.isRevealed && trapsRevealed.Contains(card.data.type))
            {
                trapsCount++;
            }
        }

        float riskFactor = trapsCount / (float)(cardsInGame.Count - trapsRevealed.Count);
        if (playerPotentialScore > iaScore && riskFactor < 0.3f)
        {
            return false; // La IA arriesga si va perdiendo y el riesgo es bajo.
        }

        return true; // Se retira si el riesgo es alto o no necesita arriesgar.
    }

    private void RevealRandomCardForIA()
    {
        List<Card> hiddenCards = cardsInGame.FindAll(card => !card.isRevealed);
        if (hiddenCards.Count > 0)
        {
            int randomIndex = Random.Range(0, hiddenCards.Count);
            hiddenCards[randomIndex].RevealCard(); // Revela la carta.
            EndTurn();
        }
        else
        {
            EndTurn(); // Si no hay cartas ocultas, termina el turno.
        }
    }

    public void EndTurn()
    {
        if (isPlayerTurn)
        {
            //playerRoundScore += playerRoundScore;
            isPlayerTurn = false;
            StartIATurn();
        }
        else
        {
            isPlayerTurn = true;
        }
        
        if (!isPlayerTurn)
        {
            StartCoroutine(IATurnDelay());
        }
        
        UpdateUI();
    }

    public void EndRound()
    {
        if (isPlayerTurn)
        {
            playerScore += playerRoundScore;
            isPlayerTurn = false;
            StartIATurn();
        }
        else
        {
            iaRoundScore += iaRoundScore;
            isPlayerTurn = true;
        }
        
        trapsRevealed.Clear();
        UpdateUI();
    }

    private IEnumerator IATurnDelay()
    {
        yield return new WaitForSeconds(1f);
    }

    private void UpdateUI()
    {
        playerRoundScoreText.text = "Jugador: " + playerRoundScore;
        iaRoundScoreText.text = "IA: " + iaRoundScore;
        turnIndicatorText.text = isPlayerTurn ? "Turno del Jugador" : "Turno de la IA";
    }
}




