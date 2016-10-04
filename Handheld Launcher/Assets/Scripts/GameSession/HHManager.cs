using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Prizm
{
	public class HHManager : MonoBehaviour
	{
		public static HHManager instance;

		public GameObject playerPrefab;
		public GameObject playerIconPrefab;
		public GameObject playerDockPrefab; //"Prefabs/GameSession/PlayerDockHH";

		List<HHPlayerSelector> potentialPlayers;

		string playerId;
		bool playerSeated = false;
		HHPlayer myPlayer = null;

		void Awake ()
		{
			if (instance == null) {
				instance = this;
			} else {
				Destroy (gameObject);
			}
		}

		void Start ()
		{
			potentialPlayers = new List<HHPlayerSelector>();
		}

		void OnEnable()
		{
			WebsocketMessageQueue.instance.AddHandler(Topics.PlayerDescriptor, OnPlayerMessage);
			WebsocketMessageQueue.instance.AddHandler(Topics.SeatRequest, OnSeatRequestMesage);
		}

		void OnDisable()
		{
			WebsocketMessageQueue.instance.RemoveHandler(Topics.PlayerDescriptor, OnPlayerMessage);
			WebsocketMessageQueue.instance.RemoveHandler(Topics.SeatRequest, OnSeatRequestMesage);
		}


		// SEATING PROCESS
		//================


		public void RequestSeatForPlayer(PlayerDescriptor selectedPlayer)
		{
			Debug.Log("selecting this player" + selectedPlayer);
			Debug.Log("complete information around this player: " + new JSONObject(JsonUtility.ToJson(selectedPlayer)).ToString());

			NetworkingManager.instance.RequestSeat(selectedPlayer);
		}


		public void DisconnectPlayer()
		{

		}


		// DELEAGTES
		//==========


		void OnSeatRequestMesage(JSONObject message)
		{
			Debug.Log("OnSeatRequestMessage: " + message.ToString());

			bool seatWasGranted = message.GetField("granted").b;
			PlayerDescriptor playerInfo = new PlayerDescriptor(message.GetField("data"));

			// if seat granted
			if (seatWasGranted) {
				Debug.Log("setting player");

				// TODO INSTANTIATE PLAYER & DOCK
				//GameObject newPlayer = Instantiate();
				//myPlayer = newPlayer.GetComponent<HHPlayer>();
				//myPlayer.BootstrapPlayer(playerInfo);

				playerSeated = true;

				// update name in player settings
				GameViewManager.instance.playerInfoView.SendMessage("OnSeatGranted", playerInfo);

				// update name in footer bar
				GameViewManager.instance.footerPanel.SendMessage("UpdatePlayerInfo", playerInfo);

			}

		}


		void OnPlayerMessage (JSONObject message)
		{
			Debug.Log("inside HH playerdescriptor delegate function");
		}
		//
		//		void Start() {
		//			playerPrefab = (GameObject)Resources.Load ("Prefabs/HHPlayer", typeof(GameObject));
		//			playerIconPrefab = (GameObject)Resources.Load ("Prefabs/HHPlayerIcon", typeof(GameObject));
		//		}
		//
		//		void OnEnable(){
		//			WebsocketClient.instance.PlayerDescriptorData += PlayerDescriptorFunction;
		//			WebsocketClient.instance.ZoneDescriptorData += ZoneDescriptorFunction;
		//		}
		//
		//		void OnDisable(){
		//			WebsocketClient.instance.PlayerDescriptorData -= PlayerDescriptorFunction;
		//			WebsocketClient.instance.ZoneDescriptorData -= ZoneDescriptorFunction;
		//		}
		//
		//		//this is called whenever a new player is added to the TT
		//		public void PlayerDescriptorFunction(object data){
		//
		//			Debug.Log ("inside HH playerdescriptor delegate function");
		//
		//			//if players not seated, show the options for players to join
		//			if (!playerSeated) {
		//				JSONObject newObj = new JSONObject (data.ToString ());
		//				//TODO: better parsing
		//				PlayerDescriptor newDescriptor = new PlayerDescriptor (newObj.GetField("data"));
		//
		//				Debug.Log ("got this newDescriptor parsed: " + newDescriptor.playerName);
		//
		//				Debug.Log ("complete original json object: " + newObj.ToString ());
		//
		//				Debug.Log ("just checking if we have guid: " + newDescriptor.playerGuid);
		//
		//				bool weHavePlayer = false;
		//
		//				foreach (HHPlayerSelector pSelector in potentialPlayers) {
		//					if (pSelector.myPlayerDescriptor.playerName == newDescriptor.playerName) {
		//						weHavePlayer = true;
		//					}
		//				}
		//
		//
		//				if (!weHavePlayer) {
		//					Debug.Log ("we got a new player on our potential roster!");
		//					GameObject newPlayerIcon = Instantiate (Resources.Load("Prefabs/PlayerSelectIcon")) as GameObject;
		//					newPlayerIcon.GetComponent<HHPlayerSelector> ().myPlayerDescriptor = newDescriptor;
		//
		//					potentialPlayers.Add (newPlayerIcon.GetComponent<HHPlayerSelector> ());
		//				}
		//
		//			}
		//		}
		//
		//		public void ZoneDescriptorFunction(JSONObject packet){
		//			Debug.Log ("Got Zone description: " + packet);
		//
		//
		//			if (myPlayer != null) {
		//				if (packet.GetField ("data").GetField ("playerOwnerGuid").str == myPlayer.myPlayerData.playerGuid) {
		//					Debug.Log ("we have a guid match! giving zone descriptor to all player's zones");
		//					myPlayer.SetThrowZones (packet.GetField("data"));
		//
		//				} else {
		//					Debug.Log ("this zone decriptor data is not meant for us, guid is: " + packet.GetField ("data").GetField ("playerOwnerGuid").str);
		//				}
		//			}
		//
		//		}
		//
		//


	}
}