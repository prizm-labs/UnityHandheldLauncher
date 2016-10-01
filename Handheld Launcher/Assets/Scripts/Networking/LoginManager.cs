using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Prizm {
	public class LoginManager : MonoBehaviour {
		public enum Seats{red=0,blue,green,yellow,orange,black,white,teal};
		public static LoginManager instance;
		public Dictionary<string, GameToJoin> AvailableGames = new Dictionary<string, GameToJoin>();

		public string gameName;
		public bool Hosting = false;
		public string HHSceneName = "";

		void Awake(){
			if (instance == null) {
				instance = this;
			} else
				Destroy (gameObject);
		}

		void Start(){
			if (!Hosting) {
				Debug.LogError ("Joining game");
				JoinGame ();
			}
		}

		#region Host Stuff

		public void HostGame(){
			NetworkDiscovery.instance.Initialize ();
			NetworkDiscovery.instance.StartAsServer ();
			Transform PanelConnection = transform.FindChild ("Pnl_Connection");
			PanelConnection.gameObject.SetActive(false);
			SetGameName();
			WebSocketManager.instance.StartAsServer ();
			CreateTTManager ();
		}

		public void CreateTTManager(){
			//GameObject emptyGameObject = new GameObject ("TTManager");
			//emptyGameObject.AddComponent<TTManager> ();
			SceneManager.LoadScene ("TT_Scene");
		}

		public void CreateMainMenuButton(string buttonName, string msg){
			Transform PanelConnection = transform.FindChild ("Pnl_Connection");
			GameObject btn = Instantiate (Resources.Load ("PlayTablePrefabs/UI/MainMenuButton/Btn_MainMenu")) as GameObject;
			btn.transform.SetParent (PanelConnection);

			btn.transform.FindChild ("Text").GetComponent<Text> ().text = buttonName;
			btn.GetComponent<Button> ().onClick.AddListener (() => gameObject.BroadcastMessage(msg));
			//btn.GetComponent<Button> ().onClick.AddListener (() => HostGame());
		}

		public void SetGameName(){
			gameName = NetworkDiscovery.instance.RoomName;
		}
		#endregion 


		#region client stuff
		void JoinGame(){
			NetworkDiscovery.instance.Initialize ();
			NetworkDiscovery.instance.StartAsClient ();
			WebSocketManager.instance.StartAsClient ();
			ShowAvailableGames ();
			CreateHHManager ();
		}

		void ShowAvailableGames(){
			DisplayGames ();
		}

		void CreateGamesButton(GameToJoin game){
			Transform PanelButton = transform.FindChild ("Pnl_NetworkGames/Pnl_ListGames");
			GameObject btn = Instantiate (Resources.Load ("PlayTablePrefabs/UI/RoomButton/Btn_Room")) as GameObject;
			btn.transform.SetParent (PanelButton);
			btn.transform.FindChild ("Text").GetComponent<Text> ().text = game.roomName + "\n" + game.LocalIp;
//			btn.GetComponent<Button> ().onClick.AddListener (() => WebsocketClient.instance.BeginConnection (game.LocalIp, game.roomName));
			btn.GetComponent<Button>().onClick.AddListener(()=> ShowRoomInfo());
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
		public void RemoveGame(string ip){
			AvailableGames.Remove (ip);
			DisplayGames ();
		}
		public void AddGame(GameToJoin game){
			AvailableGames.Add (game.LocalIp, game);
			DisplayGames ();		
		}
		public void RefreshGames(){
			AvailableGames.Clear ();
			DisplayGames ();
		}

		void ShowRoomInfo(){
			Transform PanelNetworkGames = transform.FindChild ("Pnl_NetworkGames");
			Transform PanelConnection = transform.FindChild ("Pnl_Connection");
			Transform PanelGameInfo = transform.FindChild ("Pnl_Title");
			PanelConnection.gameObject.SetActive(false);
			PanelNetworkGames.gameObject.SetActive(false);
			PanelGameInfo.gameObject.SetActive (true);	
		}
		void StartHH(){
			SceneManager.LoadScene (HHSceneName);
		}

		void CreateHHManager(){
			GameObject emptyGameObject = new GameObject ();
			emptyGameObject.name = "HHManager";
			emptyGameObject.AddComponent<HHManager> ();
		}

		public void DisplaySeats(JSONObject data){
			Transform PanelButton = transform.FindChild ("Pnl_Title");
			foreach (Transform child in PanelButton) {
				Destroy (child.gameObject);
			}
			if (data.Count > 1) {
				for (int i = 1; i < data.Count ; i++) {
					AddSeat (data [i]);
				}
			}
		}		

		private void AddSeat(JSONObject d){
			Transform PanelButton = transform.FindChild ("Pnl_Title");
			GameObject btn = Instantiate (Resources.Load ("PlayTablePrefabs/UI/SeatButton/Btn_Seat")) as GameObject;
			btn.transform.SetParent (PanelButton);
			btn.transform.FindChild ("Text").GetComponent<Text> ().text = d["playerName"].str;
			Debug.LogError (d ["playerSeated"]);
			btn.GetComponent<Button> ().interactable = d ["playerSeated"].b;
			btn.GetComponent<Button> ().onClick.AddListener (() => WebsocketClient.instance.BroadcastData (seated (d)));
		}
		
		JSONObject seated(JSONObject d){
//			PlayerDescriptor pd = JsonUtility.FromJson<PlayerDescriptor> (d.ToString ());
//			pd.playerSeated = false;
//			JSONObject jo = new JSONObject (JsonUtility.ToJson(pd).ToString());
//			Debug.LogError (jo);
//			jo.AddField ("DataType", "PlayerDescriptor");
//			return jo;

			return new JSONObject ("{\"foo\":\"bar\"}");
		}

		void CreateRefreshButton(){
			Transform PanelNetworkGames = transform.FindChild ("Pnl_NetworkGames/Pnl_RefreshPanel");
			GameObject btn = Instantiate (Resources.Load ("PlayTablePrefabs/UI/RoomButton/RefreshGamesButton/Btn_Refresh")) as GameObject;
			btn.transform.SetParent (PanelNetworkGames);
			btn.GetComponent<Button> ().onClick.AddListener (() => RefreshGames ());
		}

		#endregion

	}
}