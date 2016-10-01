using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine.UI;
using System.Reflection;

namespace Prizm {

	//TODO: 
	//must snap to edges
	//must have a canvas
	//ties player date
	public class TTDockZone : Zone {

		Canvas myCanvas;

		private TTPlayer _myPlayer;
		public TTPlayer myPlayer {
			get { return _myPlayer; }
			set {
				_myPlayer = value;
			}

		}

		protected override void Awake() {
			base.Awake ();
			//Debug.Log ("Awake");
			myType = TypeOfZone.dockZone;
			if (GetComponent<ApplyTransform> () == null) {
				gameObject.AddComponent<ApplyTransform> ();
			}

			myCanvas = transform.FindChild ("statsCanvas").GetComponent<Canvas>();
			//Debug.Log ("my canvas: " + myCanvas);

			//delete all this after debugging
			myPlayer = GetComponent<TTPlayer>();
			UpdateDock ();
		}



		//example of how setting the name would work
		//loops through all the properties in playerdata and tries to set the canvas value
		public void UpdateDock() {
			myCanvas.transform.FindChild ("playerName").GetComponent<Text> ().text = myPlayer.myPlayerData.playerName;


			//goes through all fields in game-specific data and sets the appropriate field
			foreach (PropertyInfo prop in myPlayer.myPlayerData.gameSpecificStats.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (myCanvas.transform.FindChild (prop.Name) != null) {
					myCanvas.transform.FindChild (prop.Name).GetComponent<Text> ().text = 
					prop.Name + ": " + prop.GetValue (myPlayer.myPlayerData.gameSpecificStats, null).ToString ();
				} else {
					Debug.LogWarning ("playerdock canvas does not have property: " + prop.Name);
				}
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


		protected override void OnTriggerEnter(Collider other) {
			base.OnTriggerEnter (other);
			//Debug.Log("Collided with: " + other.name);
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();
				otherPiece.myHostZone = this;
				hostedPieces.Add (otherPiece);

				//send the data to HH
				TransportToHH(otherPiece);

				//hide from screen
				SendToPurgatory (otherPiece);
			} else {
				Debug.Log ("other piece does not have a Piece component");
			}
		}
			

		void TransportToHH(Piece transportedPiece) {
			Debug.LogError ("using deprecated function");

			JSONObject pieceData = new JSONObject ();
			pieceData.AddField ("type", "PieceDescriptor");

			pieceData.AddField ("data", new JSONObject(JsonUtility.ToJson (transportedPiece.myPieceDescriptor)));
			Debug.LogError("sending piecedescriptor data: " + transportedPiece.myPieceDescriptor.ToString());

			WebsocketServer.instance.BroadcastData (pieceData);
		}

		void SendToPurgatory(Piece piece){
			piece.transform.position = new Vector3 (-100, -100, -100);	//debugging now, send it offscreen

		}

		//going to try to use throwzones now
		public void BroadcastToHH() {
			Debug.LogError ("using deprecated function");
			JSONObject zoneData = new JSONObject ();
			zoneData.AddField ("type", "ZoneDescriptor");

			zoneData.AddField ("data", new JSONObject(JsonUtility.ToJson (myZoneDescriptor)));
			Debug.LogError("sending complete zone descript data package: " + zoneData.ToString());
			WebsocketServer.instance.BroadcastData (zoneData);
		}
	}
}