using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardUnit : Card
{
    public GameObject CardToPlay;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;

    public GameObject ActionObject;

    public LineRenderer arrowPrefab; // Prefab strzałki
    private LineRenderer currentArrow;

    private static CardUnit selectedCard; // Aktualnie wybrana karta (moja)
    private Camera cam;
    
    public Transform parentObject;


    void Update()
    {
        ActionObject.SetActive(Action);
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

    
    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        if (!Enemy && Action) // Kliknięcie mojej karty
        {
            selectedCard = this;

            // Tworzymy strzałkę
            currentArrow = Instantiate(arrowPrefab);
            currentArrow.positionCount = 2;
            currentArrow.SetPosition(0, transform.position); // Start strzałki = karta
        }
    }

    void OnMouseDrag()
    {
        if (currentArrow != null)
        {
            // Rzut raycasta na płaszczyznę XZ
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                currentArrow.SetPosition(1, point);
            }
        }
    }

    void OnMouseUp()
    {
        if (currentArrow != null && selectedCard == this)
        {
            // Sprawdzamy, czy puściliśmy nad przeciwnikiem
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CardUnit enemyCard = hit.collider.GetComponent<CardUnit>();
                if (enemyCard != null && enemyCard.Enemy)
                {
                    enemyCard.BeforTakeDamage(this); // Wywołanie ataku
                }
            }

            // Usuwamy strzałkę i reset
            Destroy(currentArrow.gameObject);
            selectedCard = null;
        }
    }

    public virtual void NextTurn()
    {
        Action = true;
    }

    public virtual void Agony()
    {
        gameObject.SetActive(false);
    }   

    public virtual void BattleCry()
    {

    }

    public virtual void BeforAttack(CardUnit attacker)
    {
        Action = false;
        attacker.TakeDamage(this, Attack);
        TakeDamage(attacker, attacker.Attack);
    }

    public virtual void BeforTakeDamage(CardUnit attacker)
    {
        Debug.Log($"{attacker.name} atakuje {name}");
        attacker.BeforAttack(this);
    }

    public virtual void TakeDamage(CardUnit attacker, int Attack)
    {
        GameObject newObj = Instantiate(PopUpText, parentObject);
        newObj.GetComponent<TextMeshProUGUI>().text = Attack.ToString();

        Health -= Attack;
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
