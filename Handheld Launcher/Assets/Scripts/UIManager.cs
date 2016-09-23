using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject welcomeView;
	public GameObject pairingSetupView;
	public GameObject pairingLobbyView;
	public GameObject sessionLobbyView;
	public GameObject gameViewLandscapeView;

	private GameObject currentView;
	private GameObject lastView;
	//private GameObject nextView;

	// Use this for initialization
	void Start () {
	}

	public void goToForwardToView (GameObject nextView) {

		lastView = currentView;
		currentView = nextView;

		lastView.GetComponent<Animator> ().Play ("SlideOutLeft");
		currentView.GetComponent<Animator> ().Play ("SlideInRight");
	}

	public void goToBackwardToView (GameObject nextView) {

		lastView = currentView;
		currentView = nextView;

		lastView.GetComponent<Animator> ().Play ("SlideOutRight");
		currentView.GetComponent<Animator> ().Play ("SlideInLeft");
	}

	public void goForwardToPairingSetup() {
		goToForwardToView (pairingSetupView);
	}

	public void goForwardToPairingLobby() {
		goToForwardToView (pairingLobbyView);
	}

	public void goForwardToSessionLobby() {
		goToForwardToView (sessionLobbyView);
	}

	public void goForwardTogameViewLandscapeView() {

		Screen.orientation = ScreenOrientation.Landscape;

		goToForwardToView (gameViewLandscapeView);
	}

	void Awake () {

		Screen.orientation = ScreenOrientation.Portrait;

		currentView = welcomeView;

		welcomeView.GetComponent<Animator> ().speed = 2;
		pairingSetupView.GetComponent<Animator> ().speed = 2;
		pairingLobbyView.GetComponent<Animator> ().speed = 2;
		sessionLobbyView.GetComponent<Animator> ().speed = 2;
		gameViewLandscapeView.GetComponent<Animator> ().speed = 2;

		// show PairingSetup View

			// check if wifi enabled
			// if wifi enabled,
			// show discovery success
			// and show pairingLobby view

			// if WiFi not enabled,
			// on "Reload" button press,
				// check if WiFi enabled
			

		// Show pairing lobby View			
			
			// start network discovery

			// on click join game
			// save tabletop network info
			// switch to Session Lobby


			// if timeout
			// show session discovery failed
			// on click reload
				// start network discovery

		// Show SessionLobby view

			// request seat for session
			// on request success, 
			// show seat info

			// on request fail, 
			// on "Reload" button press
				// request seat for session 

			// if connected,
			// on "leave" button press,
			// leave session
	}
}
