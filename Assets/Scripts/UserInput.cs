using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private SolitaireGame solitaireGame;
    // Start is called before the first frame update
    void Start()
    {
        solitaireGame = FindObjectOfType<SolitaireGame>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && solitaireGame.canClick)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                // What has been hit? Deck/Card/Empty Slot
                if (hit.collider.CompareTag("Deck"))
                {
                    // clicked deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    // clicked card
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    // clicked top
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    // clicked bottom
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck()
    {
        // deck click actions
        //Debug.Log("Clicked on the Deck");
        StartCoroutine(solitaireGame.DealFromDeck());
    }

    void Card(GameObject selected)
    {
        // card click actions
        Debug.Log("Clicked on a Card");

        // if it is facedown & not blocked
        if (!selected.GetComponent<Selectable>().faceUp)
        {
            if (!Blocked(selected))
            {
                // flip it over
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }
        }

        // if the card clicked on is in the deck pile with the trips
        else if (selected.GetComponent<Selectable>().inDeckPile)
        {
            // if it is not blocked
            if (!Blocked(selected))
            {
                //Debug.Log("blocked returned false");
                // select it
                slot1 = selected;
            }
            //else
            //{
            //    Debug.Log("blocked returned true");
            //}
        }

        // if the card is face up
        // if there is no card currently selected
        // select the card
        else
        {
            if (slot1 == this.gameObject) // not null because we pass in this gameObject instead
            {
                slot1 = selected;
            }

            // if there is already a card selected (and it is not the same card)
            else if (slot1 != selected)
            {
                // if the new card is eligable to stack on the old card
                if (Stackable(selected))
                {
                    // stack it
                    Stack(selected);
                }
                else
                {
                    // select the new card
                    slot1 = selected;
                }
            }

            // else if there is already a card selected and it is the same card
            // if the time is short enough the it is a double click
            // if the card is eligible to fly up top, then do it


        }

    }

    void Top(GameObject selected)
    {
        // top click actions
        Debug.Log("Clicked on a Top Slot");
        if (slot1.CompareTag("Card"))
        {
           // if the card is an ace and the empty slot is top, then stack
           if (slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selected);
            }
        }
    }

    void Bottom(GameObject selected)
    {
        // bottom click actions
        //Debug.Log("Clicked on a Bottom Slot");
        // if the card is a king and the empty slot is bottom, then stack
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        //Debug.Log("s1 = " + s1);
        //Debug.Log("s2 = " + s2);
        // compare them to see if they stack
        if (!s2.inDeckPile)
        {
            if (s2.top) // if in the top pile, the cards must stack suited A to K
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else // if in the bottom pile, they must stack alternate colors K to A
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        Debug.Log("Not Stackable");
                        return false;
                    }
                    else
                    {
                        Debug.Log("Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        // if on top of king or empty bottom, stack the cards in place
        // else stack the cards with a negative y offset

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();

        float yOffset = 0.3f;

        if (s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(
            selected.transform.position.x,
            selected.transform.position.y - yOffset,
            selected.transform.position.z - 0.01f);

        //Debug.Log("x = " + selected.transform.position.x + "y = " + selected.transform.position.y + "z = " + selected.transform.position.z);
        slot1.transform.parent = selected.transform; // this makes the children move with the parents

        if (s1.inDeckPile) // removes the cards from the trips pile to prevent duplicate cards
        {
            solitaireGame.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && s1.value == 1) // allows movement of cards between top spots
        {
            solitaireGame.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaireGame.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top) // keeps track of the card string from the appropriate bottom list
        {
            solitaireGame.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else // removes the card string from the appropriate bottom list
        {
            solitaireGame.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; // you cannot add cards to the trips pile so this is always fine
        s1.row = s2.row;

        if (s2.top)
        {
            solitaireGame.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaireGame.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }

        // after completing move reset slot1 to be essentially null as being null will break the logic
        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitaireGame.tripsOnDisplay.Last())
            {
                return false;
            }
            else
            {
                //Debug.Log(s2.name + " is blocked by " + solitaireGame.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitaireGame.bottoms[s2.row].Last()) // check if it is the bottom card
            {
                return false;
            }
            else
            {
                //Debug.Log(s2.name + " is blocked by " + solitaireGame.bottoms[s2.row].Last());
                return true;
            }
        }
        //Selectable s2 = selected.GetComponent<Selectable>();
        //if (s2.inDeckPile == true)
        //{
        //    if (s2.name == solitaireGame.tripsOnDisplay.Last()) // if it is the last trip it is not blocked
        //        return false;
        //    else
        //    {
        //        //print(s2.name + " is blocked by " + solitaireGame.tripsOnDisplay.Last());
        //        return true;
        //    }
        //}
        //else
        //{
        //    if (s2.name == solitaireGame.bottoms[s2.row].Last()) // check if it is the bottom card
        //        return false;
        //    else
        //        return true;
        //}
    }
}
