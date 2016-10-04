using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Prizm;

public class PairingLobbyController : MonoBehaviour {

	public GameObject sessionsAvailableView;
	public GameObject sessionDiscoveryInfo;
	public GameObject discoveryFailedNotification;


	// Prefab heirarchy and Resources paths
    static string sessionsListObjectPath = "SessionAvailable/ScrollView/Panel";
    static string sessionEntryTemplatePrefabPath = "Prefabs/UIComponents/SessionEntry";

	static string sessionEntryNameObject = "SessionName";
	static string sessionEntryButtonObject = "JoinButton";

	// Use this for initialization
	void Start()
	{

		discoveryFailedNotification.SetActive(false);
		sessionDiscoveryInfo.SetActive(false);
		sessionsAvailableView.SetActive(false);

		NetworkingManager.instance.onSessionsChanged += DisplayGames;
	}


	// VIEW TRANSITIONS
	//=================


    public void OnEnter() 
	{
		sessionDiscoveryInfo.SetActive(true);
	}


	// RENDERING METHODS
	//==================


    public void DisplayGames (Dictionary<string, GameToJoin> AvailableGames)
    {
		sessionDiscoveryInfo.SetActive (false);
		sessionsAvailableView.SetActive (true);

		Transform SessionsListPanel = transform.FindChild(sessionsListObjectPath);
		GameObject sessionEntryTemplate = Instantiate(Resources.Load(sessionEntryTemplatePrefabPath)) as GameObject;

		float sessionEntryTemplateHeight = sessionEntryTemplate.transform.GetComponent<RectTransform>().rect.height;
	

		// clear all session entry listings
		foreach (Transform child in SessionsListPanel)
        {
            Destroy(child.gameObject);
        }
        
		// resize listings panel height to fit number of listings
        SessionsListPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(300, sessionEntryTemplateHeight * AvailableGames.Count);


		// TODO top offset is half the height of the scrollable view
		float viewHeight = SessionsListPanel.GetComponent<RectTransform>().rect.height;
		float topOffset = viewHeight / 2;


		int index = 0;

        foreach (GameToJoin game in AvailableGames.Values)
        {
            // TODO calculate (vertical) offset for each subsequent button in sessions list
            // ~230px Y
			float entryOffset = topOffset + index*sessionEntryTemplateHeight;
			CreateGamesButton(game, SessionsListPanel, entryOffset);
        }

		Destroy(sessionEntryTemplate);
    }

	void CreateGamesButton(GameToJoin game, Transform SessionsListPanel, float topOffset)
    {
		GameObject sessionEntry = Instantiate(Resources.Load(sessionEntryTemplatePrefabPath)) as GameObject;

		sessionEntry.transform.SetParent(SessionsListPanel);
		sessionEntry.transform.GetComponent<RectTransform>().localPosition = new Vector2(0, topOffset);

		sessionEntry.transform.Find(sessionEntryNameObject).GetComponent<Text>().text = game.roomName + "\n" + game.LocalIp;
		sessionEntry.transform.Find(sessionEntryButtonObject).GetComponent<Button>().onClick.AddListener( 
			() => {
				GameObject.Find(GlobalObjects.AppManagerObject).SendMessage("LoadGameScene");
			}
		);
    }

}
