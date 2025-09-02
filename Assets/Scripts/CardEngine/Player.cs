using System.Collections.Generic;

namespace CardEngine
{
    class Player
    {
        public int Id { get; set; }
        public List<Card> Deck { get; set; }
        public List<Card> ToDraw { get; set; }
        public List<Card> Hand { get; set; }
        public List<Card> Battlefield { get; set; }
        public List<Card> Graveyard { get; set; }
    }
}