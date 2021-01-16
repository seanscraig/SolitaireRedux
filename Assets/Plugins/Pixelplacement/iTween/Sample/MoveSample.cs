using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{
	public GameObject destination;
	void Start(){
		//iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
		//iTween.MoveAdd(
		//	gameObject,
		//	new Hashtable() {
		//		{ iT.MoveAdd.x, 2 },
		//		{ iT.MoveAdd.easetype, "easeInOutExpo" },
		//		{ iT.MoveAdd.looptype, "pingPong" },
		//		{ iT.MoveAdd.delay, 0.1 }
		//	});
		iTween.MoveBy(gameObject, destination.transform.position, 2);
	}
}

