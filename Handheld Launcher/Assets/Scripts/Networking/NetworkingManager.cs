using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Prizm;


public class NetworkingManager : MonoBehaviour {

	public static NetworkingManager instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
			Destroy(gameObject);
	}

	NetworkDiscovery networkDiscovery;
	public WebsocketClient websocketClient;

	public delegate void OnSessionsChanged( Dictionary<string, GameToJoin> AvailableGames );
	public OnSessionsChanged onSessionsChanged;

	public Dictionary<string, GameToJoin> AvailableGames = new Dictionary<string, GameToJoin>();

	// Use this for initialization
	void Start () {

		Debug.Log ("newtowrk manager Start");

		// Bootstrapping 
		networkDiscovery = GetComponent<NetworkDiscovery> ();
		websocketClient = GetComponent<WebsocketClient> ();

		networkDiscovery.InitializeAsClient ();

		onSessionsChanged += (Dictionary<string, GameToJoin> availableGames) => {};

		tryAutoWebsocketClientConnection ();
	}

	// allow automatic websocket client connection
	// if cached or preset for development
	public void tryAutoWebsocketClientConnection() {
		
		string cachedIPAddress = GameObject.Find("AppManager").GetComponent<AppManager>().currentSessionIPAddress;

		Debug.Log ("current IP address: " + cachedIPAddress);

		websocketClient.BeginConnection (cachedIPAddress);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    // SendMessage Receivers
    //======================

    public void ClientDiscoveryStarted()
    {

        Debug.Log("ClientDiscoveryStarted");
    }

    public void RegisterSession(GameToJoin newGame)
    {
        if (!AvailableGames.ContainsKey(newGame.LocalIp)) {
            AvailableGames.Add(newGame.LocalIp, newGame);
            Debug.Log(string.Format("new session found: {0} {1}", newGame.LocalIp, newGame.roomName));

			onSessionsChanged( AvailableGames );
        }
        
    }

    public void UnregisterSession(GameToJoin existingGame)
    {
        if (AvailableGames.ContainsKey(existingGame.LocalIp))
        {
            AvailableGames.Remove(existingGame.LocalIp);
            
			onSessionsChanged( AvailableGames );
			//GameObject.Find("PairingLobby").SendMessage("DisplayGames", AvailableGames);
        }
    }

	public void JoinSession(GameToJoin game) {

		Debug.Log (string.Format("JoinSession {0} {1}",game.LocalIp, game.roomName) );
		Debug.Log (game);

		websocketClient.BeginConnection(game.LocalIp);
	}

	public void RequestSeat(PlayerDescriptor seatInfo)
	{
		Debug.Log("RequestSeat" + seatInfo.ToString());

		JSONObject seatRequest = new JSONObject();
		seatRequest.AddField("type", "SeatRequested");
		seatRequest.AddField("player", seatInfo.playerGuid);

		// send websocket connection ID 
		// so server can associate the connection ID with the player ID
		seatRequest.AddField("connection", websocketClient.connectionID);

		websocketClient.SendToTabletop(seatRequest);
	}


    // UTILITIES
    //==========

    public bool HasInternetLANConnection()
	{
		bool isConnectedToInternet = false;
		// Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
		if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			isConnectedToInternet = true;
		}
		return isConnectedToInternet;
	}


	//		To check for Internet Connection,
	IEnumerator CheckForConnection() 
	{
		Ping png = new Ping("139.130.4.5");
		float startTime = Time.time;      
		while (Time.time < startTime + 5.0f)       
		{         
			yield return new WaitForSeconds(0.1f);      
		}
		if(png .isDone)      
		{
			print("Connected!");  
		}     
		else    
		{    
			print("Not Connected!");
		}
	}

}
