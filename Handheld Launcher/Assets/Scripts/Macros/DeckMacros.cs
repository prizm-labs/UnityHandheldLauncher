using UnityEngine;
using System.Collections;
using Prizm;


public class DeckMacros : MonoBehaviour {

	private DeckZone myDeckZone;


	void Awake() {
		myDeckZone = GetComponent<DeckZone> ();
		if (myDeckZone == null) {
			Debug.LogError ("we don't have a deckzone with a deck macro on " + name);
		}
	}

	//User-defined macros go into Start
	void Start () {
		myDeckZone.TapMacro = StartRecallCards;
		myDeckZone.FlickMacro = StartDealCards;

//		TTManager.instance.PlayerAdded += HandlePlayerAddedDelegate;
	}
		

	void OnDisable() {
//		TTManager.instance.PlayerAdded -= HandlePlayerAddedDelegate;
	}




	//these macros define movement, sound, and any possible animator triggers
	void StartDealCards() {
		StartCoroutine (DealCards ());
	}

	void StartRecallCards() {
		StartCoroutine (RecallCards());
	}

	public IEnumerator DealCards() {
		if (myDeckZone.targetDealLocations.Count == 0) {
			Debug.LogWarning ("no locations to deal to!");
			yield break;
		}
		if (((DeckPrefabData)myDeckZone.myPrefabData).completeDeal) {
			foreach (SnapZone sz in myDeckZone.targetDealLocations) {
				//if there is not a card in our target location, deal a card from our list and put it there
				if (myDeckZone.hostedPieces.Count == 0) {
					Debug.LogError ("out of cards! can't deal no more!");
					break;
				}
				if (sz.hostedPieces.Count == 0) {
					Zone.MovePieceToZone (myDeckZone.hostedPieces [0], sz);
					myDeckZone.hostedPieces [0].PlayASound ();
					myDeckZone.hostedPieces.RemoveAt (0);

					yield return new WaitForSeconds (0.1f);
				}
			}
		} else {
			//Debug.Log ("incompelte deal");
			if (myDeckZone.dealingIndex >= myDeckZone.targetDealLocations.Count)
				myDeckZone.dealingIndex = 0;
			if (myDeckZone.hostedPieces.Count == 0) {
				Debug.LogError ("out of cards");
				yield break;
			}
			Zone.MovePieceToZone (myDeckZone.hostedPieces [0], myDeckZone.targetDealLocations [myDeckZone.dealingIndex]);	//TODO: make sure hosted pieces never runs out
			myDeckZone.hostedPieces.RemoveAt (0);
			myDeckZone.dealingIndex++;

			yield return new WaitForSeconds (0.1f);
		}
	}

	public IEnumerator RecallCards() {
		StopAllCoroutines ();
		myDeckZone.GetDeck ();
		Debug.Log ("recalling cards");
		foreach (Transform child in transform) {
			if (child.GetComponent<Piece> () != null) {
				Zone.MovePieceToZone (child.GetComponent<Piece> (), myDeckZone, travelTime: 0.8f);

				//yield return new WaitForSeconds (0.05f);
			}
		}

		foreach (CardPiece cp in myDeckZone.originalCardsInDeck) {
			Zone.MovePieceToZone (cp, myDeckZone, travelTime: 0.8f);
		}

		if (myDeckZone.hostedPieces.Count > 0) {
			myDeckZone.hostedPieces [0].PlayASound ();
		}
		yield return null;
	}

	//when a new player is added, we have to add them to the deal zones
	void HandlePlayerAddedDelegate (TTPlayer newPlayer)
	{
		Debug.Log ("player added, adding them to deck deal zones, throw zones count: " + newPlayer.myThrowZones.Count);
		foreach(ThrowZone tz in newPlayer.myThrowZones) {
			Debug.Log ("added throw zone to deck deal list!");
			myDeckZone.targetDealLocations.Add (tz);
		}
	}
}
