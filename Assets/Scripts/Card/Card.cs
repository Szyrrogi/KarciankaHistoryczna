using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public int cost;
    public string cardName;

    protected Canvas CardUI;

    protected TextMeshProUGUI CostText;
    protected TextMeshProUGUI NameText;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        CardUI = transform.Find("CardUI").GetComponent<Canvas>();


        CostText = CardUI.transform.Find("CostText").GetComponent<TextMeshProUGUI>();
        NameText = CardUI.transform.Find("NameText").GetComponent<TextMeshProUGUI>();


        CostText.text = cost.ToString();
        NameText.text = cardName;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
