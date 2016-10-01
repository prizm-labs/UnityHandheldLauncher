using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour {

	[SerializeField]
	public string currentSessionIPAddress = "";

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadGameScene () {

		Scene pairingScene = SceneManager.GetActiveScene ();

		GameObject.Find ("Canvas").SetActive (false);

		SceneManager.LoadScene ("GameView", LoadSceneMode.Additive);
//		SceneManager.LoadScene ("GameView", LoadSceneMode.Single);

		Debug.Log ("Scene count: " + SceneManager.sceneCount);
		Debug.Log ("Scene count in build settings: " + SceneManager.sceneCountInBuildSettings);

		Scene gameViewScene = SceneManager.GetSceneAt (1);
//		Scene gameViewScene = SceneManager.GetSceneByName ("GameView.unity");
//		Scene gameViewScene = SceneManager.GetSceneByPath("Assets/Scenes/GameView.unity");

		Debug.Log ("loading scene: "+gameViewScene.name);

//		SceneManager.LoadScene( gameViewScene.name , LoadSceneMode.Additive);
		SceneManager.MoveGameObjectToScene (GameObject.Find ("AppManager"), gameViewScene);
		SceneManager.MoveGameObjectToScene (GameObject.Find ("NetworkManager"), gameViewScene);
		SceneManager.SetActiveScene (gameViewScene);


		foreach (GameObject go in pairingScene.GetRootGameObjects() ) {
			Destroy (go);
		}
	}


}
