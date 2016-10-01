using UnityEngine;
using System.Collections;
using Prizm;
using TouchScript.Gestures;


public class CoupPlayerCoinHolderMacros : MonoBehaviour {

	private SnapZone mySnapZone;

	public TTPlayer myPlayer;


	void Awake() {
		mySnapZone = GetComponent<SnapZone> ();
		if (mySnapZone == null) {
			Debug.LogError ("we don't have a snapzone with a snap macro on " + name);
		}

		myPlayer = transform.parent.GetComponent<TTPlayer> ();
	}

	//User-defined macros go into Start
	void Start () {
		mySnapZone.PieceEnterMacro = GiveCoin;
		mySnapZone.PieceExitMacro = TakeCoin;
	}






	//lets go of coin
	//increases player's coin count
	//updates players UI and HH
	void GiveCoin(Piece coin) {
		Debug.Log ("Giving player coin");
		myPlayer.myPlayerData.gameSpecificStats.coins++;

		try {
			coin.GetComponent<TransformGesture> ().Cancel ();
			coin.GetComponent<Piece>().StopMoveMacro();
		} catch (System.NullReferenceException ex) {
			Debug.LogError ("collider " + coin.name + " does not have a TransformGesture");
			Debug.LogError (ex);
		}

		myPlayer.myDockZone.UpdateDock ();
		myPlayer.UpdateHHPlayerDesc ();

		Zone.MovePieceToZone (coin, mySnapZone);
	}

	//when a player takes coin from this zone
	//reduce coins from player
	//update HH and dock
	void TakeCoin(Piece coin) {
		Debug.Log ("taking player coin");
		myPlayer.myPlayerData.gameSpecificStats.coins--;


		myPlayer.myDockZone.UpdateDock ();
		myPlayer.UpdateHHPlayerDesc ();
	}

}
