using UnityEngine;
using System.Collections;

namespace Prizm {

	//used as the location for spawning objects from HH
	public class TargetZone : Zone {

		Player myOwner;


		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.targetZone;
			myZoneDescriptor.GenerateNewGUID ();
		}

		protected override void Start() {
			base.Start ();

			if (WebsocketMessageQueue.instance != null)
			{
				WebsocketMessageQueue.instance.AddHandler(Topics.PieceDescriptor, PieceDescriptorFunction);
			}
			//if (WebsocketServer.instance != null) {
			//	WebsocketServer.instance.PieceDescriptorData += PieceDescriptorFunction;
			//} else if (WebsocketClient.instance != null) {
			//	WebsocketClient.instance.PieceDescriptorData += PieceDescriptorFunction;

			//} else {
			//	Debug.LogError ("websocket client nor server is loaded yet, consider loading later with bootstrap");
			//}
		}

		protected override void OnDisable() {
			base.OnDisable ();

			if (WebsocketMessageQueue.instance != null)
			{
				WebsocketMessageQueue.instance.RemoveHandler(Topics.PieceDescriptor, PieceDescriptorFunction);
			}

			//if (WebsocketServer.instance != null) {
			//	WebsocketServer.instance.PieceDescriptorData -= PieceDescriptorFunction;
			//} else if (WebsocketClient.instance != null) {
			//	WebsocketClient.instance.PieceDescriptorData -= PieceDescriptorFunction;
			//} else {
			//	Debug.LogError ("websocket client nor server is loaded");
			//}
		}


		//not sure if target zones should implement ontriggerenter
		/*
		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			Debug.Log("Collided with: " + other.name);
			if (other.GetComponent<Piece> () != null) {
				other.GetComponent<Piece> ().myHostZone = this;
				hostedPieces.Add (other.GetComponent<Piece> ());
			} else {
				Debug.Log ("other piece does not have a Piece component");
			}
		}

		protected override void OnTriggerExit(Collider other) {
			base.OnTriggerExit (other);
			if (other.GetComponent<Piece> () != null) {
				other.GetComponent<Piece> ().myHostZone = null;
				hostedPieces.Remove (other.GetComponent<Piece> ());
			}
		}

		*/





		public void SetOwner(Player owner){
			myOwner = owner;
			myZoneDescriptor.playerOwnerGuid = myOwner.myPlayerData.playerGuid;
		}

		//triggered whenever a piece changes location on the websocket
		//if the piece's zone matches our zoneID, we'll instantiate a new one
		void PieceDescriptorFunction(JSONObject data) {
			PieceDescriptor newPieceDescr = new PieceDescriptor (data);

			Debug.LogError (name + " has receieved piece data: " + newPieceDescr.zoneGuid + " == myzoneguid: " + myZoneDescriptor.zoneGuid);

			if (newPieceDescr.zoneGuid == myZoneDescriptor.zoneGuid) {
				Debug.LogError ("we have a match for the zone guids! going to try to instantiate new object");
				Debug.Log ("prefab path is: " + newPieceDescr.prefabDataPath);
				if (string.IsNullOrEmpty (newPieceDescr.prefabDataPath)) {
					Debug.LogError ("prefab path is empty!!!");
				}
				try {
					GameObject newPieceGO = Instantiate (Resources.Load (newPieceDescr.prefabDataPath)) as GameObject;
					Piece newPiece = newPieceGO.GetComponent<Piece> ();
					newPiece.myPieceDescriptor = newPieceDescr;

					if (!string.IsNullOrEmpty(newPieceDescr.spriteDataPath)) {
						newPieceGO.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(newPieceDescr.spriteDataPath);
					}

					if (newPiece == null) {
						Debug.LogError ("instantiate object from prefab path does not have a piece component!");
					}

					//move piece to location
					//maybe should instantiate object at player dock zone, and then move it to the target zone to make it look nice
					MovePieceToZone(newPiece, this);
				} catch (System.Exception ex) {
					Debug.LogError (ex);
				}
			}
		}

		public void BroadcastToHH() {

			JSONObject zoneData = new JSONObject ();
			zoneData.AddField ("type", "ZoneDescriptor");

			zoneData.AddField ("data", new JSONObject(JsonUtility.ToJson (myZoneDescriptor)));
			Debug.Log("sending zone data for targetzone: " + myZoneDescriptor.gatewayZoneCategory);

			if (WebsocketServer.instance != null) {
				WebsocketServer.instance.BroadcastData (zoneData);
			} else if (WebsocketClient.instance != null) {
				WebsocketClient.instance.BroadcastData (zoneData);
			} else {
				Debug.LogError ("neither server nor client!");
			}
		}


		//TODO: find use for these
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