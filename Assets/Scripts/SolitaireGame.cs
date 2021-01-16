using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SolitaireGame : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckPos;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] tops;

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    [SerializeField] private float waitTime = 0f;
    public bool doneDealing = false;

    public List<string> deck;

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
        SolitaireSort();
        StartCoroutine (SolitaireDeal());
    }

    // Create deck from the 2 lists of strings
    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(v + s);
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
                yield return new WaitForSeconds(waitTime);

                //GameObject newCard =
                //    Instantiate(
                //        cardPrefab,
                //        new Vector3(
                //            bottomPos[i].transform.position.x,
                //            bottomPos[i].transform.position.y - yOffset,
                //            bottomPos[i].transform.position.z - zOffset),
                //        Quaternion.identity,
                //        bottomPos[i].transform);
                GameObject newCard = Instantiate(
                    cardPrefab,
                    new Vector3(
                        deckPos.transform.position.x,
                        deckPos.transform.position.y,
                        deckPos.transform.position.z
                        ),
                    Quaternion.identity,
                    bottomPos[i].transform
                    );
                Debug.Log("bottomPosX = " + bottomPos[i].transform.position.x + ", bottomPosY = " + bottomPos[i].transform.position.y + ", bottomPosZ = " + bottomPos[i].transform.position.z);
                iTween.MoveTo(
                    newCard,
                    bottomPos[i].transform.position.x,
                    new Vector3(
                        bottomPos[i].transform.position.x,
                        bottomPos[i].transform.position.y - yOffset,
                        bottomPos[i].transform.position.z - zOffset),
                    1.5f);
                //yield return newCard;
                //yield return new WaitForSeconds();
                newCard.name = card;
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().faceUp = true;
                }

                yOffset = yOffset + 0.3f;
                zOffset = zOffset + 0.03f;
            }
            if (i == 6)
            {
                doneDealing = true;
            }
            if (doneDealing)
            {
                //Debug.Log("done dealing");
            }
            else
            {
                //Debug.Log("still dealing");
            }
        }
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
}
