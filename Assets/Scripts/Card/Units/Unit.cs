using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Unit : Card
{
    public int health;

    public int attack;

    TextMeshProUGUI HealthText;
    TextMeshProUGUI AttackText;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        HealthText = CardUI.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        AttackText = CardUI.transform.Find("AttackText").GetComponent<TextMeshProUGUI>();

        HealthText.text = health.ToString();
        AttackText.text = attack.ToString();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
}
