using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardEngine.Enums;
using CardEngine;
using TMPro;
using Photon.Pun.Demo.PunBasics;
using CardEngine.RequestEvents;
public class BattlefieldCard : MonoBehaviour
{

    public Card Card;

    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI AttackText;
    public Image CardImage;
    public GameObject ActionObject;

    public LineRenderer arrowPrefab; // Prefab strzałki
    private LineRenderer currentArrow;

    private static BattlefieldCard selectedCard; // Aktualnie wybrana karta (moja)
    private Camera cam;
    
    public Transform parentObject;

    public void Init(Card data)
    {
        Card = data;

        UpdateCardUI();
    }
    protected virtual void Start()
    {
        cam = Camera.main;

    }
    protected void Update()
    {

    }
    
    public void UpdateCardUI()
    {

        HealthText.text = Card.Health.ToString();
        AttackText.text = Card.Attack.ToString();
        CardImage.sprite = Card.CardSprite;
    }

    void OnMouseDown()
    {
        if (Card.OwnerId == BattleManager.LOCAL_PLAYER_ID) // Kliknięcie mojej karty
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
                BattlefieldCard enemyCard = hit.collider.GetComponent<BattlefieldCard>();
                if (enemyCard != null && enemyCard.Card.OwnerId != Card.OwnerId) // Upewniamy się, że to karta przeciwnika
                {
                    // Wysyłamy żądanie ataku do BattleManager
                    BattleManager.battleManager.SendRequestEvent(new AttackUnitRequest(Card.CardInstanceId, enemyCard.Card.CardInstanceId));
                }
            }

            // Usuwamy strzałkę i reset
            Destroy(currentArrow.gameObject);
            selectedCard = null;
        }
    }

}
