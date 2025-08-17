using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardToPlay : Card
{
    public GameObject CardUnit;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI CostText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI YearText;

    void Update()
    {
        HealthText.text = Health.ToString();
        AttackText.text = Attack.ToString();
        CostText.text = Cost.ToString();
        YearText.text = Year.ToString();
        NameText.text = Name;

        if(Health < MaxHealth)
            HealthText.color = Color.red;
        if(Health > MaxHealth)
            HealthText.color = Color.green;
        if(Health == MaxHealth)
            HealthText.color = Color.white;
        if(Attack < MaxAttack)
            AttackText.color = Color.red;
        if(Attack > MaxAttack)
            AttackText.color = Color.green;
        if(Attack == MaxAttack)
            AttackText.color = Color.white;

        base.Update();
    }
}
