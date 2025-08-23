using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager battleManager;

    public List<GameObject> Deck; 
    public List<GameObject> DeckEnemy;
    public List<GameObject> Hand; 
    public List<GameObject> HandEnemy; 
    public List<GameObject> Graveyard; 
    public List<GameObject> GraveyardEnemy;
    public List<GameObject> UsedCard; 
    public List<GameObject> UsedCardEnemy;
    public List<GameObject> Battle; 
    public List<GameObject> BattleEnemy;
    

    public Transform DeckPosition; // Pozycja stosu talii
    public Transform HandCenter;   // Punkt środkowy wachlarza w ręce
    public Transform Battlefild;

    public GameObject CardPrefab;

    private float drawDuration = 0.5f; // czas animacji dobrania karty
    private float fanRadius = 5f;      // promień wachlarza
    private float fanAngle = 3f;      // maksymalny kąt wachlarza

    void Awake()
    {
        battleManager = this;
    }

    void Start()
    {
        // Tworzenie przykładowej talii
        for(int i = 0; i < 10; i++) 
        { 
            Vector3 newPosition = new Vector3( 
                DeckPosition.position.x - 0.02f * i, 
                DeckPosition.position.y + 0.02f * i, 
                DeckPosition.position.z ); 

            Quaternion rotation = Quaternion.Euler(0, 0, 180);
            
            GameObject newCard = Instantiate(CardPrefab, newPosition, rotation, DeckPosition); 
            Deck.Add(newCard); 
        }

        // Dobieramy na start np. 5 kart
        StartCoroutine(DrawMultipleCards(5));
    }

    public void NextTurn()
    {
        StartCoroutine(DrawMultipleCards(1));
        foreach(GameObject card in Battle)
        {
            card.GetComponent<CardUnit>().NextTurn();
        }
    }

    public void PutOnBattlefild(GameObject card)
    {
        DragCard.ChosenCard = null;
        UsedCard.Add(card);
        Hand.Remove(card);
        //card.transform.position = new Vector3(10000f,0,0); NIE DZIAŁA
        GameObject newUnit = Instantiate(card.GetComponent<CardToPlay>().CardUnit, Battlefild.position, Quaternion.identity); 
        Battle.Add(newUnit);
        CopyStats(card.GetComponent<Card>(), newUnit.GetComponent<Card>());
        card.SetActive(false);  //ZASTĘPSTWO
        ArrangeHand();
        ArrangeFild();
    }

    void CopyStats(Card Old, Card New)
    {
        New.Health = Old.Health;
        New.MaxHealth = Old.MaxHealth;
        New.Attack = Old.Attack;
        New.MaxAttack = Old.MaxAttack;
        New.SpriteCard = Old.SpriteCard;
        New.Enemy = Old.Enemy;
    }

    public IEnumerator DrawMultipleCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return StartCoroutine(DrawCard());
        }
    }

    public IEnumerator DrawCard()
    {
        if (Deck.Count == 0) yield break;

        GameObject card = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(card);

        // Animacja przejścia do ręki
        Vector3 startPos = card.transform.position;
        Vector3 endPos = HandCenter.position;
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
        int cardCount = Battle.Count;
        if (cardCount == 0) return;

        int i = 0;
        foreach(GameObject card in Battle)
        {
            float posX = Battlefild.position.x - (i - (cardCount - 1) / 2f) * 8f;
            Vector3 pos = new Vector3(posX, Battlefild.position.y, Battlefild.position.z);

            card.transform.position = pos;
            i++;
        }
    }

    public void ArrangeHand()
    {
        int cardCount = Hand.Count;
        if (cardCount == 0) return;

        int i = 0;
        foreach(GameObject card in Hand)
        {
            float offsetFromCenter = i - (cardCount - 1) / 2f;

            float posX = HandCenter.position.x - (i - (cardCount - 1) / 2f) * fanRadius;
            //float posZ = HandCenter.position.z - (i - (cardCount - 1) / 2f) * 0.3f;
            float posZ = HandCenter.position.z + Mathf.Abs(offsetFromCenter) * 0.3f; 
            float rotationY = HandCenter.rotation.eulerAngles.y + (i - (cardCount - 1) / 2f) * fanAngle;

            Vector3 pos = new Vector3(posX, HandCenter.position.y + i * 0.05f, posZ);
            Vector3 rot = new Vector3(HandCenter.rotation.eulerAngles.x, rotationY, HandCenter.rotation.eulerAngles.z);

            card.transform.position = pos;
            card.transform.rotation = Quaternion.Euler(rot); // <-- zamiana na Quaternion
            i++;

        }
    }
}
