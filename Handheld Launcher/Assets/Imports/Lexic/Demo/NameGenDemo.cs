using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NameGenDemo : MonoBehaviour {

    public GameObject nameGenObject;
    private Text txt;

	// Use this for initialization
	void Start () {
        txt = GetComponent<Text>();
        string newtext = "";
        for (int i = 0; i < 10; i++)
			newtext += "\n" + Lexic.NameGenerator.GetNextRandomName();
        txt.text = newtext;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
