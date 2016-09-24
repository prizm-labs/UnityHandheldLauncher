using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Prizm;


public class NetworkingManager : MonoBehaviour {

	NetworkDiscovery networkDiscovery;
	WebsocketClient websocketClient;

	public Dictionary<string, GameToJoin> AvailableGames = new Dictionary<string, GameToJoin>();

	public void RegisterSession( GameToJoin newGame ) {
		//AvailableGames.Add (newGame.LocalIp, newGame.roomName);
	}

	public void UnregisterSession ( GameToJoin existingGame ) {
		if (AvailableGames.ContainsKey (existingGame.LocalIp)) {
			AvailableGames.Remove (existingGame.LocalIp);
		}
	}

	public void DisplayGames(){
		Transform PanelListGames = transform.FindChild ("Pnl_NetworkGames/Pnl_ListGames");		

		foreach (Transform child in PanelListGames) {
			Destroy (child.gameObject);
		}
		foreach(GameToJoin game in AvailableGames.Values){
			CreateGamesButton (game);
		}
	}

	void CreateGamesButton(GameToJoin game){
		Transform PanelButton = transform.FindChild ("Pnl_NetworkGames/Pnl_ListGames");
		GameObject btn = Instantiate (Resources.Load ("PlayTablePrefabs/UI/RoomButton/Btn_Room")) as GameObject;
		btn.transform.SetParent (PanelButton);
		btn.transform.FindChild ("Text").GetComponent<Text> ().text = game.roomName + "\n" + game.LocalIp;
		btn.GetComponent<Button> ().onClick.AddListener (() => WebsocketClient.instance.BeginConnection (game.LocalIp, game.roomName));
		//btn.GetComponent<Button>().onClick.AddListener(()=> ShowRoomInfo());
	}

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
