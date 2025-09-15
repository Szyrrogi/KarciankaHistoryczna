using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardEngine.Enums;
using CardEngine;
using TMPro;
public class HandCard : MonoBehaviour
{

    public Card Card;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI CostText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI YearText;
    public Image CardImage;

    public int InstanceId { get; private set; }


    public void Init(Card data)
    {
        Card = data;
        InstanceId = Card.CardInstanceId;

        HealthText.text = Card.Health.ToString();
        AttackText.text = Card.Attack.ToString();
        CostText.text = Card.Cost.ToString();
        YearText.text = Card.Year.ToString();
        NameText.text = Card.CardName;
        CardImage.sprite = Card.CardSprite;
    }
    protected virtual void Start()
    {
    }
    protected void Update()
    {
        
    }
}
