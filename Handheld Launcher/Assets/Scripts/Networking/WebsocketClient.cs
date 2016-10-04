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

	public string connectionID;

	static string websocketService = "players";
	static int websocketPort = 4649;


	public WebsocketMessageQueue messageQueue;


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

	void Start() {

		//gameObject.AddComponent<WebsocketMessageQueue> ();
		messageQueue = gameObject.GetComponent<WebsocketMessageQueue> ();

		messageQueue.AddHandler (Topics.PlayerDescriptor, OnPlayerDescriptor);
		messageQueue.AddHandler(Topics.ClientRegistration, OnRegisterClient);
		messageQueue.AddHandler(Topics.ClientRegistration, OnRegisterClient);

		Debug.Log ("WS client start");
	}

	// Websocket serve sends each client conenciton its websocket connection ID
	// this client saves the conenction ID and will send it back along with the player ID (when available)
	// so the server can associate a websocket connection with a player
	void OnRegisterClient(JSONObject data)
	{
		Debug.Log("OnRegisterClient" + data.ToString());

		connectionID = data.GetField("data").str;
	}

	void OnPlayerDescriptor(JSONObject data) {
		Debug.Log ("Player descriptor"+data.ToString());
	}




	void Update() {
		
	}

	public void BeginConnection(string ipAddress){

		string wsUrl = String.Format("ws://{0}:{1}/{2}", ipAddress, websocketPort,websocketService );
		Debug.Log ("Connecting to Webscoket server @ " + wsUrl);

		ws = new WebSocket(wsUrl);

		//var ver = Application.unityVersion;
		ws.OnOpen += (sender, e) =>
		{
		};

		ws.OnMessage += (sender, e) =>
		{
			wsMessage = e.Data;
			messageQueue.QueueMessage(wsMessage);
		};

		ws.OnError += (sender, e) =>
		{
			Debug.LogError(e.Message);
			Debug.LogError(e);
		};        

		ws.OnClose += (sender, e) => {
			Debug.LogError("Closing");

			// server disconnected
			// TODO handle disconnection
			// show manual reconnect button
			// reconnect with cached IP
		};
		
		ws.Connect();
	}

	public void BroadcastData(JSONObject data){
		Debug.Log ("broadcasting data: " + data.ToString());
		ws.Send (data.ToString ());
	}

	public void SendToTabletop(JSONObject data)
	{
		// append conenction ID to messge

		Debug.Log("sending data to tabletop: " + data.ToString());
		ws.Send(data.ToString());
	}


	public void CancelConnection(){
		ws.Close ();
	}

}
