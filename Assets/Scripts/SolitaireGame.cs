using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SolitaireGame : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public GameObject cardPrefab;
    public GameObject deckButton;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] tops;
    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> deckTrips = new List<List<string>>();

    public List<string> deck;
    public List<string> discardPile = new List<string>();

    public bool canClick = false;

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    [SerializeField] private float dealWaitTime = 0f;
    [SerializeField] private float drawWaitTime = 0f;
    private int deckLocation;
    private int trips;
    private int tripsRemainder;
    

    //
    void Start()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        PlayCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Things it does:
    // 1. Generate Deck
    // 2. Shuffle Deck
    // 3. Deal Cards
    public void PlayCards()
    {
        deck = GenerateDeck();
        Shuffle(deck);
        // test the cards in the deck
        //foreach (string card in deck)
        //{
        //    Debug.Log(card);
        //}
        //Debug.Log("deck size = " + deck.Count);
        SolitaireSort();
        StartCoroutine (SolitaireDeal());
        SortDeckIntoTrips();
        //Debug.Log("deck size = " + deck.Count);
    }

    // Create deck from the 2 lists of strings
    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }

    //
    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    // Deals the cards from the bottoms list to their positions on the game board
    IEnumerator SolitaireDeal()
    {
        //Debug.Log("deal started");
        for (int i = 0; i < 7; i++)
        {
            float yOffset = 0;
            float zOffset = 0.03f;
            foreach (string card in bottoms[i])
            {
                yield return new WaitForSeconds(dealWaitTime);

                GameObject newCard = Instantiate(
                    cardPrefab,
                    new Vector3(
                        deckButton.transform.position.x,
                        deckButton.transform.position.y,
                        deckButton.transform.position.z
                        ),
                    Quaternion.identity,
                    bottomPos[i].transform
                    );
                //Debug.Log("bottomPosX = " + bottomPos[i].transform.position.x + ", bottomPosY = " + bottomPos[i].transform.position.y + ", bottomPosZ = " + bottomPos[i].transform.position.z);
                iTween.MoveTo(
                    newCard,
                    bottomPos[i].transform.position.x,
                    new Vector3(
                        bottomPos[i].transform.position.x,
                        bottomPos[i].transform.position.y - yOffset,
                        bottomPos[i].transform.position.z - zOffset),
                    1.5f);
                newCard.name = card;
                newCard.GetComponent<Selectable>().row = i;
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().faceUp = true;
                }

                yOffset = yOffset + 0.3f;
                zOffset = zOffset + 0.03f;
                discardPile.Add(card);
            }
            if (i == 6)
            {
                canClick = true;
            }
            if (canClick)
            {
                //Debug.Log("done dealing");
            }
            else
            {
                //Debug.Log("still dealing");
            }
        }

        foreach (string card in discardPile)
        {
            if (deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }

    // Adds 28 cards to the bottoms list and removes them from the deck
    void SolitaireSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    public void SortDeckIntoTrips()
    {
        trips = deck.Count / 3;
        tripsRemainder = deck.Count % 3;
        deckTrips.Clear();

        int modifier = 0;
        for (int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            for (int j = 0; j < 3; j++)
            {
                myTrips.Add(deck[j + modifier]);
            }
            deckTrips.Add(myTrips);
            modifier = modifier + 3;
        }
        if (tripsRemainder != 0)
        {
            List<string> myRemainders = new List<string>();
            modifier = 0;
            for (int k = 0; k < tripsRemainder; k++)
            {
                myRemainders.Add(deck[deck.Count - tripsRemainder + modifier]);
                modifier++;
            }
            deckTrips.Add(myRemainders);
            trips++;
        }
        deckLocation = 0;
    }

    public IEnumerator DealFromDeck()
    {
        // add remaining cards to discard pile
        foreach (Transform child in deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                //yield return new WaitForSeconds(drawWaitTime);
                //iTween.MoveTo(
                //    child.gameObject,
                //    deckButton.transform.position.x,
                //    new Vector3(
                //        deckButton.transform.position.x,
                //        deckButton.transform.position.y,
                //        deckButton.transform.position.z),
                //    1.5f);
                deck.Remove(child.name);
                discardPile.Add(child.name);
                Destroy(child.gameObject);
            }
        }
        if (deckLocation < trips)
        {
            tripsOnDisplay.Clear();
            canClick = false;
            // draw 3 new cards
            float xOffset = 2.5f;
            float zOffset = -0.2f;
            foreach (string card in deckTrips[deckLocation])
            {
                yield return new WaitForSeconds(drawWaitTime);
                GameObject newTopCard = Instantiate(
                    cardPrefab,
                    new Vector3(
                        deckButton.transform.position.x,
                        deckButton.transform.position.y,
                        deckButton.transform.position.z),
                    Quaternion.identity,
                    deckButton.transform);
                iTween.MoveTo(
                    newTopCard,
                    deckButton.transform.position.x,
                    new Vector3(
                        deckButton.transform.position.x + xOffset,
                        deckButton.transform.position.y,
                        deckButton.transform.position.z + zOffset),
                    1.5f);
                xOffset = xOffset + 0.5f;
                zOffset = zOffset - 0.2f;
                newTopCard.name = card;
                tripsOnDisplay.Add(card);
                newTopCard.GetComponent<Selectable>().faceUp = true;
                newTopCard.GetComponent<Selectable>().inDeckPile = true;
                if (card == deckTrips[deckLocation].Last())
                {
                    canClick = true;
                }
            }
            deckLocation++;
        }
        else
        {
            // Restack the top deck
            RestackTopDeck();
        }
        
    }

    void RestackTopDeck()
    {
        deck.Clear();
        foreach (string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        SortDeckIntoTrips();
    }
}
