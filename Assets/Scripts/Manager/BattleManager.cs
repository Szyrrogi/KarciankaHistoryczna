using System;
using System.Collections;
using System.Collections.Generic;
using CardEngine;
using CardEngine.GameEvents;
using CardEngine.RequestEvents;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BattleManager : MonoBehaviour
{
    public const int LOCAL_PLAYER_ID = 1;
    public const int ENEMY_PLAYER_ID = 2;


    public static BattleManager battleManager;

    private Game _game;

    [HideInInspector]
    public List<GameObject> LocalPlayerHandCards; 
    [HideInInspector]
    public List<GameObject> LocalPlayerGraveyardCards;
    [HideInInspector]
    public List<GameObject> LocalPlayerBattlefieldCards; 

    [HideInInspector]
    public List<GameObject> EnemyPlayerHandCards;
    [HideInInspector]
    public List<GameObject> EnemyPlayerGraveyardCards;
    [HideInInspector]
    public List<GameObject> EnemyPlayerBattlefieldCards;


    [Header("Local Player Card Positions")]
    public Transform LocalPlayerDeckPosition; // Pozycja stosu talii
    public Transform LocalPlayerHandCenter;   // Punkt środkowy wachlarza w ręce
    public Transform LocalPlayerBattlefield;
    public Transform LocalPlayerGraveyardPosition;
    public Transform LocalPlayerToDrawPosition;


    [Header("Enemy Player Card Positions")]
    public Transform EnemyPlayerDeckPosition; // Pozycja stosu talii
    public Transform EnemyPlayerHandCenter;   // Punkt środkowy wachlarza w ręce
    public Transform EnemyPlayerBattlefield;
    public Transform EnemyPlayerGraveyardPosition;
    public Transform EnemyPlayerToDrawPosition;


    public GameObject BattlefieldCardPrefab;
    public GameObject HandCardPrefab;

    [Header("Deck Data")]
    public DeckData localPlayerDeck;
    public DeckData enemyPlayerDeck;


    private float drawDuration = 0.5f; // czas animacji dobrania karty
    private float fanRadius = 5f;      // promień wachlarza
    private float fanAngle = 3f;      // maksymalny kąt wachlarza

    void Awake()
    {
        battleManager = this;
        _game = new Game(localPlayerDeck, enemyPlayerDeck);
    }

    void Start()
    {
        ProcessEvents(_game.HandleRequest(new StartGameRequest()));
    }


    public void SendRequestEvent(RequestEvent request)
    {
        List<GameEvent> events = _game.HandleRequest(request);
        ProcessEvents(events);
    }

    private void ProcessEvents(List<GameEvent> events)
    {
        foreach (GameEvent evt in events)
        {
            switch (evt)
            {
                case AttackEvent attack:
                    ProcessAttackEvent(attack);
                    break;

                case DrawCardEvent draw:
                    ProcessDrawCardEvent(draw);
                    break;

                case PlayerWonEvent win:
                    ProcessPlayerWonEvent(win);
                    break;

                case CardDestroyedEvent destroy:
                    ProcessCardDestroyedEvent(destroy);
                    break;

                case CardUpdatedEvent update:
                    ProcessCardUpdatedEvent(update);
                    break;

                case TurnEndEvent turnEnd:
                    ProcessTurnEndEvent(turnEnd);
                    break;

                case PutCardOnBattlefieldEvent putCard:
                    ProcessPutCardOnBattlefieldEvent(putCard);
                    break;

                default:
                    UnityEngine.Debug.LogWarning($"Unhandled event type: {evt.GetType()}");
                    break;
            }
        }
    }

    private void ProcessPutCardOnBattlefieldEvent(PutCardOnBattlefieldEvent putCard)
    {
        throw new NotImplementedException();
    }

    private void ProcessTurnEndEvent(TurnEndEvent turnEnd)
    {
        throw new NotImplementedException();
    }

    private void ProcessCardUpdatedEvent(CardUpdatedEvent update)
    {
        throw new NotImplementedException();
    }

    private void ProcessCardDestroyedEvent(CardDestroyedEvent destroy)
    {
        throw new NotImplementedException();
    }

    private void ProcessPlayerWonEvent(PlayerWonEvent win)
    {
        throw new NotImplementedException();
    }

    private void ProcessDrawCardEvent(DrawCardEvent draw) 
    {
        if (draw.PlayerId == LOCAL_PLAYER_ID)
        {
            Vector3 pos = LocalPlayerDeckPosition.position;
            Quaternion rot = Quaternion.identity;

            GameObject handCardObject = Instantiate(HandCardPrefab, pos, rot, LocalPlayerToDrawPosition);
            handCardObject.GetComponent<HandCard>().Init(_game.GetCardByInstanceId(draw.InstanceCardId));

            StartCoroutine(DrawCardAnimation(handCardObject));
        }
    }

    private void ProcessAttackEvent(AttackEvent attack)
    {
        throw new NotImplementedException();
    }



    // public void NextTurn()
    // {
    //     StartCoroutine(DrawMultipleCards(1));
    //     foreach (GameObject card in LocalPlayerBattlefieldCards)
    //     {
    //         card.GetComponent<CardUnit>().NextTurn();
    //     }
    // }

    // public void PutOnBattlefild(GameObject card)
    // {
    //     DragCard.ChosenCard = null;
    //     LocalPlayerUsedCards.Add(card);
    //     LocalPlayerHandCards.Remove(card);
    //     //card.transform.position = new Vector3(10000f,0,0); NIE DZIAŁA
    //     GameObject newUnit = Instantiate(card.GetComponent<CardToPlay>().CardUnit, LocalPlayerBattlefield.position, Quaternion.identity);
    //     LocalPlayerBattlefieldCards.Add(newUnit);
    //     CopyStats(card.GetComponent<Card>(), newUnit.GetComponent<Card>());
    //     card.SetActive(false);  //ZASTĘPSTWO
    //     ArrangeHand();
    //     ArrangeFild();
    // }

    public IEnumerator DrawCardAnimation(GameObject card)
    {

        // Animacja przejścia do ręki
        Vector3 startPos = card.transform.position;
        Vector3 endPos = LocalPlayerHandCenter.position;
        Quaternion startRot = card.transform.rotation;
        Quaternion endRot = Quaternion.identity;

        float t = 0;
        while (t < drawDuration)
        {
            t += Time.deltaTime;
            float lerp = t / drawDuration;

            card.transform.position = Vector3.Lerp(startPos, endPos, lerp);
            card.transform.rotation = Quaternion.Lerp(startRot, endRot, lerp);
            yield return null;
        }

        ArrangeHand();
    }

    public void ArrangeFild()
    {
        int cardCount = LocalPlayerBattlefieldCards.Count;
        if (cardCount == 0) return;

        int i = 0;
        foreach(GameObject card in LocalPlayerBattlefieldCards)
        {
            float posX = LocalPlayerBattlefield.position.x - (i - (cardCount - 1) / 2f) * 8f;
            Vector3 pos = new Vector3(posX, LocalPlayerBattlefield.position.y, LocalPlayerBattlefield.position.z);

            card.transform.position = pos;
            i++;
        }
    }

    public void ArrangeHand()
    {
        int cardCount = LocalPlayerHandCards.Count;
        if (cardCount == 0) return;

        int i = 0;
        foreach(GameObject card in LocalPlayerHandCards)
        {
            float offsetFromCenter = i - (cardCount - 1) / 2f;

            float posX = LocalPlayerHandCenter.position.x - (i - (cardCount - 1) / 2f) * fanRadius;
            //float posZ = HandCenter.position.z - (i - (cardCount - 1) / 2f) * 0.3f;
            float posZ = LocalPlayerHandCenter.position.z + Mathf.Abs(offsetFromCenter) * 0.3f;
            float rotationY = LocalPlayerHandCenter.rotation.eulerAngles.y + (i - (cardCount - 1) / 2f) * fanAngle;

            Vector3 pos = new Vector3(posX, LocalPlayerHandCenter.position.y + i * 0.05f, posZ);
            Vector3 rot = new Vector3(LocalPlayerHandCenter.rotation.eulerAngles.x, rotationY, LocalPlayerHandCenter.rotation.eulerAngles.z);

            card.transform.position = pos;
            card.transform.rotation = Quaternion.Euler(rot); // <-- zamiana na Quaternion
            i++;

        }
    }
}
