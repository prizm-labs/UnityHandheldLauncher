﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Prizm;


public class PlayerSettingsController : MonoBehaviour {

	public GameObject availableSeatsView;
	public GameObject playerNameField;

	NetworkingManager networkingManager;

	List<HHPlayerSelector> availableSeats;

	int ListRowSeatCount = 4;

	// Paths for Prefabs & object heirarchy
	// !!! UPDATE THESE IF PREFABS STRUCTURE OR RESORUCE PATHS CHANGE!!!
	static string availableSeatEntryTemplatePath = "Prefabs/UIComponents/AvailableSeatEntry";


	static string availableSeatListObjectPath = "SeatsAvailable/ScrollView/Panel";
	static string seatNameObject = "SeatName";
	static string seatButtonObject = "Button";

	// Use this for initialization
	void Start () {
	
		networkingManager = GameObject.Find (GlobalObjects.NetworkManagerObject).GetComponent<NetworkingManager> ();

		// listen for seats available event
		//GameObject.Find (GlobalObjects.NetworkManagerObject).GetComponent<WebsocketClient>().
		//          messageQueue.AddHandler (Topics.SeatsAvailable, OnAvailableSeatsMessage);
		WebsocketMessageQueue.instance.AddHandler(Topics.SeatsAvailable, OnAvailableSeatsMessage);
		WebsocketMessageQueue.instance.AddHandler(Topics.SeatRequest, OnSeatRequestMessage);
	}

			
	// Update is called once per frame
	void Update () {
	
	}

	void OnSeatRequestMessage (JSONObject data)
	{
		Debug.Log("OnSeatRequestMessage: "+ data.ToString());
		
		PlayerDescriptor playerInfo = new PlayerDescriptor(data.GetField("data"));

		bool seatWasGranted = data.GetField("granted").b;

		if (seatWasGranted)
		{
			Debug.Log("seat was granted");
			// set player name in display field
			playerNameField.GetComponent<InputField>().text = playerInfo.playerName;

			// switch to game view
			GameObject.Find(GlobalObjects.GameViewManagerObject).SendMessage("HideInfoView");
		}
		else {
			Debug.Log("seat was denied");
		}

	}

	void DisplaySeats(List<PlayerDescriptor> seats)
	{

		Transform SeatsListPanel = transform.FindChild(availableSeatListObjectPath);
		GameObject availableSeatTemplate = Instantiate(Resources.Load(availableSeatEntryTemplatePath)) as GameObject;

		// get height and width of template to calculate offset of each instance in list
		float seatTemplateWidth = availableSeatTemplate.transform.GetComponent<RectTransform>().rect.width;
		float seatTemplateHeight = availableSeatTemplate.transform.GetComponent<RectTransform>().rect.height;

		foreach (Transform child in SeatsListPanel)
		{
			Destroy(child.gameObject);
		}


		// TODO show loading animation...

		// hide listing panel while rendering items
		SeatsListPanel.gameObject.SetActive(false);

		int seatsAvailableCount = 0;

		foreach (PlayerDescriptor seat in seats)
		{
			if (!seat.playerSeated)
			{
				seatsAvailableCount++;
			}
		}

		// resize listings panel height to fit number of listings
		int rowCount = (seatsAvailableCount < ListRowSeatCount) ? 1 :
		                (seatsAvailableCount % ListRowSeatCount > 0) ?
		                (seatsAvailableCount / ListRowSeatCount + 1) :
					   (seatsAvailableCount / ListRowSeatCount);
		
		float panelHeight = rowCount * seatTemplateHeight;
		float panelWidth = Math.Min(ListRowSeatCount, seatsAvailableCount)*seatTemplateWidth;
		SeatsListPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth, panelHeight);

		// set panel view at top of scroll area
		SeatsListPanel.GetComponent<RectTransform>().localPosition = new Vector3( 0, - (rowCount % 2 > 0 ?
		                                                                                rowCount / 2 + 1 : 
		                                                                                rowCount /2) * 
		                                                                         seatTemplateHeight/2, 0);

		Debug.Log(String.Format("seats available: {0} PanelHeight {1} PanelWidth {2}", seatsAvailableCount, panelHeight, panelWidth));

		SeatsListPanel.gameObject.SetActive(true);

		int index = 0;

		foreach (PlayerDescriptor seat in seats)
		{
			if (!seat.playerSeated)
			{
				float xOffset = GetSeatXOffset(index, seatTemplateWidth, panelWidth);
				float yOffset = GetSeatYOffset(index, seatTemplateHeight, panelHeight);

				CreateSeatButton(seat, SeatsListPanel, xOffset, yOffset);

				index++;
			}

		}


		Destroy(availableSeatTemplate);
	}

	float GetSeatXOffset (int seatIndex, float seatWidth, float panelWidth)
	{
		return ((seatIndex % ListRowSeatCount) * seatWidth) - panelWidth / 2 + seatWidth/2 ;
	}

	float GetSeatYOffset(int seatIndex, float seatHeight, float panelHeight)
	{
		int rowIndex = (seatIndex - seatIndex % ListRowSeatCount) / ListRowSeatCount;
		Debug.Log(rowIndex);
		return ( rowIndex * -seatHeight) + panelHeight / 2 - seatHeight/2;
	}

	void CreateSeatButton(PlayerDescriptor seatInfo, Transform SeatsListPanel, float xOffset, float yOffset)
	{

		Debug.Log(string.Format("creating seat: {0} {1} {2}", xOffset, yOffset, seatInfo.playerName));
		GameObject availableSeat = Instantiate(Resources.Load(availableSeatEntryTemplatePath)) as GameObject;

		availableSeat.transform.SetParent(SeatsListPanel);
		availableSeat.transform.GetComponent<RectTransform>().localPosition = new Vector2(xOffset, yOffset);

		availableSeat.transform.Find(seatNameObject).GetComponent<Text>().text = seatInfo.playerName;
		availableSeat.transform.Find(seatButtonObject).GetComponent<Button>().onClick.AddListener(
			() =>
			{
			GameObject.Find(GlobalObjects.HandheldManagerObject).SendMessage("RequestSeatForPlayer",seatInfo);

				//TODO show request loading 
				
				// hide seat panel list
				availableSeatsView.SetActive(false);

				// the rest of this UI flow will be resolved once the server responds 
				// with granting or denying seat request
			}
		);
	}

	// Receving a list of all available seats from Tabletop Client
	void OnAvailableSeatsMessage (JSONObject message) {


		Debug.Log ("we got a new player on our potential roster!");

		List<PlayerDescriptor> allSeats = new List<PlayerDescriptor>();
		List<JSONObject> allSeatsData = message.GetField("data").list;

		foreach (JSONObject seat in allSeatsData)
		{
			Debug.Log("seat data");
			Debug.Log(seat.ToString());
			allSeats.Add(new PlayerDescriptor(seat));
		}

		DisplaySeats(allSeats);

		//newPlayerIcon.GetComponent<HHPlayerSelector> ().myPlayerDescriptor = newDescriptor;

		//availableSeats.Add (newPlayerIcon.GetComponent<HHPlayerSelector> ());


	}

}
