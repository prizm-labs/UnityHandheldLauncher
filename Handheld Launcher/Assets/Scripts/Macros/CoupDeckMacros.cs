using UnityEngine;
using System.Collections;
using Prizm;


public class CoupDeckMacros : MonoBehaviour {

	private DeckZone myDeckZone;


	void Awake() {
		myDeckZone = GetComponent<DeckZone> ();
		if (myDeckZone == null) {
			Debug.LogError ("we don't have a deckzone with a deck macro on " + name);
		}
	}

	//User-defined macros go into Start
	void Start () {
		myDeckZone.TapMacro = StartDrawCard;
		myDeckZone.FlickMacro = StartDealCards;
		myDeckZone.LongPressMacro = StartRecallCards;

//		TTManager.instance.PlayerAdded += HandlePlayerAddedDelegate;

		StartShuffle ();
	}
		

	void OnDisable() {
//		TTManager.instance.PlayerAdded -= HandlePlayerAddedDelegate;
	}




	//these macros define movement, sound, and any possible animator triggers
	void StartDealCards() {
		//Debug.Log ("dealing cards");
		StartCoroutine (DealCards ());
	}

	void StartDrawCard() {
		//Debug.Log ("drawing card");
		StartCoroutine (DrawCard());
	}


	void StartShuffle() {
		StartCoroutine (ShuffleCards ());
	}

	void StartRecallCards() {
		StartCoroutine (RecallCards());
	}

	public IEnumerator DrawCard() {

		if (myDeckZone.hostedPieces.Count == 0) {
			Debug.LogError ("out of cards");
			yield break;
		}
		Zone.MovePieceToZone (myDeckZone.hostedPieces [0], myDeckZone, false, GameLayers.transport, 0.2f);	//TODO: make sure hosted pieces never runs out
		myDeckZone.hostedPieces.RemoveAt (0);

		yield return new WaitForSeconds (0.1f);
		
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
					Debug.Log ("playing sound");
					myDeckZone.hostedPieces [0].PlayASound ();
					Zone.MovePieceToZone (myDeckZone.hostedPieces [0], sz);

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
			myDeckZone.hostedPieces [0].PlayASound ();
			Zone.MovePieceToZone (myDeckZone.hostedPieces [0], myDeckZone.targetDealLocations [myDeckZone.dealingIndex]);	//TODO: make sure hosted pieces never runs out
			myDeckZone.hostedPieces.RemoveAt (0);
			myDeckZone.dealingIndex++;

			yield return new WaitForSeconds (0.1f);
		}
	}

	public IEnumerator RecallCards() {
		StopAllCoroutines ();

		Debug.Log ("recalling cards");
		foreach (Transform child in transform) {
			if (child.GetComponent<Piece> () != null) {
				Zone.MovePieceToZone (child.GetComponent<Piece> (), myDeckZone, rotateAngles: false,travelTime: 0.8f);

				//yield return new WaitForSeconds (0.05f);
			}
		}

		foreach (CardPiece cp in myDeckZone.originalCardsInDeck) {
			Zone.MovePieceToZone (cp, myDeckZone, rotateAngles: false, travelTime: 0.8f);
			cp.transform.SetParent (transform);
		}

		if (myDeckZone.hostedPieces.Count > 0) {
			myDeckZone.hostedPieces [0].PlayASound ();
		}

		myDeckZone.GetDeck ();
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

	//uses itween to punch each card in deck and randomizes the order of hostedpieces (not transform children)
	IEnumerator ShuffleCards() {
		Debug.Log ("shuffling cards");

		Vector3 punchVector = new Vector3 (10.0f, 10.0f, 10.0f);

		for (int i = 0; i < myDeckZone.hostedPieces.Count; i++) {
			Piece temp = myDeckZone.hostedPieces [i];
			int randomIndex = Random.Range (i, myDeckZone.hostedPieces.Count);
			myDeckZone.hostedPieces [i] = myDeckZone.hostedPieces [randomIndex];
			myDeckZone.hostedPieces [randomIndex] = temp;

			Hashtable punchArgs = new Hashtable ();
			punchArgs.Add ("amount", punchVector);
			punchArgs.Add ("time", 0.2f);
			iTween.PunchRotation(temp.gameObject, punchArgs);
		}

		yield return null;
	}
}
