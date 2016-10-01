using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Prizm {

	//storing abstract data, such as player's targetzones, points, etc.
	public class HHPlayer : Player {



		private TTDockZone myDockZone;

		public List<ThrowZone> myThrowZones;
		public List<TargetZone> myTargetZones;

		void Awake() {
			if (myTargetZones.Count == 0) {
				Debug.LogWarning ("target zones is empty!");
			}
			if (myThrowZones.Count == 0) {
				Debug.LogWarning ("throw zones is empty!");
		}
		}
			

		//called when player selects their seat
		public void BootstrapPlayer(PlayerDescriptor thisPlayersData) {
			myPlayerData = thisPlayersData;

			foreach (TargetZone tz in myTargetZones) {
				tz.SetOwner (this);
			}

			SendPlayerDescriptor ();
			SendTargetZoneDescriptors ();
		}
			


		//if we get zone data that matches our player, set all categories of that zone to the same guid
		//(syncs target and throw zone guids across TT and HH)
		public void SetThrowZones(JSONObject data) {
			Debug.Log ("got zone data!");
			ZoneDescriptor tempZD = new ZoneDescriptor (data);
			if (tempZD.playerOwnerGuid == myPlayerData.playerGuid) {
				Debug.Log ("This is our data! (double checked), now setting guids" + data.ToString());

				foreach (ThrowZone tz in myThrowZones) {
					if (tempZD.gatewayZoneCategory == tz.myZoneDescriptor.gatewayZoneCategory) {
						tz.myZoneDescriptor.zoneGuid = tempZD.zoneGuid;
					}
				}
			}
		}
			


		public void SendPlayerDescriptor(){
			//Debug.Log ("sending players descriptor");

			JSONObject tempObj = new JSONObject ();
			tempObj.AddField ("type", "PlayerDescriptor");
			tempObj.AddField ("data", new JSONObject(JsonUtility.ToJson (myPlayerData)));
			WebsocketClient.instance.BroadcastData (tempObj);
		}

		void SendTargetZoneDescriptors() {
			Debug.Log ("broadcasting HH targetzones");

			foreach (TargetZone tz in myTargetZones) {
				tz.BroadcastToHH ();
			}
		}



	}
}