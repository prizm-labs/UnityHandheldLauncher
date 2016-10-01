using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

namespace Prizm {

	//pieces are able to float freely in free zones without rubber banding
	//free zone turns rubber banding off in a large area
	//has a large collider
	public class FreeZone : Zone {
		public List<PieceCategories> acceptedCategories;


		public SnapPrefabData myPrefabData;

		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.freeZone;

		}



		//SnapZones take pieces that are in their "acceptedCategories" list and set that piece's host zone to itself
		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			Debug.Log ("on trigger enter snapzone");
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				if (acceptedCategories.Contains (otherPiece.myCategory)) {
					otherPiece.myHostZone = this;
					hostedPieces.Add (otherPiece);

					otherPiece.rubberBanding = false;
				}
			} else {
				Debug.Log ("other piece does not have a Piece component");
			}

		}
			

		public override void SaveToPrefabData() {
			Debug.LogError ("save to prefab data not implemented");
			//throw System.NotImplementedException;
		}

		public override void ApplyDataChange() {
			Debug.LogError ("save to prefab data not implemented");
			//throw System.NotImplementedException;
		}

		//free zones only relieve the hostzone if the piece enters another zone
		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit (other);


			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				//other.GetComponent<Piece> ().myHostZone = null;
				hostedPieces.Remove (other.GetComponent<Piece> ());

				otherPiece.rubberBanding = true;
			}

		}

	}


}