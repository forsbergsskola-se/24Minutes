using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardContainer;
    public Button passTurnButton;
    public Text playerScoreText, aiScoreText, roundText, turnText;

    private List<Card> cards = new List<Card>();
    private int playerScore = 0, aiScore = 0;
    private int round = 1;
    private bool isPlayerTurn = true;
    private int currentRoundScore = 0;
    private HashSet<string> discoveredTraps = new HashSet<string>();

    void Start()
    {
        passTurnButton.onClick.AddListener(PassTurn);
        SetupGame();
    }

    void SetupGame()
    {
        // Generate cards and shuffle
        List<CardData> deck = GenerateDeck();
        deck = ShuffleDeck(deck);

        foreach (CardData cardData in deck)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardContainer);
            Card card = cardObject.GetComponent<Card>();
            card.Setup(cardData, OnCardClicked);
            cards.Add(card);
        }

        UpdateUI();
    }

    List<CardData> GenerateDeck()
    {
        List<CardData> deck = new List<CardData>();

        // Add cards
        for (int i = 0; i < 11; i++)
        {
            deck.Add(new CardData("Diamond", Random.Range(1, 4)));
        }
        deck.Add(new CardData("Treasure", 6));
        for (int i = 1; i <= 4; i++)
        {
            deck.Add(new CardData($"Trap{i}", 0));
            deck.Add(new CardData($"Trap{i}", 0));
        }

        return deck;
    }

    List<CardData> ShuffleDeck(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(0, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
        return deck;
    }

    void OnCardClicked(Card card)
    {
        if (!isPlayerTurn || card.IsRevealed) return;

        card.Reveal();
        CardData cardData = card.CardData;

        if (cardData.Type.StartsWith("Trap"))
        {
            if (discoveredTraps.Contains(cardData.Type))
            {
                // Lose turn and round score
                EndPlayerTurn(false);
                return;
            }
            else
            {
                discoveredTraps.Add(cardData.Type);
            }
        }
        else
        {
            currentRoundScore += cardData.Value;
        }

        UpdateUI();
    }

    void PassTurn()
    {
        if (currentRoundScore > 0 && discoveredTraps.Count > 0)
        {
            EndPlayerTurn(true);
        }
    }

    void EndPlayerTurn(bool success)
    {
        if (success)
        {
            playerScore += currentRoundScore;
        }

        ResetRound();
        isPlayerTurn = false;
        StartCoroutine(AITurn());
    }

    IEnumerator AITurn()
    {
        int aiRoundScore = 0;
        HashSet<string> aiDiscoveredTraps = new HashSet<string>();

        foreach (Card card in cards)
        {
            if (!card.IsRevealed)
            {
                yield return new WaitForSeconds(2);

                card.Reveal();
                CardData cardData = card.CardData;

                if (cardData.Type.StartsWith("Trap"))
                {
                    if (aiDiscoveredTraps.Contains(cardData.Type))
                    {
                        break;
                    }
                    else
                    {
                        aiDiscoveredTraps.Add(cardData.Type);
                    }
                }
                else
                {
                    aiRoundScore += cardData.Value;

                    // Risk-taking logic
                    if (ShouldAIPass(aiRoundScore, aiDiscoveredTraps.Count))
                    {
                        break;
                    }
                }
            }
        }

        aiScore += aiRoundScore;
        ResetRound();

        if (round == 3)
        {
            EndGame();
        }
        else
        {
            isPlayerTurn = true;
            round++;
            UpdateUI();
        }
    }

    bool ShouldAIPass(int aiRoundScore, int trapCount)
    {
        if (aiScore + aiRoundScore > playerScore) return true;
        if (trapCount >= 2) return true;
        if (playerScore > aiScore && Random.Range(0f, 1f) > 0.7f) return false;

        return Random.Range(0f, 1f) > 0.5f;
    }

    void ResetRound()
    {
        currentRoundScore = 0;
        discoveredTraps.Clear();
    }

    void UpdateUI()
    {
        playerScoreText.text = $"Player Score: {playerScore}";
        aiScoreText.text = $"AI Score: {aiScore}";
        roundText.text = $"Round: {round}/3";
        turnText.text = isPlayerTurn ? "Your Turn" : "AI Turn";
    }

    void EndGame()
    {
        if (playerScore > aiScore)
        {
            turnText.text = "You Win!";
        }
        else
        {
            turnText.text = "You Lose!";
        }
    }
}

