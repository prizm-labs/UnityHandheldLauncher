using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Prizm {

	//storing abstract data, such as player's targetzones, points, etc.
	public class TTPlayer : Player {



		public TTDockZone myDockZone;

		public List<TargetZone> myTargetZones;
		public List<ThrowZone> myThrowZones;

		public bool tethered = false;

		void Awake() {
			if (myTargetZones.Count == 0) {
				Debug.LogWarning ("target zones is empty!");
			}
			if (myThrowZones.Count == 0) {
				Debug.LogWarning ("throw zones is empty!");
			}
		}
			

		//has to be instart because onenable is too fast for websocketserver to finish initializing
		void Start(){
			WebsocketServer.instance.PlayerDescriptorData += PlayerDescriptorFunction;
		}

		void OnDisable(){
			WebsocketServer.instance.PlayerDescriptorData -= PlayerDescriptorFunction;
		}

		void ZoneDescriptorFunction(object data) {
			Debug.LogError ("TTplayer does not need to mess with zone descriptor anymore!");
			if (((ZoneDescriptor)data).playerOwnerGuid == myPlayerData.playerGuid) {
				Debug.Log ("this is our zone data!");

			}
		}


		//anytime the TTPlayer receives info from HH about itself, it updates itself
		void PlayerDescriptorFunction(JSONObject data) {

			PlayerDescriptor playerData = new PlayerDescriptor (data);

			if (playerData.playerGuid == myPlayerData.playerGuid) {
				//Debug.Log ("this is data meant for us!");
				myPlayerData = playerData;	//sets our new name or victory points or whatever
			
				myDockZone.UpdateDock ();
			}
		}

		public void BootstrapPlayer() {
			myPlayerData = new PlayerDescriptor ();
			myPlayerData.GenerateNewGuid ();

			TTManager.instance.myPlayers.Add (this);

			Debug.Log ("New player with name: " + myPlayerData.playerName);

			//TODO: make this better
			//transform.FindChild("Canvas/Name").GetComponent<Text>().text = "name: " + myPlayerData.playerName;

			//TODO: might replace ttdockzone with throwzone for consistency
			myDockZone = GetComponent<TTDockZone> ();
			myDockZone.myPlayer = this;

			myDockZone.UpdateDock ();

			foreach (TargetZone tz in myTargetZones) {
				tz.SetOwner (this);
				//tz.BroadcastToHH ();	//broadcasting happens during tether phase
			}

			//GameManager.instance.SendPlayerDockStatus (myPlayerData);
			UpdateHHPlayerDesc();
		}


		//establishes connection between zones
		public void TetherPlayer() {
			if (!tethered) {
				Debug.Log ("beginning TetherPlayer() process");

				Debug.Log ("we have to sync this many target zones: " + myTargetZones.Count.ToString ());
				foreach (TargetZone tz in myTargetZones) {
					tz.BroadcastToHH ();
				}

				//myDockZone.BroadcastToHH ();

				tethered = true;
			} else {
				Debug.Log ("this player is already tethered! why are you trying to tether them? lol, jk, kind of intentional but you should fix this in the future");
			}
		}

		//if we get zone data that matches our player, set all categories of that zone to the same guid
		//(syncs target and throw zone guids across TT and HH)
		public void SetThrowZones(JSONObject data) {
			Debug.Log ("got zone data!");
			ZoneDescriptor tempZD = new ZoneDescriptor (data);
			if (tempZD.playerOwnerGuid == myPlayerData.playerGuid) {
				Debug.Log ("This is our data! (double checked)" + data.ToString());

				foreach (ThrowZone tz in myThrowZones) {
					if (tempZD.gatewayZoneCategory == tz.myZoneDescriptor.gatewayZoneCategory) {
						Debug.Log ("set guids to equal each other!");
						tz.myZoneDescriptor.zoneGuid = tempZD.zoneGuid;
					}
				}
			}
		}


		//sends player descriptor data to HH
		public void UpdateHHPlayerDesc(){
			JSONObject PlayerDocks = new JSONObject ();
			PlayerDocks.AddField ("type", "PlayerDescriptor");
			//PlayerDocks.AddField ("name", "defaultname");

			PlayerDocks.AddField ("data", new JSONObject(JsonUtility.ToJson (myPlayerData)));
			Debug.Log("sending playerDescriptor data: " + myPlayerData.playerName);
			WebsocketServer.instance.BroadcastData (PlayerDocks);
		}



	}
}