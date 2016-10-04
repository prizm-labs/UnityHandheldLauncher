using UnityEngine;
using System.Collections;
using Prizm;

// GameViewController handles rendering/messagaing for all the pieces and zones

public class GameViewController : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// listen for zone, piece, and player messages from tabletop

		WebsocketMessageQueue.instance.AddHandler(Topics.PlayerDescriptor, OnPlayerMessage);
		WebsocketMessageQueue.instance.AddHandler(Topics.ZoneDescriptor, OnZoneMessage);
		WebsocketMessageQueue.instance.AddHandler(Topics.PieceDescriptor, OnPieceMessage);
	}

	void OnPlayerMessage(JSONObject message)
	{

	}

	void OnZoneMessage(JSONObject message)
	{
			
	}
		                                          
	void OnPieceMessage(JSONObject message)
	{

	}

	// Update is called once per frame
	void Update () {
	
	}

}
