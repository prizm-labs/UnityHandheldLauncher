using UnityEngine;
using System.Collections;
using Prizm;

public class StoredMacros : MonoBehaviour {

	StoredZone myStoredZone;


	void Awake() {
		myStoredZone = GetComponent<StoredZone> ();
		if (myStoredZone == null) {
			Debug.LogError ("we don't have a storedZone with a macro on " + name);
		}
	}

	//User-defined macros go into Start
	void Start () {
		myStoredZone.TapMacro = StartRecallCards;
	}

	//these macros define movement, sound, and any possible animator triggers
	void StartRecallCards() {
		StartCoroutine (RecallCards());
	}

	public IEnumerator RecallCards() {
		StopAllCoroutines ();
		Debug.Log ("recalling cards");
		foreach (Transform child in transform) {
			if (child.GetComponent<Piece> () != null) {
				Zone.MovePieceToZone (child.GetComponent<Piece> (), myStoredZone, travelTime: 0.8f);

				//yield return new WaitForSeconds (0.05f);
			}
		}
		//myStoredZone.hostedPieces [0].PlayASound ();
		yield return null;
	}
}
