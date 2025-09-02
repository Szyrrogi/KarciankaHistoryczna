using UnityEngine;
using CardEngine.Enums;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public int id;
    public string cardName;
    public int cost;
    public int attack;
    public int health;
    public Rarity rarity;
    public UnitType unitType;
    public Nation nation;
}
