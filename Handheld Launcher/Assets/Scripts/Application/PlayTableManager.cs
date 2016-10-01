using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using UnityEngine.SceneManagement;


namespace Prizm {
	public class PlayTableManager : MonoBehaviour {
		public bool hosting = true;
		public bool loadFromConfig = false;

		// Use this for initialization
		void Awake () {
			PlayTableBootStrap ();
			//BootstrapNoNetworking ();

		}


		void PlayTableBootStrap(){
			GameObject mainMenu;
			if (hosting)
				mainMenu = Instantiate (Resources.Load ("PlayTablePrefabs/UI/MainMenu"))as GameObject;
			else 
				mainMenu = Instantiate (Resources.Load ("PlayTablePrefabs/UI/MainMenu_test"))as GameObject;
			mainMenu.AddComponent<LoginManager> ();
			mainMenu.GetComponent<LoginManager> ().Hosting = hosting;
			gameObject.AddComponent<NetworkDiscovery> ();
			gameObject.AddComponent<TouchScript.InputSources.StandardInput> ();
			gameObject.AddComponent<TouchScript.Behaviors.TouchScriptInputModule> ();
			gameObject.AddComponent<WebSocketManager> ();
			gameObject.AddComponent<PrizmGameManager> ();
//			gameObject.AddComponent<AudioSource> ();
//			gameObject.AddComponent<SoundManager> ();

			GetComponent<PrizmGameManager> ().loadFromConfig = loadFromConfig;
		}

		void BootstrapNoNetworking() {
			gameObject.AddComponent<TouchScript.InputSources.StandardInput> ();
			gameObject.AddComponent<TouchScript.Behaviors.TouchScriptInputModule> ();
			gameObject.AddComponent<PrizmGameManager> ();
//			gameObject.AddComponent<AudioSource> ();
//			gameObject.AddComponent<SoundManager> ();
//
			GetComponent<PrizmGameManager> ().loadFromConfig = loadFromConfig;
		}

 

    }
}