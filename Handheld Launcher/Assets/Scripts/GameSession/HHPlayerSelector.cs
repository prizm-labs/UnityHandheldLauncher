using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using UnityEngine.UI;

namespace Prizm {
	public class HHPlayerSelector : MonoBehaviour {

		private PlayerDescriptor _myPlayerDescriptor;
		public PlayerDescriptor myPlayerDescriptor {
			get { return _myPlayerDescriptor; }
			set { 
				
				_myPlayerDescriptor = value;
				//Debug.Log ("player descriptor guid: " + _myPlayerDescriptor.playerGuid.ToString ());
				UpdateUI ();
			}
		}

		

		void OnEnable() {
			GetComponent<TapGesture>().Tapped += TapHandler;
		}

		void OnDisable() {
			GetComponent<TapGesture>().Tapped -= TapHandler;
		}

		//if we are trying to select a player who is available
		void TapHandler (object sender, System.EventArgs e)
		{
			Debug.Log ("tapped");
			if (!myPlayerDescriptor.playerSeated) {
				//HHManager.instance.SelectThisPlayer (myPlayerDescriptor);
			}
		}

		//TODO: make this neater (not using .findchild)
		void UpdateUI() {
			Debug.Log ("updating player dock ui choices");
			transform.FindChild ("Canvas/playerName").GetComponent<Text> ().text = "name: " + myPlayerDescriptor.playerName;
			transform.FindChild ("Canvas/victoryPoints").GetComponent<Text> ().text = "coins: " + myPlayerDescriptor.gameSpecificStats.coins;
		}
	}
}