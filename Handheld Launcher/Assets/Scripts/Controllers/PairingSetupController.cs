using UnityEngine;
using System.Collections;

public class PairingSetupController : MonoBehaviour {

	public GameObject discoveryLANNotification;
	public GameObject discoveryLANSuccess;
	public GameObject discoveryLANFailure;

	NetworkingManager networkingManager;
	UIManager uiManager;

	public void OnEnter() {
		discoveryLANFailure.SetActive (false);
		discoveryLANSuccess.SetActive (false);

		StartCoroutine (DiscoverLAN ());
	}

	public IEnumerator DiscoverLAN() {
		bool hasLANConnection = networkingManager.HasInternetLANConnection();

		yield return new WaitForSeconds(1.0f);
		discoveryLANNotification.SetActive (false);

		if (hasLANConnection) {

			discoveryLANFailure.SetActive (false);
			discoveryLANSuccess.SetActive (true);

			yield return new WaitForSeconds(1.0f);

			uiManager.goForwardToPairingLobby ();

		} else {


			discoveryLANFailure.SetActive (true);
			discoveryLANSuccess.SetActive (false);
		}
	}

	void Awake() {
		networkingManager = GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent<NetworkingManager> ();
		uiManager = GameObject.FindGameObjectWithTag ("UIManager").GetComponent<UIManager> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
