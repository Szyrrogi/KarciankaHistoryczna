using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardEngine.Enums;
using CardEngine;
using TMPro;
public class HandCard : MonoBehaviour
{

    private Card _card;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI CostText;
    public TextMeshProUGUI NameText;

    public TextMeshProUGUI YearText;
    public Image CardImage;

    public void Init(Card data)
    {
        _card = data;

    Debug.Log($"Card Name: '{_card.CardName}'");
    Debug.Log($"Health: {_card.Health}");
    Debug.Log($"Attack: {_card.Attack}");
    Debug.Log($"Cost: {_card.Cost}");
    Debug.Log($"Year: {_card.Year}");
    Debug.Log($"Sprite: {_card.CardSprite?.name}");

        HealthText.text = _card.Health.ToString();
        AttackText.text = _card.Attack.ToString();
        CostText.text = _card.Cost.ToString();
        YearText.text = _card.Year.ToString();
        NameText.text = _card.CardName;
        CardImage.sprite = _card.CardSprite;
    }
    protected virtual void Start()
    {
    }
    protected void Update()
    {
        
    }

    protected virtual void AfterPut()
    {

    }


}
