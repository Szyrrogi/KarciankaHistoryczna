using System;
using System.Collections.Generic;
using CardEngine.GameEvents;
using CardEngine.RequestEvents;
using CardEngine.HelperClassess;
using UnityEditor.ShortcutManagement;

namespace CardEngine
{
    public class Game
    {
        private const int MIN_DECK_SIZE = 10; // Na razie nie użyte
        private const int MAX_DECK_SIZE = 30; // Na razie nie użyte
        private const int INITIAL_HAND_SIZE = 5;
        private const int MAX_CARDS_ON_BATTLEFIELD = 10; // Na razie nie użyte
        private const int FIRST_PLAYER_ID = 1;
        private const int SECOND_PLAYER_ID = 2;

        Player firstPlayer { get; set; }
        Player secondPlayer { get; set; }
        int turnCounter { get; set; }
        bool isFirstPlayerTurn { get; set; }
        bool hasFirstPlayerWon { get; set; }
        bool isGameActive { get; set; }
        bool isGameOver { get; set; }
        int lastCardInstanceId { get; set; }


        public Game(DeckData firstDeck, DeckData secondDeck)
        {
            lastCardInstanceId = 0;

            // Initialize players with decks
            List<Card> firstPlayerDeck = createDeck(firstDeck, FIRST_PLAYER_ID);
            firstPlayer = new Player
            {
                Id = FIRST_PLAYER_ID,
                Deck = firstPlayerDeck,
                ToDraw = new List<Card>(firstPlayerDeck),
                Hand = new List<Card>(),
                Battlefield = new List<Card>(),
                Graveyard = new List<Card>()
            };

            List<Card> secondPlayerDeck = createDeck(secondDeck, SECOND_PLAYER_ID);
            secondPlayer = new Player
            {
                Id = SECOND_PLAYER_ID,
                Deck = secondPlayerDeck,
                ToDraw = new List<Card>(secondPlayerDeck),
                Hand = new List<Card>(),
                Battlefield = new List<Card>(),
                Graveyard = new List<Card>()
            };

            turnCounter = 1;
            isFirstPlayerTurn = true;
            isGameOver = false;
            hasFirstPlayerWon = false;
            isGameActive = false;

        }

        // Ta sekcja odpowiada za obsługę żądań od graczy i ich walidacje
        #region Requests
        public List<GameEvent> HandleRequest(RequestEvent request)
        {
            return request switch
            {
                StartGameRequest r => ProcessStartGameRequest(),
                AttackUnitRequest r => ProcessAttackUnitRequest(r.attackerInstanceId, r.defenderInstanceId),
                EndTurnRequest r => ProcessEndTurnRequest(r.PlayerId),
                PutCardOnBattlefieldRequest r => ProcessPutCardOnBattlefieldRequest(r.InstanceCardId),
                _ => new List<GameEvent>()
            };
        }
        private List<GameEvent>ProcessStartGameRequest()
        {
            List<GameEvent>events = new List<GameEvent>();

            if (isGameActive || isGameOver)
            {
                UnityEngine.Debug.LogWarning("Game can't be started again");
                return events;
            }

            events.AddRange(StartGame());
            return events;
        }


        // Sprawdza, czy jednostka może zaatakować. Jeśli tak, wywoływana jest metoda Attack
        public List<GameEvent>ProcessAttackUnitRequest(int attackerInstanceId, int defenderInstanceId)
        {
            List<GameEvent>events = new List<GameEvent>();

            if (!isGameActive)
            {
                UnityEngine.Debug.LogWarning("Game is inactive");
                return events;
            }

            Player attackingPlayer = isFirstPlayerTurn ? firstPlayer : secondPlayer;
            Player defendingPlayer = isFirstPlayerTurn ? secondPlayer : firstPlayer;

            Card attackingCard = getCardByInstanceId(attackerInstanceId, attackingPlayer);
            Card defendingCard = getCardByInstanceId(defenderInstanceId, defendingPlayer);

            if (attackingCard == null)
            {
                UnityEngine.Debug.LogWarning("Attacking card not found on battlefield");
                return events;
            }

            if (defendingCard == null)
            {
                UnityEngine.Debug.LogWarning("Defending card not found on battlefield");
                return events;
            }

            events.AddRange(AttackCard(attackingCard, defendingCard));

            return events;
        }

        public List<GameEvent>ProcessEndTurnRequest(int playerId)
        {
            List<GameEvent>events = new List<GameEvent>();

            if (!isGameActive)
            {
                UnityEngine.Debug.LogWarning("Game is inactive");
                return events;
            }

            Player currentPlayer = GetCurrentPlayer();
            if (currentPlayer.Id != playerId)
            {
                UnityEngine.Debug.LogWarning("It's not the player's turn");
                return events;
            }

            events.AddRange(EndTurn());

            return events;
        }

        public List<GameEvent>ProcessPutCardOnBattlefieldRequest(int cardInstanceId)
        {
            List<GameEvent>events = new List<GameEvent>();

            if (!isGameActive)
            {
                UnityEngine.Debug.LogWarning("Game is inactive");
                return events;
            }

            Player currentPlayer = GetCurrentPlayer();
            Card card = currentPlayer.Hand.Find(c => c.CardInstanceId == cardInstanceId);
            if (card == null)
            {
                UnityEngine.Debug.LogWarning("Card not found in hand");
                return events;
            }

            if (!currentPlayer.Hand.Contains(card))
            {
                UnityEngine.Debug.LogWarning("It's not the player's turn");
                return events;
            }

            events.AddRange(PutCardOnBattlefield(card));

            return events;
        }
        #endregion


        //W tej sekcji są metody realizujące zglaszne requesty
        #region Game Logic

        private List<GameEvent>StartGame()
        {
            List<GameEvent>events = new List<GameEvent>();
            isGameActive = true;

            firstPlayer.Deck.ShuffleCrypto();
            secondPlayer.Deck.ShuffleCrypto();


            for (int i = 0; i < INITIAL_HAND_SIZE; i++)
            {
                events.AddRange(DrawCard(firstPlayer));
                events.AddRange(DrawCard(secondPlayer));
            }
            events.Add(new GameStartedEvent());

            return events;
        }

        private IEnumerable<GameEvent>PutCardOnBattlefield(Card card)
        {
            List<GameEvent>events = new List<GameEvent>();

            Player currentPlayer = GetCurrentPlayer();

            currentPlayer.Hand.Remove(card);
            currentPlayer.Battlefield.Add(card);

            events.Add(new PutCardOnBattlefieldEvent(currentPlayer.Id, card.CardInstanceId));

            return events;
        }

        private List<GameEvent>EndTurn()
        {
            List<GameEvent>events = new List<GameEvent>();

            isFirstPlayerTurn = !isFirstPlayerTurn;
            if (isFirstPlayerTurn)
                turnCounter++;

            //Resetujemy aktywność wszystkich kart nowego gracza
            Player currentPlayer = GetCurrentPlayer();
            foreach (Card card in currentPlayer.Battlefield)
            {
                card.MovedThisTurn = false;
            }

            //Nowy gracz dobiera kartę
            events.Add(new NewTurnEvent(currentPlayer.Id));
            events.AddRange(DrawCard(currentPlayer));
            return events;
        }
        private List<GameEvent>DrawCard(Player player)
        {
            List<GameEvent>events = new List<GameEvent>();

            if (player.ToDraw.Count == 0)
            {
                UnityEngine.Debug.LogWarning("Player has no cards to draw");
                return events; // No cards to draw
            }
            Card drawnCard = player.ToDraw.Pop();
            player.Hand.Add(drawnCard);
            events.Add(new DrawCardEvent(player.Id, drawnCard.CardId, drawnCard.CardInstanceId));
            return events;
        }
        private List<GameEvent>AttackCard(Card attacker, Card defender)
        {
            List<GameEvent>events = new List<GameEvent>();

            if (attacker.CanAttack(defender))
            {
                attacker.MovedThisTurn = true;

                defender.Health -= attacker.Attack;
                attacker.Health -= defender.Attack;

                events.Add(new CardUpdatedEvent(0, attacker.CardInstanceId));
                events.Add(new CardUpdatedEvent(0, defender.CardInstanceId));

                if (defender.Health <= 0)

                    events.Add(new CardDestroyedEvent(0, defender.CardInstanceId));

                if (attacker.Health <= 0)
                    events.Add(new CardDestroyedEvent(0, attacker.CardInstanceId));

                events.Add(new AttackEvent(attacker.CardInstanceId, defender.CardInstanceId));
            }

            return events;
        }
        #endregion

        private Card getCardByInstanceId(int instanceId, Player player)
        {
            foreach (Card card in player.Deck)
            {
                if (card.CardInstanceId == instanceId)
                    return card;
            }

            return null; // Card not found
        }
        private List<Card> createDeck(DeckData deckData, int ownerId)
        {
            List<Card> deck = new List<Card>();

            // Add cards to the deck based on the DeckData
            foreach (var cardData in deckData.cards)
            {
                deck.Add(new Card(cardData, ++lastCardInstanceId, ownerId));
            }

            return deck;
        }

        private Player GetPlayerById(int playerId)
        {
            if (firstPlayer.Id == playerId)
                return firstPlayer;
            if (secondPlayer.Id == playerId)
                return secondPlayer;
            return null;
        }

        public Card GetCardByInstanceId(int instanceId)
        {
            Card card = getCardByInstanceId(instanceId, firstPlayer);
            if (card != null)
                return card;

            card = getCardByInstanceId(instanceId, secondPlayer);
            return card;
        }
        
        public Player GetCurrentPlayer()
        {
            return isFirstPlayerTurn ? firstPlayer : secondPlayer;
        }
    }
}
