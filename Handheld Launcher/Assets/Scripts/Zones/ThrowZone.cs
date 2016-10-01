using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Prizm {

	//sends data that collides with to the targetzone on the other side of HH
	public class ThrowZone : Zone {



		//makes sure the object is still in the collider after .3 seconds before throwing it to HH
		//prevents un-intended throws
		[System.NonSerialized]
		public float throwDelay = 0.3f;


		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.throwZone;
			//myZoneDescriptor = new ZoneDescriptor ();
		}
			





		void GetTargetZoneDescriptorHandler() {
			Debug.Log ("receiving some targetzone data");
			//if playerguid == myplayer guid
			//if gatwayzonecategory == myzonedescriptor.gatewayzonecateogry
				//put zoneGuid in arriving packet to 
				//myTargetZone = new ZoneDescriptor();
				//myTargetZone.zoneGuid = arrivinguid;

		}


		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			//Debug.Log("Collided with: " + other.name);
			if (other.GetComponent<Piece> () != null) {
				
				Piece otherPiece = other.GetComponent<Piece> ();
				if (acceptedCategories.Contains (otherPiece.myCategory)) {
					//TransportPiece (otherPiece);
					otherPiece.readyToThrowZone = this;
					StartCoroutine (TransportPieceDelayed (otherPiece));
					StartCoroutine (OrientPieceDelayed (otherPiece));

					//Zone.MovePieceToZone (otherPiece, this);
				}


			} else {
				Debug.Log ("other piece " + other.name + "does not have a Piece component");
			}
		}


		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit (other);
			Debug.Log ("piece left: " + other.name);

			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				if (acceptedCategories.Contains (otherPiece.myCategory)) {
					otherPiece.readyToThrowZone = null;
				}
			}

		}

		IEnumerator OrientPieceDelayed(Piece transportedPiece) {
			yield return new WaitForSeconds (throwDelay);
			if (transportedPiece.readyToThrowZone == this) {
				//Debug.Log ("moving piece to this piece to target");
				transportedPiece.transform.SetParent (transform);

				Zone.MovePieceToZone (transportedPiece, this);

			} else {
				yield break;
			}
		}

		IEnumerator TransportPieceDelayed(Piece transportedPiece) {
			yield return new WaitForSeconds (throwDelay);
			if (transportedPiece.readyToThrowZone == this) {
				Debug.Log ("throwing piece to target");
				transportedPiece.myPieceDescriptor.zoneGuid = myZoneDescriptor.zoneGuid;

				JSONObject pdJSON = new JSONObject (JsonUtility.ToJson (transportedPiece.myPieceDescriptor));

				JSONObject dataPacket = new JSONObject ();
				dataPacket.AddField ("type", "PieceDescriptor");
				dataPacket.AddField ("data", pdJSON);

				if (WebsocketServer.instance != null) {
					WebsocketServer.instance.BroadcastData (dataPacket);
				} else if (WebsocketClient.instance != null) {
					WebsocketClient.instance.BroadcastData (dataPacket);
				} else {
					Debug.LogError ("neither server nor client!");
				}

				//Destroy (transportedPiece.gameObject);

			} else {
				yield break;
			}
		}

		void TransportPiece(Piece transportedPiece) {
			//create data packet
			//Debug.Log ("transporting to TT, pieceprefab: " + transportedPiece.myPieceDescriptor.prefabDataPath);

			transportedPiece.myPieceDescriptor.zoneGuid = myZoneDescriptor.zoneGuid;

			JSONObject pdJSON = new JSONObject (JsonUtility.ToJson (transportedPiece.myPieceDescriptor));

			JSONObject dataPacket = new JSONObject ();
			dataPacket.AddField ("type", "PieceDescriptor");
			dataPacket.AddField ("data", pdJSON);

			if (WebsocketServer.instance != null) {
				WebsocketServer.instance.BroadcastData (dataPacket);
			} else if (WebsocketClient.instance != null) {
				WebsocketClient.instance.BroadcastData (dataPacket);
			} else {
				Debug.LogError ("neither server nor client!");
			}

		}

		public override void ApplyDataChange ()
		{
			throw new System.NotImplementedException ();
		}

		public override void SaveToPrefabData ()
		{
			throw new System.NotImplementedException ();
		}
	}
}
