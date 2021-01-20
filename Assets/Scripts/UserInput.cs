using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    Top();
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    // clicked bottom
                    Bottom();
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

        // if the card clicked on is facedown
            // if the card clicked on is not blocked
                // flip it over

        // if the card clicked on is in the deck pile with the trips
            // if it is not blocked
                // select it

        // if the card is face up
            // if there is no card currently selected
                // select the card
        if (slot1 == this.gameObject) // not null because we pass in this gameObject instead
        {
            slot1 = selected;
        }

            // if there is already a card selected (and it is not the same card)
                // if the new card is eligable to stack on the old card
                    // stack it
                // else
                    // select the new card

            // else if there is already a card selected and it is the same card
                // if the time is short enough the it is a double click
                    // if the card is eligible to fly up top, then do it

    }

    void Top()
    {
        // top click actions
        Debug.Log("Clicked on a Top Slot");
    }

    void Bottom()
    {
        // bottom click actions
        Debug.Log("Clicked on a Bottom Slot");
    }
}
