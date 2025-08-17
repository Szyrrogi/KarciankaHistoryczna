using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardUnit : Card
{
    public GameObject CardToPlay;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;


    void Update()
    {
        HealthText.text = Health.ToString();
        AttackText.text = Attack.ToString();

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
