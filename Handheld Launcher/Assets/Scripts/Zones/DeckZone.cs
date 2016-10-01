using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine.UI;

namespace Prizm {
	public class DeckZone : Zone {

		public List<Zone> targetDealLocations = new List<Zone>();

		private OceanZone targetOceanZone;

		public GenerateDeckSettings myGenerateDeckSettings;

		public DeckPrefabData myPrefabData;

		public List<CardPiece> originalCardsInDeck = new List<CardPiece>();


		private bool dataLoaded = false;
		[System.NonSerialized]
		public int dealingIndex = 0;

		[System.Serializable]
		public class GenerateDeckSettings
		{
			public GameObject cardPrefab;
			public string wordListPath;
		}

		public class WordList {
			public List<string> words;
		}

		private List<string> myWordList = new List<string> ();

		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.deckZone;

			if (myPrefabData.generateByScript) {
				StartCoroutine (GenerateDeck ());
			} else {
				//getting deck is now done in Start() via ApplyDataChange
				//GetDeck ();
			}
		}

		//SDK default macros go into OnEnable
		protected override void OnEnable() {
			base.OnEnable ();
			PieceEnterMacro = TakeOwnership;
		}

		//adds to our hosted pieces
		//sets this transform as the parent (this behaviour is only found on decks)
		void TakeOwnership(Piece piece) {
			if (myPrefabData.takesOwnership) {
				if (piece.myType == TypeOfPiece.cardPiece) {
					Debug.Log ("taking ownership of cardpiece: " + piece.name);
					piece.transform.SetParent (transform);
					hostedPieces.Add (piece);
				}
			}
		}


		protected override void Start() {
			base.Start ();
			ApplyDataChange ();
		}

		public override void ApplyDataChange() {
			//only load once
			if (dataLoaded)
				return;
			dataLoaded = true;
			if (myPrefabData != null) {
				if (!string.IsNullOrEmpty(myPrefabData.spritePath) && GetComponent<SpriteRenderer>() != null) {
					GetComponent<SpriteRenderer> ().sprite = Resources.Load (myPrefabData.spritePath, typeof(Sprite)) as Sprite;
				}
			}

			//TODO: make sure this has correct behavior
			if (myPrefabData != null) {
				if (!string.IsNullOrEmpty(myPrefabData.targetOceanZoneName)) {
					try {
						targetOceanZone = GameObject.Find (myPrefabData.targetOceanZoneName).GetComponent<OceanZone> ();
					} catch (System.NullReferenceException ex) {
						Debug.LogWarning (ex);
					}
				}
			}
			//find the ocean snap zones
			if (targetOceanZone != null) {
				foreach (SnapZone sz in targetOceanZone.mySnapZones) {
					targetDealLocations.Add (sz);
				}
			}

			GetDeck ();
		}

		public override void SaveToPrefabData() {
			if (myPrefabData == null) {
				myPrefabData = new DeckPrefabData ();
			}

		}


		public void GetDeck() {
			//Debug.Log ("getting deck");
			hostedPieces.Clear ();
			foreach (Transform child in transform) {
				//Debug.Log ("we have child in transform");
				if (child.GetComponent<CardPiece> () != null) {
					//Debug.Log ("added piece to deck");
					hostedPieces.Add (child.GetComponent<CardPiece> ());

					if (!originalCardsInDeck.Contains(child.GetComponent<CardPiece>())) {
						originalCardsInDeck.Add(child.GetComponent<CardPiece>());
					}
				}
			}
			//Debug.Log ("our deck size: " + hostedPieces.Count);
		}
			
		//decks take ownership of collided cards
		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			//Debug.Log("Deck zone Collided with: " + other.name);
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				otherPiece.myHostZone = this;
				TakeOwnership (otherPiece);
			} else {
				Debug.Log ("other piece does not have a Piece component");
			}
		}



		//shuffle all the cards around
		public void ShuffleCards() {
			foreach (Piece p in hostedPieces) {
				Debug.Log ("shuffling p: " + p.ToString ());

			}

			Debug.LogError ("not implemented shufflecards in deckzone");
		}



		IEnumerator GenerateDeck() {
			Debug.Log ("generating deck");
			yield return GetWordList ();

			foreach (string word in myWordList) {
				GameObject newCard = Instantiate (myGenerateDeckSettings.cardPrefab, transform.position, Quaternion.identity) as GameObject;
				newCard.transform.GetChild (0).Find ("topText").GetComponent<Text> ().text = word;
				newCard.transform.GetChild (0).Find ("botText").GetComponent<Text> ().text = word;



				newCard.transform.position = new Vector3 (newCard.transform.position.x, (float)GameLayers.lowerPiece, newCard.transform.position.z);

				newCard.transform.parent = transform;
				newCard.transform.localEulerAngles = Vector3.zero;

				hostedPieces.Add (newCard.GetComponent<Piece> ());
			}
		}


		IEnumerator GetWordList() {
			string dataPath = Application.streamingAssetsPath + "\\" + myGenerateDeckSettings.wordListPath;

			System.IO.StreamReader sr = new System.IO.StreamReader (dataPath);
			yield return sr;

			WordList tempWL = JsonUtility.FromJson<WordList> (sr.ReadToEnd ());

			myWordList = tempWL.words;
		}
	}
}