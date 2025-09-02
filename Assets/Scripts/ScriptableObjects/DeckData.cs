using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDeck", menuName = "DeckData")]
public class DeckData : ScriptableObject
{
    public int id;
    public List<CardData> cards;
}
