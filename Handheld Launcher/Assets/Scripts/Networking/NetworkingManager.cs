using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Prizm;


public class NetworkingManager : MonoBehaviour {

	NetworkDiscovery networkDiscovery;
	WebsocketClient websocketClient;

	public Dictionary<string, GameToJoin> AvailableGames = new Dictionary<string, GameToJoin>();

	// Use this for initialization
	void Start () {

		// Bootstrapping 
		gameObject.AddComponent<NetworkDiscovery> ();
		//gameObject.AddComponent<WebSocketManager> ();
		gameObject.AddComponent<WebsocketClient> ();

		networkDiscovery = GetComponent<NetworkDiscovery> ();

		networkDiscovery.InitializeAsClient ();
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

            GameObject.Find("PairingLobby").SendMessage("DisplayGames", AvailableGames);
        }
        
    }

    public void UnregisterSession(GameToJoin existingGame)
    {
        if (AvailableGames.ContainsKey(existingGame.LocalIp))
        {
            AvailableGames.Remove(existingGame.LocalIp);
            GameObject.Find("PairingLobby").SendMessage("DisplayGames", AvailableGames);
        }
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
