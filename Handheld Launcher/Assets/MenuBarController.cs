using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prizm;

public class MenuBarController : MonoBehaviour {


	public GameObject settingsButton;
	public GameObject gameViewButton;
	public GameObject quickInfoButton;
	public GameObject rulesButton;

	// Use this for initialization
	void Start () {

		settingsButton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(
			() =>
			{
				Debug.Log("OnCLick settings button");
				GameViewManager.instance.SendMessage("ShowInfoView");
			}
		);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
