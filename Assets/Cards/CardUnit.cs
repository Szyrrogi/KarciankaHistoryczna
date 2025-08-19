using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardUnit : Card
{
    public GameObject CardToPlay;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;

    private bool Action;

    public LineRenderer arrowPrefab; // Prefab strzałki
    private LineRenderer currentArrow;

    private static CardUnit selectedCard; // Aktualnie wybrana karta (moja)
    private Camera cam;
    


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

    
    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        if (!Enemy) // Kliknięcie mojej karty
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
}
