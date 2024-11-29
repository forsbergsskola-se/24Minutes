using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { Diamond, Treasure, Trap1, Trap2, Trap3, Trap4 }

[System.Serializable]
public class CardData
{
    public CardType type;
    public int value; // Diamantes y Tesoro tienen valores; las Trampas tienen valor 0.
}


