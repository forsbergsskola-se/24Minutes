using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardBack, cardFront;
    private CardData cardData;
    private System.Action<Card> onClickCallback;
    public bool IsRevealed { get; private set; }

    public CardData CardData => cardData;

    public void Setup(CardData data, System.Action<Card> callback)
    {
        cardData = data;
        onClickCallback = callback;
        cardFront.sprite = cardData.GetSprite();
        IsRevealed = false;
        UpdateView();
    }

    public void Reveal()
    {
        IsRevealed = true;
        UpdateView();
    }

    void UpdateView()
    {
        cardBack.gameObject.SetActive(!IsRevealed);
        cardFront.gameObject.SetActive(IsRevealed);
    }

    void OnMouseDown()
    {
        if (!IsRevealed) onClickCallback?.Invoke(this);
    }
}