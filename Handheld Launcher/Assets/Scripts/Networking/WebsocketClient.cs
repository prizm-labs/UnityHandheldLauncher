using UnityEngine;
using System;
using System.Collections;
using WebSocketSharp;
using UnityEngine.UI;
using System.Collections.Generic;

public class WebsocketClient : MonoBehaviour {
	public static WebsocketClient instance;

	public delegate void DataReceiveDelegate(JSONObject data);
	public DataReceiveDelegate PieceDescriptorData;
	public DataReceiveDelegate PlayerDescriptorData;
	public DataReceiveDelegate ZoneDescriptorData;

	public string ip;
	public string sessionName;
	public string websocketService = "players";

	private bool debugging = true;	//displays the websocket message in text 

	private List<string> messageLog = new List<string>();

	public WebSocket ws;
	public string wsMessage = "";
	void Awake(){
		if (instance == null) {
			instance = this;
		} else
			Destroy (gameObject);
	}


	void Update() {
		if (messageLog.Count > 0) {	//if we have messages in our backlog, we have to process them in the main thread
			int numMessages = messageLog.Count;

			for (int i = 0; i < numMessages; i++) {
				wsMessage = messageLog [i];
				if (debugging) {
					GameObject.Find ("Canvas/raw").GetComponent<Text> ().text = "raw ws message: '" + wsMessage + "';";
					try {
						JSONObject jsonText = new JSONObject (wsMessage);
						GameObject.Find ("Canvas/json").GetComponent<Text> ().text = "message in json: '" + jsonText.ToString () + "';";
					} catch (System.Exception e) {
						Debug.LogWarning ("probably improperly formatted json in debugging");
						Debug.LogWarning (e);
					}

				}

				JSONObject d = new JSONObject (wsMessage);
				//Debug.Log ("received object from json!\n" + d.ToString ());
				Debug.Log ("receieved packet type: " + d.GetField ("type").str);
				switch (d.GetField ("type").str) {
				case("PlayerDescriptor"):
					Debug.Log ("received a 'PlayerDescriptor'");
					PlayerDescriptorData (d);
					break;
				case("ZoneDescriptor"):
					Debug.Log ("got into a 'ZoneDescriptor'");
					ZoneDescriptorData (d);
					break;
				case("PieceDescriptor"):
					PieceDescriptorData (d);
					break;
				default:
					break;

				}
			}

			messageLog.RemoveRange (0, numMessages);
		}
	}

	public void BeginConnection(string ipAddress, string name){
		ip = ipAddress;
		sessionName = name;
		ws = new WebSocket("ws://"+ip+":4649/"+websocketService);

		//var ver = Application.unityVersion;
		ws.OnOpen += (sender, e) =>
		{
		};

		ws.OnMessage += (sender, e) =>
		{
			//wsMessage = e.Data;
			Debug.Log("got a message: " + wsMessage);
			messageLog.Add(e.Data);
		};

		ws.OnError += (sender, e) =>
		{
			Debug.LogError(e.Message);
			Debug.LogError(e);
		};        

		ws.OnClose += (sender, e) => {
			Debug.LogError("Closing");
			ws.Send(String.Format("Player disconnected"));

		};
		
		ws.Connect();
	}

	public void BroadcastData(JSONObject data){
		Debug.Log ("broadcasting data: " + data.ToString());
		ws.Send (data.ToString ());
	}


	public void CancelConnection(){
		ws.Close ();
	}

}
