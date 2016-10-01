using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

namespace Prizm {

	//a clutter zone takes a piece and moves it manually to one of its psuedo-random locations
	public class ClutterZone : Zone {


		public SnapPrefabData myPrefabData;

		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.clutterZone;



		}



		//clutter zones can be configured to accept only certain types, such as coins
		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			//Debug.Log ("on trigger enter clutterZone");
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				if (acceptedCategories.Contains (otherPiece.myCategory)) {


					//clutter zone always lets go of the pice that collides with it
					try {
						other.GetComponent<TransformGesture> ().Cancel ();
						if (other.GetComponent<Piece>().StopMoveMacro != null)
							other.GetComponent<Piece>().StopMoveMacro();
					} catch (System.NullReferenceException ex) {
						Debug.LogError ("collider " + other.name + " does not have a TransformGesture");
						Debug.LogError (ex);
					}


					otherPiece.myHostZone = this;
					hostedPieces.Add (otherPiece);

					bool foundSpot = false;				//if we don't find an empty child transform, put it in main transform

					foreach (Transform child in transform) {
						//if the child transform has no children, there are no pieces in it and we can put new ones in
						//also if the child transform is not a piece
						if (child.childCount == 0 && child.GetComponent<Piece>() == null) {

							Hashtable ht = new Hashtable ();
							ht.Add ("position", child.position);
							ht.Add ("time", 0.2f);
							iTween.MoveTo (otherPiece.gameObject, ht);

							otherPiece.transform.SetParent (child);

							Hashtable rotationOptions = new Hashtable ();
							rotationOptions.Add ("rotation", Vector3.zero);
							rotationOptions.Add ("time", 0.2f);

							iTween.RotateTo (otherPiece.gameObject, rotationOptions);

							foundSpot = true;
						}
					}

					if (!foundSpot) {
						Hashtable ht = new Hashtable ();
						ht.Add ("position", transform.position);
						ht.Add ("time", 0.2f);
						iTween.MoveTo (otherPiece.gameObject, ht);

						otherPiece.transform.SetParent (transform);

						Hashtable rotationOptions = new Hashtable ();
						rotationOptions.Add ("rotation", Vector3.zero);
						rotationOptions.Add ("time", 0.2f);

						iTween.RotateTo (otherPiece.gameObject, rotationOptions);
					}
				}
			} else {
				Debug.Log ("other piece does not have a Piece component");
			}

		}
			

		public override void SaveToPrefabData() {
			Debug.LogError ("clutterzone not implemented save to prefab data");

		}

		public override void ApplyDataChange() {
			Debug.LogError ("apply data change not implemented");
			//throw System.NotImplementedException;
		}

		//clutterzone takes the piece and puts it into a sorted predefined area
		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit (other);


			if (other.GetComponent<Piece> () != null) {
				//other.GetComponent<Piece> ().myHostZone = null;
				hostedPieces.Remove (other.GetComponent<Piece> ());
			}

		}

	}


}