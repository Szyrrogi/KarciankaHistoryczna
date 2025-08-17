using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public string Name;
    public string Description;
    public int ID;

    public int Attack;
    public int MaxAttack;
    public int Health;
    public int MaxHealth;
    public int Cost;
    public int Year;
    public Sort Sort;
    public List<Effect> Effects;
    public Nation Nation;
    public Rarity Rarity;

    public Image ImageCard;
    public Sprite SpriteCard;

    protected virtual void Start()
    {
        Attack = MaxAttack;
        Health = MaxHealth;
        ImageCard.sprite = SpriteCard;
    }

    protected void Update()
    {
        
    }

    protected virtual void AfterPut()
    {

    }

    public virtual void Agony()
    {

    }

    public virtual void BattleCry()
    {

    }

    public virtual void BeforAttack()
    {

    }

    public virtual void BeforTakeDamage()
    {

    }

    public virtual void TakeDamage()
    {
        if(Health <= 0)
        {
            Agony();
        }
    }

    public virtual void AfterAttack()
    {

    }

    public virtual void AfterTakeDamage()
    {

    }
}

public enum Effect
{
    Concealment,
    Charge,
    Poison,
    Shield,
    Immunity,
    Trap,
    Flanking,
    Provocation,
    Flamethrower,
    AntiTank,
    AntiAir,
    AnimosityTheft,
    WhiteFlag
}

public enum Sort
{
    Infantry,
    Tank,
    Aircraft
}

public enum Nation
{
    Allies,
    Britain,
    Axis,
    Japan,
    Italy,
    Comintern,
}

public enum Rarity
{
    Common,
    Rare,
    Unusual
}
