using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private SolitaireGame solitaireGame;
    // Start is called before the first frame update
    void Start()
    {
        solitaireGame = FindObjectOfType<SolitaireGame>();
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
                    Card();
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

    void Card()
    {
        // card click actions
        Debug.Log("Clicked on a Card");
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
