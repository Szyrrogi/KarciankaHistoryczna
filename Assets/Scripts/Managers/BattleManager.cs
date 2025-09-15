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

    [Header("Local Player Card Objects")]
    public Transform LocalPlayerDeck;
    public Transform LocalPlayerHand;
    public Transform LocalPlayerBattlefield;
    public Transform LocalPlayerGraveyard;

    [Header("Enemy Player Card Objects")]
    public Transform EnemyPlayerDeck;
    public Transform EnemyPlayerHand;
    public Transform EnemyPlayerBattlefield;
    public Transform EnemyPlayerGraveyard;


    [Header("Prefabs")]
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

        LocalPlayerHandCards = new List<GameObject>();
        LocalPlayerGraveyardCards = new List<GameObject>();
        LocalPlayerBattlefieldCards = new List<GameObject>();

        EnemyPlayerHandCards = new List<GameObject>();
        EnemyPlayerGraveyardCards = new List<GameObject>();
        EnemyPlayerBattlefieldCards = new List<GameObject>();

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
            Debug.Log($"Processing event: {evt.GetType()}");
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
        
        // Local Player
        if (putCard.PlayerId == LOCAL_PLAYER_ID)
        {
            GameObject cardToPlay = LocalPlayerHandCards.Find(card => card.GetComponent<HandCard>().InstanceId == putCard.InstanceCardId);
            if (cardToPlay != null)
            {
                LocalPlayerHandCards.Remove(cardToPlay);

                GameObject battlefieldCardObject = Instantiate(BattlefieldCardPrefab, LocalPlayerBattlefield.position, Quaternion.identity, LocalPlayerBattlefield);
                battlefieldCardObject.GetComponent<BattlefieldCard>().Init(_game.GetCardByInstanceId(putCard.InstanceCardId));

                LocalPlayerBattlefieldCards.Add(battlefieldCardObject);

                Destroy(cardToPlay);

                ArrangeLocalPlayerHand();
                ArrangeLocalPlayerField();
            }
            else
            {
                Debug.LogWarning($"Card with InstanceId {putCard.InstanceCardId} not found in hand.");
            }
        }
        // Enemy Player
        else
        {
            GameObject cardToPlay = EnemyPlayerHandCards.Find(card => card.GetComponent<HandCard>().InstanceId == putCard.InstanceCardId);
            if (cardToPlay != null)
            {
                EnemyPlayerHandCards.Remove(cardToPlay);

                GameObject battlefieldCardObject = Instantiate(BattlefieldCardPrefab, EnemyPlayerBattlefield.position, Quaternion.identity, EnemyPlayerBattlefield);
                battlefieldCardObject.GetComponent<BattlefieldCard>().Init(_game.GetCardByInstanceId(putCard.InstanceCardId));

                EnemyPlayerBattlefieldCards.Add(battlefieldCardObject);

                Destroy(cardToPlay);

                ArrangeLocalPlayerHand();
                ArrangeLocalPlayerField();
            }
            else
            {
                Debug.LogWarning($"Card with InstanceId {putCard.InstanceCardId} not found in hand.");
            }
        }
    }

    private void ProcessTurnEndEvent(TurnEndEvent turnEnd)
    {
        throw new NotImplementedException();
    }

    private void ProcessCardUpdatedEvent(CardUpdatedEvent update)
    {
        GameObject cardToUpdate = null;

        if (update.PlayerId == LOCAL_PLAYER_ID)
        {
            cardToUpdate = LocalPlayerBattlefieldCards.Find(card => card.GetComponent<BattlefieldCard>().Card.CardInstanceId == update.InstanceCardId); //Mozna pomyslec nad slownikami
        }
        else
        {
            cardToUpdate = EnemyPlayerBattlefieldCards.Find(card => card.GetComponent<BattlefieldCard>().Card.CardInstanceId == update.InstanceCardId);
        }

        if (cardToUpdate != null)
        {
            cardToUpdate.GetComponent<BattlefieldCard>().UpdateCardUI();
        }
        else
        {
            Debug.LogWarning($"Card to update not found for InstanceId {update.InstanceCardId}");
        }
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
            Vector3 pos = LocalPlayerDeck.position;
            Quaternion rot = Quaternion.identity;

            GameObject handCardObject = Instantiate(HandCardPrefab, pos, rot, LocalPlayerDeck);
            handCardObject.GetComponent<HandCard>().Init(_game.GetCardByInstanceId(draw.InstanceCardId));

            LocalPlayerHandCards.Add(handCardObject);

            StartCoroutine(DrawLocalPlayerCardAnimation(handCardObject));
        }
        else
        {
            Vector3 pos = EnemyPlayerDeck.position;
            Quaternion rot = Quaternion.identity;

            GameObject handCardObject = Instantiate(HandCardPrefab, pos, rot, EnemyPlayerDeck);
            handCardObject.GetComponent<HandCard>().Init(_game.GetCardByInstanceId(draw.InstanceCardId));

            EnemyPlayerHandCards.Add(handCardObject);

            StartCoroutine(DrawEnemyCardAnimation(handCardObject));
        }
    }

    private void ProcessAttackEvent(AttackEvent attack)
    {
        // Na razie nic nie robimy, zmiana życia itd jest obsługiwana w ProcessCardUpdatedEvent
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




    public IEnumerator DrawEnemyCardAnimation(GameObject card)
    {
        yield return DrawCardAnimation(card, EnemyPlayerDeck.position, EnemyPlayerHand.position);
        ArrangeEnemyHand();
    }
    public IEnumerator DrawLocalPlayerCardAnimation(GameObject card)
    {
        yield return DrawCardAnimation(card, LocalPlayerDeck.position, LocalPlayerHand.position);
        ArrangeLocalPlayerHand();
    }
    public IEnumerator DrawCardAnimation(GameObject card, Vector3 startPos, Vector3 endPos)
    {
        // Animacja przejścia do ręki`
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
    }

    public void ArrangeLocalPlayerField()
    {
        ArrangeField(isLocalPlayer: true);
    }

    public void ArrangeLEnemyField()
    {
        ArrangeField(isLocalPlayer: false);
    }

    public void ArrangeField(bool isLocalPlayer)
    {
        List<GameObject> BattlefieldCards = isLocalPlayer ? this.LocalPlayerBattlefieldCards : this.EnemyPlayerBattlefieldCards;
        Transform Battlefield = isLocalPlayer ? this.LocalPlayerBattlefield : this.EnemyPlayerBattlefield;
        int cardCount = BattlefieldCards.Count;
        if (cardCount == 0) return;

        int i = 0;
        foreach (GameObject card in BattlefieldCards)
        {
            float posX = LocalPlayerBattlefield.position.x - (i - (cardCount - 1) / 2f) * 8f;
            Vector3 pos = new Vector3(posX, Battlefield.position.y, Battlefield.position.z);

            card.transform.position = pos;
            i++;
        }
    }




    public void ArrangeLocalPlayerHand()
    {
        ArrangeHand(isLocalPlayer: true);
    }
    public void ArrangeEnemyHand()
    {
        ArrangeHand(isLocalPlayer: false);
    }

    public void ArrangeHand(bool isLocalPlayer)
    {
        List<GameObject> handCards = isLocalPlayer ? LocalPlayerHandCards : EnemyPlayerHandCards;
        Transform handRoot = isLocalPlayer ? LocalPlayerHand : EnemyPlayerHand;

        int count = handCards.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            float offset = i - (count - 1) / 2f;

            // Lokalny: posX = root.x - offset * fanRadius
            // Wróg   : posX = root.x + offset * fanRadius  (odwrotna strona wachlarza)
            float posX = handRoot.position.x + (isLocalPlayer ? -offset : offset) * fanRadius;
            float posZ = handRoot.position.z + Mathf.Abs(offset) * 0.3f;

            // Rotacja odwrotna dla wroga
            float rotY = handRoot.rotation.eulerAngles.y + (isLocalPlayer ? offset : -offset) * fanAngle;

            Vector3 pos = new Vector3(posX, handRoot.position.y + i * 0.05f, posZ);
            Quaternion rot = Quaternion.Euler(
                handRoot.rotation.eulerAngles.x,
                rotY,
                handRoot.rotation.eulerAngles.z
            );

            handCards[i].transform.SetPositionAndRotation(pos, rot);
        }

        // private void ArrangeHand(bool isLocalPlayer)
        // {
        //     List<GameObject> handCards = isLocalPlayer ? LocalPlayerHandCards : EnemyPlayerHandCards;

        //     int cardCount = handCards.Count;
        //     if (cardCount == 0) return;

        //     int i = 0;
        //     foreach (GameObject card in handCards)
        //     {
        //         float offsetFromCenter = i - (cardCount - 1) / 2f;

        //         float posX = LocalPlayerHand.position.x - (i - (cardCount - 1) / 2f) * fanRadius;
        //         //float posZ = HandCenter.position.z - (i - (cardCount - 1) / 2f) * 0.3f;
        //         float posZ = LocalPlayerHand.position.z + Mathf.Abs(offsetFromCenter) * 0.3f;
        //         float rotationY = LocalPlayerHand.rotation.eulerAngles.y + (i - (cardCount - 1) / 2f) * fanAngle;

        //         Vector3 pos = new Vector3(posX, LocalPlayerHand.position.y + i * 0.05f, posZ);
        //         Vector3 rot = new Vector3(LocalPlayerHand.rotation.eulerAngles.x, rotationY, LocalPlayerHand.rotation.eulerAngles.z);

        //         card.transform.position = pos;
        //         card.transform.rotation = Quaternion.Euler(rot); // <-- zamiana na Quaternion
        //         i++;

        //     }
        // }
    }
}