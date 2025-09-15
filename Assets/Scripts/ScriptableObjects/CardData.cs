using UnityEngine;
using CardEngine.Enums;

[CreateAssetMenu(fileName = "NewCard", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public int Id;
    public string CardName;
    public int Cost;
    public int Attack;
    public int Health;
    public int Year;
    public Rarity Rarity;
    public UnitType UnitType;
    public Nation Nation;
    public Sprite CardSprite;
}
