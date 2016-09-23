using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Prizm {
	public class TTManager : MonoBehaviour {
//		[System.NonSerialized]
//		public List<PlayerDescriptor> ListOfPlayerDocks = new List<PlayerDescriptor>();
		public static TTManager instance;
//
//		[System.NonSerialized]
//		public List<TTPlayer> myPlayers = new List<TTPlayer>();
//
		void Awake() {
			if (instance == null || instance == this) {
				instance = this;
				DontDestroyOnLoad (this);
			} else {
				Destroy (gameObject);
			}
		}

//
//		void OnEnable() {
//			WebsocketServer.instance.PlayerDescriptorData += PlayerDescriptorFunction;
//			WebsocketServer.instance.ZoneDescriptorData += ZoneDescriptorFunction;
//		}
//
//		void OnDisable() {
//			WebsocketServer.instance.PlayerDescriptorData -= PlayerDescriptorFunction;
//			WebsocketServer.instance.ZoneDescriptorData -= ZoneDescriptorFunction;
//		}
//
//		//if we get data back from a player and they are seated
//		//trigger for their zones to broadcast their guids
//		void PlayerDescriptorFunction(JSONObject data) {
//			Debug.Log ("receieved data after a player was seated");
//
//
//			//JSONObject newObj = new JSONObject (data.ToString ());
//
//			PlayerDescriptor newDescriptor = new PlayerDescriptor (data.GetField ("data"));
//			Debug.Log ("got this newDescriptor parsed: " + newDescriptor.playerName + ", seated?" + newDescriptor.playerSeated.ToString());
//
//
//			//PlayerDescriptor tempPD = (PlayerDescriptor)data;
//			Debug.Log("size of myplayers: " + myPlayers.Count.ToString());
//			foreach (TTPlayer player in myPlayers) {
//				Debug.Log ("checking to see if we have to tether: " + player.myPlayerData.playerName);
//				Debug.Log ("current descriptor: " + newDescriptor.playerGuid + " == player in list: " + player.myPlayerData.playerGuid);
//				Debug.Log ("is that player seated?" + newDescriptor.playerSeated.ToString ());
//				if (newDescriptor.playerGuid == player.myPlayerData.playerGuid && newDescriptor.playerSeated == true && !player.tethered) {	//if the object is that player and that player is seated
//					Debug.Log("beginning to tether player");
//					player.TetherPlayer ();
//				}
//			}
//		}
//
//		void ZoneDescriptorFunction(JSONObject packet) {
//			Debug.Log ("receiving zonedescriptor data");
//
//			ZoneDescriptor zoneDescr = new ZoneDescriptor (packet.GetField("data"));
//
//			foreach (TTPlayer player in myPlayers) {
//				Debug.Log ("checking to see if we have to sync throw zones: " + player.myPlayerData.playerName);
//				Debug.Log ("comparing guids: " + zoneDescr.playerOwnerGuid + "==" + player.myPlayerData.playerGuid);
//				if (zoneDescr.playerOwnerGuid == player.myPlayerData.playerGuid) {
//					Debug.Log ("this zone data belogns to player! need to sync throw zones");
//					player.SetThrowZones (packet.GetField("data"));
//				}
//			}
//		}
//
//		void Update() {
//			if (Input.GetKeyDown (KeyCode.S)) {
//				SendAllPlayersData ();
//			}
//		}
//
//		public void InstantiatePlayer() {
//			GameObject newPlayer = Instantiate (Resources.Load("Prefabs/PlayerDockTT")) as GameObject;
//
//			newPlayer.GetComponent<TTPlayer> ().BootstrapPlayer ();
//		}
//
//
//		public void SendAllPlayersData(){
//			foreach (TTPlayer dude in myPlayers) {
//				dude.SendPlayerDockStatus ();
//			}
//		}
//
	}
}