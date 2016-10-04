using UnityEngine;
using System.Collections;

public class FooterViewController : MonoBehaviour {

	public GameObject playerName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdatePlayerName(string name)
	{
		playerName.GetComponent<GUIText>().text = name;
	}
}
