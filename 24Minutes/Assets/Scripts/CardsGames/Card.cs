using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage;
    public Sprite cardBack; 
    public Sprite cardFront; 

    public CardData data; 
    public bool isRevealed = false;
    
    public void SetupCard(CardData cardData, Sprite frontImage)
    {
        data = cardData;
        cardFront = frontImage;
        cardImage.sprite = cardBack;
    }

    public void RevealCard()
    {
        if (isRevealed) return;

        isRevealed = true;
        cardImage.sprite = cardFront;
        
        CardGameManager.Instance.OnCardClicked(this);
    }

    public void HideCard()
    {
        isRevealed = false;
        cardImage.sprite = cardBack;
    }
}