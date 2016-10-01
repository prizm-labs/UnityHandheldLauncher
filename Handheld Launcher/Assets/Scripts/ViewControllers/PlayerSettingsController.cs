using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prizm;


public class PlayerSettingsController : MonoBehaviour {

	NetworkingManager networkingManager;

	List<HHPlayerSelector> availableSeats;

	// Paths for Prefabs & object heirarchy
	static string availableSeatEntryTemplatePath = "Prefabs/UIComponents/AvailableSeatEntry";


	// Use this for initialization
	void Start () {
	
		networkingManager = GameObject.Find ("NetworkManager").GetComponent<NetworkingManager> ();

		// listen for seats available event
		GameObject.Find ("NetworkManager").GetComponent<WebsocketClient>().messageQueue.AddHandler (Topics.SeatsAvailable, OnAvailableSeatsMessage);
		//networkingManager.websocketClient.messageQueue.AddHandler(Topics.SeatsAvailable, OnAvailableSeatsMessage);
	}

			
	// Update is called once per frame
	void Update () {
	
	}


	// Receving a list of all available seats from Tabletop Client
	void OnAvailableSeatsMessage (JSONObject message) {


		Debug.Log ("we got a new player on our potential roster!");
		GameObject newPlayerIcon = Instantiate (Resources.Load(availableSeatEntryTemplatePath)) as GameObject;
		//newPlayerIcon.GetComponent<HHPlayerSelector> ().myPlayerDescriptor = newDescriptor;

		availableSeats.Add (newPlayerIcon.GetComponent<HHPlayerSelector> ());


	}

}
