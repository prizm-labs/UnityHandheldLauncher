using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Prizm;

public class FooterViewController : MonoBehaviour {

	public GameObject playerName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdatePlayerInfo(PlayerDescriptor playerInfo)
	{
		playerName.GetComponent<Text>().text = playerInfo.playerName;
	}
}
