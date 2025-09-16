using System.Collections.Generic;
using CardEngine;
using CardEngine.GameEvents;
using CardEngine.RequestEvents;
using UnityEngine;

public class DragCard : MonoBehaviour
{
    private Card card;
    private Transform battlefield;       // Pole, do którego karta ma się przyciągnąć
    private List<GameObject> handCards;       // Pole, do którego karta ma się przyciągnąć
    private float snapDistanceX = 24f; // Maksymalna odległość przyciągania w osi X
    private float snapDistanceZ = 3.5f; // Maksymalna odległość przyciągania w osi Z

    private bool isDragging = false;
    private Camera mainCamera;

    public static GameObject ChosenCard;

    Vector3 originalEuler;
    Vector3 originalScale;
    Vector3 originalPosition;


    void Start()
    {
        //battlefield = BattleManager.battleManager.LocalPlayerBattlefield?.gameObject;
        mainCamera = Camera.main;
    }

    // Należy wywołać po HandCard.Init()
    public void Init()
    {
        HandCard handCard = GetComponent<HandCard>();
        battlefield = handCard.Card.OwnerId == BattleManager.LOCAL_PLAYER_ID ? BattleManager.battleManager.LocalPlayerBattlefield : BattleManager.battleManager.EnemyPlayerBattlefield;
        handCards = handCard.Card.OwnerId == BattleManager.LOCAL_PLAYER_ID ? BattleManager.battleManager.LocalPlayerHandCards : BattleManager.battleManager.EnemyPlayerHandCards;
        card = handCard.Card;
    }
    void OnMouseEnter() //powiększa
    {
        if (BattleManager.battleManager.Game.IsPlayerTurn(card.OwnerId) && ChosenCard == null && isDragging == false && handCards.Contains(this.gameObject))
        {
            ChosenCard = this.gameObject;

            // zapamiętujemy oryginalne wartości
            originalEuler = transform.eulerAngles;
            originalScale = transform.localScale;
            originalPosition = transform.position;

            // powiększamy
            transform.localScale *= 1.4f;

            // przesuwamy
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + 0.1f,
                transform.position.z - 3f
            );

            // ustawiamy rotację na 0 w osi Y
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                0f,
                transform.eulerAngles.z
            );
        }
    }

    void OnMouseExit()  //pomniejsza
    {
        if (ChosenCard == this.gameObject && isDragging == false)
        {
            ChosenCard = null;

            // przywracamy oryginalne wartości
            transform.localScale = originalScale;
            transform.position = originalPosition;
            transform.eulerAngles = originalEuler;
        }
    }

    void OnMouseDown()  //klikniaesz
    {
        if (BattleManager.battleManager.Game.IsPlayerTurn(card.OwnerId) && handCards.Contains(this.gameObject))
        {
            isDragging = true;
            OnMouseExit();
        }
    }

    void OnMouseUp()    //spada
    {

        isDragging = false;

        // Liczymy różnicę w X i Z
        float diffX = Mathf.Abs(transform.position.x - battlefield.position.x);
        float diffZ = Mathf.Abs(transform.position.z - battlefield.position.z);

        // Debug w konsoli ile brakuje
        Debug.Log($"Odległość X: {diffX:F3}, Odległość Z: {diffZ:F3}");

        // Sprawdzamy, czy mieści się w zakresie przyciągania
        if (diffX <= snapDistanceX && diffZ <= snapDistanceZ)
        {
            BattleManager.battleManager.SendRequestEvent(new PutCardOnBattlefieldRequest(card.CardInstanceId));
           // BattleManager.battleManager.PutOnBattlefild(this.gameObject);
        }
        BattleManager.battleManager.ArrangeHand(isLocalPlayer: card.OwnerId == BattleManager.LOCAL_PLAYER_ID);
    }

    void Update()   //ruszać
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                transform.position = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            }
        }
    }
}
