using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public string Type { get; private set; }
    public int Value { get; private set; }

    public CardData(string type, int value)
    {
        Type = type;
        Value = value;
    }

    public Sprite GetSprite()
    {
        // Implement logic to return the appropriate sprite based on type
        return Resources.Load<Sprite>($"Cards/{Type}");
    }
}

