using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

namespace Prizm {
	public class SnapZone : Zone {
		

		public bool reparentCollisions = false;
		public SnapPrefabData myPrefabData;

		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.snapZone;

			if (myPrefabData == null)
				Debug.LogError ("null prefab data");

			if (acceptedCategories.Count == 0) {
				Debug.LogWarning (name + " has no accepted categories!");
			}
		}

		protected override void Start() {
			ApplyDataChange ();
		}

		//SnapZones take pieces that are in their "acceptedCategories" list and set that piece's host zone to itself
		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			Debug.Log ("on trigger enter snapzone");
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				if (acceptedCategories.Contains (otherPiece.myCategory)) {		//only accept pieces that we're meant to
					otherPiece.myHostZone = this;
					hostedPieces.Add (otherPiece);
					if (reparentCollisions) {									//reparent pieces that enter so that when you move parent transform around, children follow
						other.transform.SetParent(transform);
					}

					if (myPrefabData.letGoOnCollide) {
						try {
							other.GetComponent<TransformGesture> ().Cancel ();
							other.GetComponent<Piece>().StopMoveMacro();
						} catch (System.NullReferenceException ex) {
							Debug.LogError ("collider " + other.name + " does not have a TransformGesture");
							Debug.LogError (ex);
						}
					}
				}
			} else {
				Debug.Log ("other piece does not have a Piece component");
			}


		}

		public override void ApplyDataChange() {
			if (myPrefabData != null) {
				if (!string.IsNullOrEmpty(myPrefabData.spritePath) && GetComponent<SpriteRenderer>() != null) {
					GetComponent<SpriteRenderer> ().sprite = Resources.Load (myPrefabData.spritePath, typeof(Sprite)) as Sprite;
				}
			}
		}

		public override void SaveToPrefabData() {
			if (myPrefabData == null) {
				myPrefabData = new SnapPrefabData ();
			}

		}

		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit (other);
			if (other.GetComponent<Piece> () != null) {
				
				Piece otherPiece = other.GetComponent<Piece> ();

				if (acceptedCategories.Contains (otherPiece.myCategory)) {
					//other.GetComponent<Piece> ().myHostZone = null;
					hostedPieces.Remove (other.GetComponent<Piece> ());
				}
			}

		}

	}


}