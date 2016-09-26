using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Prizm;

public class PairingLobbyController : MonoBehaviour {

	public GameObject sessionsAvailableView;
	public GameObject sessionDiscoveryInfo;
	public GameObject discoveryFailedNotification;

    static string sessionsListObject = "SessionAvailable/ScrollView/Panel";
    static string sessionButtonPrefab = "Prefabs/UIComponents/SessionEntry";


    public void OnEnter() {


	}


    public void DisplayGames (Dictionary<string, GameToJoin> AvailableGames)
    {
        Transform PanelListGames = transform.Find(sessionsListObject);

        foreach (Transform child in PanelListGames)
        {
            Destroy(child.gameObject);
        }

        Transform PanelView = transform.FindChild(sessionsListObject);

        GameObject btn = Instantiate(Resources.Load(sessionButtonPrefab)) as GameObject;

        float sessionEntryTemplateHeight = btn.transform.GetComponent<RectTransform>().rect.height;
        PanelView.GetComponent<RectTransform>().sizeDelta = new Vector2(300, sessionEntryTemplateHeight * AvailableGames.Count);

        // TODO top offset is half the height of the scrollable view
        float viewHeight = PanelView.GetComponent<RectTransform>().rect.height;
        float topOffset = viewHeight / 2;

        foreach (GameToJoin game in AvailableGames.Values)
        {
            // TODO calculate (vertical) offset for each subsequent button in sessions list
            // ~230px Y
            CreateGamesButton(btn, game, PanelView, topOffset);
        }
    }

    void CreateGamesButton(GameObject btn, GameToJoin game, Transform PanelView, float topOffset)
    {

       

        btn.transform.SetParent(PanelView);
        //btn.transform.position = new Vector3(0, topOffset, 0);
        btn.transform.GetComponent<RectTransform>().localPosition = new Vector2(0, topOffset);

        btn.transform.Find("SessionName").GetComponent<Text>().text = game.roomName + "\n" + game.LocalIp;
        
        //btn.GetComponent<Button>().onClick.AddListener(() => WebsocketClient.instance.BeginConnection(game.LocalIp, game.roomName));
        //btn.GetComponent<Button>().onClick.AddListener(()=> ShowRoomInfo());
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
