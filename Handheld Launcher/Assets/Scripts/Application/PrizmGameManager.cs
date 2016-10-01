using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Prizm {

	[System.Serializable]
	public class Transaction {
		//public KeyValuePair<Piece, Zone> before = new KeyValuePair<Piece, Zone>();
		//public KeyValuePair<Piece, Zone> after;

		public Piece piece;
		public Zone before;
		public Zone after;
	}

	//PrizmGameManager responsible for:
	//loading, saving, tracking game states
	//loading config files
	//loading all pieces from config file if necessary
	public class PrizmGameManager : MonoBehaviour {

		[System.Serializable]
		public class GameConfig {
			public float cardMovementTime = 0.4f;	//how long it takes for cards to move from one location to the next (used by Zones)
		}

		public bool loadFromConfig;
		public GameConfig myGameConfig = new GameConfig();

		public static PrizmGameManager instance;

		//private List<Transaction> masterTransactionLog = new List<Transaction>();

		private int rosterPointer = -1;	//points to the proper current zero-based index (starts at -1 and increments to 0 when count is increased)


		void Awake() {
			if (instance == null) {
				instance = this;
			} else {
				Debug.LogWarning ("uh oh, destorying PrizmGameManager");
				Destroy (instance);
				instance = this;
			}

		}

		void Start() {
			
			if (loadFromConfig) {
				StartCoroutine (LoadObjectsFromConfig ());
			}
		}

		//parses objects and instantiates them
		IEnumerator LoadObjectsFromConfig() {
			Debug.Log ("loading from config file");

			string filePath = Application.streamingAssetsPath + "/gameConfig.json";
			StreamReader sr = new StreamReader (filePath);
			yield return sr;

			JSONObject root = new JSONObject (sr.ReadToEnd ());
			JSONObject objects = root.GetField ("objects");

			// InstantiateJsonObjects (objects);
		}

//		void InstantiateJsonObjects(JSONObject objects, GameObject parent = null) {
//			Debug.Log ("instantiate json objects: " + objects.ToString ());
//			foreach (JSONObject obj in objects.list) {
//				ConfigData<PrefabData> initialData = JsonUtility.FromJson<ConfigData<PrefabData>> (obj.Print ());	//have to check json with initial type to determine "type" field before entering switch
//
//				switch (initialData.type) {
//				case "snapZone":
//					{
//						ConfigData<SnapPrefabData> confData = JsonUtility.FromJson<ConfigData<SnapPrefabData>> (obj.Print ());
//						GameObject prefab = Resources.Load (confData.prefabPath, typeof(GameObject)) as GameObject;
//						Vector3 spawnPosition = new Vector3 (confData.position.x, confData.position.y, confData.position.z);
//						GameObject newObject = Instantiate (prefab, spawnPosition, Quaternion.LookRotation (Vector3.down)) as GameObject;
//						newObject.name = confData.name;
//						SnapZone newZone = newObject.GetComponent<SnapZone> ();
//						newZone.myPrefabData = confData.prefabData;
//						newZone.ApplyDataChange ();
//
//						break;
//					}
//				case "deckZone":
//					{
//						ConfigData<DeckPrefabData> confData = JsonUtility.FromJson<ConfigData<DeckPrefabData>> (obj.Print ());
//						GameObject prefab = Resources.Load (confData.prefabPath, typeof(GameObject)) as GameObject;
//						Vector3 spawnPosition = new Vector3 (confData.position.x, confData.position.y, confData.position.z);
//						GameObject newObject = Instantiate (prefab, spawnPosition, Quaternion.LookRotation (Vector3.down)) as GameObject;
//						newObject.name = confData.name;
//						DeckZone newZone = newObject.GetComponent<DeckZone> ();
//						newZone.myPrefabData = confData.prefabData;
//						newZone.ApplyDataChange ();
//						Debug.Log ("prefab's prefab data's generate by script: " + newZone.GetComponent<DeckZone> ().myPrefabData.generateByScript.ToString ());
//
//
//						JSONObject wholeObj = new JSONObject (obj.Print ());
//						JSONObject containsList = wholeObj ["contains"];
//						if (containsList != null) {
//							Debug.Log ("not null contains list!");
//							InstantiateJsonObjects (containsList, newObject);
//						}
//						break;
//					}
//				case "cardPiece":
//					{
//						ConfigData<CardPrefabData> confData = JsonUtility.FromJson<ConfigData<CardPrefabData>> (obj.Print ());
//						GameObject prefab = Resources.Load (confData.prefabPath, typeof(GameObject)) as GameObject;
//						Vector3 spawnPosition = new Vector3 (confData.position.x, confData.position.y, confData.position.z);
//						GameObject newObject = Instantiate (prefab, spawnPosition, Quaternion.LookRotation (Vector3.down)) as GameObject;
//						if (parent != null) {
//							newObject.transform.SetParent (parent.transform);
//						}
//						newObject.name = confData.name;
//						CardPiece newPiece = newObject.GetComponent<CardPiece> ();
//						newPiece.myPrefabData = confData.prefabData;
//						newPiece.ApplyDataChange ();
//
//						if (newPiece.myPrefabData.spritePath != null) {
//							if (newPiece.gameObject.GetComponent<SpriteRenderer> () == null)
//								newPiece.gameObject.AddComponent<SpriteRenderer> ();
//							Sprite loadedSprite = Resources.Load (newPiece.myPrefabData.spritePath, typeof(Sprite)) as Sprite;
//
//							newPiece.gameObject.GetComponent<SpriteRenderer> ().sprite = loadedSprite;
//						}
//						break;
//					}
//				case "gamePiece":
//					{
//						ConfigData<GamePiecePrefabData> confData = JsonUtility.FromJson<ConfigData<GamePiecePrefabData>> (obj.Print ());
//						GameObject prefab = Resources.Load (confData.prefabPath, typeof(GameObject)) as GameObject;
//						Vector3 spawnPosition = new Vector3 (confData.position.x, confData.position.y, confData.position.z);
//						GameObject newObject = Instantiate (prefab, spawnPosition, Quaternion.LookRotation (Vector3.down)) as GameObject;
//						newObject.name = confData.name;
//						GamePiece newPiece = newObject.GetComponent<GamePiece> ();
//						newPiece.myPrefabData = confData.prefabData;
//						newPiece.ApplyDataChange ();
//
//						if (newPiece.myPrefabData.spritePath != null) {
//							if (newPiece.gameObject.GetComponent<SpriteRenderer> () == null)
//								newPiece.gameObject.AddComponent<SpriteRenderer> ();
//							Sprite loadedSprite = Resources.Load (newPiece.myPrefabData.spritePath, typeof(Sprite)) as Sprite;
//
//							newPiece.gameObject.GetComponent<SpriteRenderer> ().sprite = loadedSprite;
//						}
//						break;
//					}
//
//				default:
//					{
//						Debug.LogError ("check your json! something doesn't have a proper type! (or it isn't implemented to be loaded yet)" + initialData.type);
//						break;
//					}
//
//				}
//			}
//
//		}
//
//
//
//		void Update() {
//			if (Input.GetKeyDown (KeyCode.S)) {
//
//				SaveGame ("SaveGame1.xml");
//			}
//
//			if (Input.GetKeyDown (KeyCode.L)) {
//
//				StartCoroutine (LoadObjectsFromConfig());
//			}
//
//
//		}
//
//		public void AddToRoster(Transaction token) {
//			Debug.Log ("logged!");
//
//
//			//set nuke all transactions after current pointer location
//			if (rosterPointer < masterTransactionLog.Count - 1)
//				masterTransactionLog.RemoveRange(rosterPointer + 1, masterTransactionLog.Count - rosterPointer - 1);
//
//
//			//add transactiont o log, increment our reference pointer
//			masterTransactionLog.Add (token);
//			rosterPointer++;
//		}
//
//		//undo the last action
//		//does not delete entries in masterTransactionLog
//		//decrements rosterPointer
//		public void Revert() {
//			Debug.Log ("reverting");
//			if (rosterPointer < 0) {
//				Debug.LogError ("at the beginning of the list!");
//				return;
//			}
//
//			Piece tempPiece = masterTransactionLog [rosterPointer].piece;
//
//			tempPiece.myHostZone = masterTransactionLog [rosterPointer].before;
//			Zone.MovePieceToZone (tempPiece, tempPiece.myHostZone);
//
//			rosterPointer--;
//		}
//
//		public void Traverse() {
//			Debug.Log ("Traversing");
//			if (rosterPointer >= masterTransactionLog.Count - 1) {
//				Debug.LogError ("at end of transaction log");
//				return;
//			}
//
//			try {
//				Piece tempPiece = masterTransactionLog [rosterPointer].piece;
//
//				tempPiece.myHostZone = masterTransactionLog [rosterPointer].after;
//				Zone.MovePieceToZone (tempPiece, tempPiece.myHostZone);
//
//				rosterPointer++;
//			} catch (System.ArgumentOutOfRangeException ex) {
//				Debug.LogError (ex);
//			}
//
//
//		}
//
//		public void SaveGame(string saveName) {
//			Debug.Log ("saving game...");
//			List<PieceDescriptor> savePieceObjects = new List<PieceDescriptor> ();
//			foreach (Transaction token in masterTransactionLog) {
//				savePieceObjects.Add (token.piece.myPieceDescriptor);
//			}
//
//
//			SerializeObject<List<PieceDescriptor>> (savePieceObjects, saveName);
//
//		}
//
//
//		/// <summary>
//		/// Serializes an object.
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="serializableObject"></param>
//		/// <param name="fileName"></param>
//		public void SerializeObject<T>(T serializableObject, string fileName)
//		{
//			string filePath = Application.streamingAssetsPath + "/" + fileName;
//			Debug.Log("serializing to path: " + filePath);
//			if (serializableObject == null) { return; }
//
//			try
//			{
//				XmlDocument xmlDocument = new XmlDocument();
//				XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
//				using (MemoryStream stream = new MemoryStream())
//				{
//					serializer.Serialize(stream, serializableObject);
//					stream.Position = 0;
//					xmlDocument.Load(stream);
//					xmlDocument.Save(filePath);
//					stream.Close();
//				}
//			}
//			catch (System.Exception ex)
//			{
//				Debug.LogError (ex);
//				//Log exception here
//			}
//		}
//
//
//		/// <summary>
//		/// Deserializes an xml file into an object list
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="fileName"></param>
//		/// <returns></returns>
//		public T DeSerializeObject<T>(string fileName)
//		{
//			if (string.IsNullOrEmpty(fileName)) { return default(T); }
//
//			T objectOut = default(T);
//
//			try
//			{
//				XmlDocument xmlDocument = new XmlDocument();
//				xmlDocument.Load(fileName);
//				string xmlString = xmlDocument.OuterXml;
//
//				using (StringReader read = new StringReader(xmlString))
//				{
//					System.Type outType = typeof(T);
//
//					XmlSerializer serializer = new XmlSerializer(outType);
//					using (XmlReader reader = new XmlTextReader(read))
//					{
//						objectOut = (T)serializer.Deserialize(reader);
//						reader.Close();
//					}
//
//					read.Close();
//				}
//			}
//			catch (System.Exception ex)
//			{
//				Debug.LogError (ex);
//				//Log exception here
//			}
//
//			return objectOut;
//		}
//
//
//
	}
}