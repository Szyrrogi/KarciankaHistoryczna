using System;
using System.Collections.Generic;
using System.Data;
using CardEngine.Enums;
using CardEngine.GameEvents;
using UnityEngine.TextCore;

namespace CardEngine
{
    public class Card
    {
        public int CardInstanceId { get; internal set; }
        public int CardId { get; internal set; }
        public string CardName { get; internal set; }
        public int Cost { get;  internal set; }
        public int Attack { get;  internal set; }
        public int Health { get; internal set; }
        public Rarity Rarity { get; internal set; }
        public UnitType UnitType { get; internal set; }
        public Nation Nation { get; internal set; }
        public  bool MovedThisTurn { get; internal set; }

        public Card(CardData data, int cardInstanceId)
        {
            CardInstanceId = cardInstanceId;
            CardId = data.id;
            CardName = data.cardName;
            Cost = data.cost;
            Attack = data.attack;
            Health = data.health;
            Rarity = data.rarity;
            UnitType = data.unitType;
            Nation = data.nation;

            MovedThisTurn = false;
        }

        internal bool CanAttack(Card defender)
        {
            return !MovedThisTurn; // Tymczasowo zawsze mozna atakowac
        }
    }
}