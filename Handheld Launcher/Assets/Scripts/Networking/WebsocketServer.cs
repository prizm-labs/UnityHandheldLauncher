using System;
using System.Threading;
using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

namespace Prizm {
	public class WebsocketServer : MonoBehaviour {
	    public static WebsocketServer instance;
		public WebSocketSessionManager playerSessions;

		public delegate void DataReceiveDelegate(JSONObject data);
		public DataReceiveDelegate PieceDescriptorData;
		public DataReceiveDelegate PlayerDescriptorData;
		public DataReceiveDelegate ZoneDescriptorData;

		public delegate void PlayerSeatDelegate();

		public PlayerSeatDelegate updatePlayerSeatOnConnect;

		[System.NonSerialized]
		public List<string> messageLog = new List<string> ();

		public string wsMessage = "";
		public static int port = 4649;

		WebSocketServer wssv = new WebSocketServer(port);

	    void Awake() {
	        if (instance == null)
	        {
				DontDestroyOnLoad (gameObject);
	            instance = this;
	        }
	        else
	            Destroy(gameObject);
	    }

		void Start () {
			String PlayerServicePath = "players";
			wssv.AddWebSocketService<PlayerService>("/"+ PlayerServicePath);

			wssv.Start();
			playerSessions = wssv.WebSocketServices["/" + PlayerServicePath].Sessions;
	    }


	    void Update () {		
			if (messageLog.Count > 0) {	//if we have messages in our backlog, we have to process them in the main thread
				int numMessages = messageLog.Count;

				for (int i = 0; i < numMessages; i++) {
					wsMessage = messageLog [i];

					JSONObject jo = new JSONObject (wsMessage);
					switch (jo ["type"].str) {

					case("PlayerDescriptor"):
						PlayerDescriptorData (jo);
						break;
					case("PieceDescriptor"):
						PieceDescriptorData (jo);
						break;
					case("ZoneDescriptor"):
						Debug.Log ("zonedescriptor");
						ZoneDescriptorData (jo);
						break;

					default:
						break;
					}


				}

				messageLog.RemoveRange (0, numMessages);
			}
		}

		//send state via JSONObject
		public void sendPlayerUpdate(JSONObject data)
		{  
			playerSessions.Broadcast(data.ToString());
		}

		//send state via JSONObject
		public void BroadcastData(JSONObject data)
		{  
			Debug.Log ("broadcasting Data: " + data.ToString());
			playerSessions.Broadcast(data.ToString());
		}

		//send state via JSONObject
		public void sendPieceUpdate(JSONObject data)
		{  
			playerSessions.Broadcast(data.ToString());
		}

	}



	public class PlayerService : WebSocketService{
		
		//when player login, send them update of gamestate
		protected override void OnOpen()
		{
			Debug.LogError("a player joined the session");
			// TTManager.instance.SendAllPlayersData ();
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			
			//WebsocketServer.instance.wsMessage = e.Data;
			WebsocketServer.instance.messageLog.Add (e.Data);

		}

		protected override void OnClose(CloseEventArgs e)
		{
			
		}
	}
}
