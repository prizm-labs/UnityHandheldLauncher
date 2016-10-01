using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using Prizm;

public class DiceMacros : MonoBehaviour {

	DicePiece myDicePiece;

	void Awake() {
		myDicePiece = GetComponent<DicePiece> ();
		if (myDicePiece == null) {
			Debug.LogError ("we don't have a dicepiece with a dice macro on " + name);
		}
	}

	//User-defined macros go into Start
	void Start () {
		myDicePiece.FlickMacro = DiceFlick;
	}
		
	void DiceFlick ()
	{
		Vector3 flickDirection = new Vector3(GetComponent<FlickGesture>().ScreenFlickVector.x, 50.0f, GetComponent<FlickGesture>().ScreenFlickVector.y);
		GetComponent<Rigidbody> ().AddForce (flickDirection);
		GetComponent<Rigidbody> ().AddTorque (flickDirection);
	}
		
}
