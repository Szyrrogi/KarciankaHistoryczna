using System;
using System.Collections.Generic;
using System.Data;
using CardEngine.Enums;
using CardEngine.GameEvents;
using UnityEngine;
using UnityEngine.TextCore;

namespace CardEngine
{
    public class Card
    {
        public int CardInstanceId { get; internal set; }
        public int CardId { get; internal set; }
        public int OwnerId { get; internal set; }
        public string CardName { get; internal set; }
        public int Cost { get;  internal set; }
        public int Attack { get;  internal set; }
        public int Health { get; internal set; }
        public int Year { get; internal set; }
        public Rarity Rarity { get; internal set; }
        public UnitType UnitType { get; internal set; }
        public Nation Nation { get; internal set; }
        public  bool MovedThisTurn { get; internal set; }
        public Sprite CardSprite { get; internal set; }

        public Card(CardData data, int cardInstanceId, int ownerId)
        {
            CardInstanceId = cardInstanceId;
            OwnerId = ownerId;
            CardName = data.CardName;
            Cost = data.Cost;
            Attack = data.Attack;
            Health = data.Health;
            Year = data.Year;
            Rarity = data.Rarity;
            UnitType = data.UnitType;
            Nation = data.Nation;
            CardSprite = data.CardSprite;

            MovedThisTurn = false;
        }

        internal bool CanAttack(Card defender)
        {
            return !MovedThisTurn; // Tymczasowo zawsze mozna atakowac
        }
    }
}