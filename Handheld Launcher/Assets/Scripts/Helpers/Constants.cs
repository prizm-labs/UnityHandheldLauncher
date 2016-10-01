using UnityEngine;
using System.Collections;

//global across all playtable games (changing this changes all games)
namespace Prizm{
	public enum ObjectCreatorButtons {Player = 0, Dice, Cards, Common, Custom, LoadBG, BGMusic}

	public enum TypeOfPiece {all = 0, gamePiece, playerPiece, cardPiece, dicePiece, currencyPiece, tutorialPiece}
	public enum TypeOfZone {all = 0, snapZone, oceanZone, dockZone, deckZone, storedZone, targetZone, throwZone, freeZone, clutterZone}
	public enum GameLayers {backdrop = -10, floor = 0, insetGUI = 5, tile = 10, lowerPiece = 2, upperPiece = 5, dockZone = 10, resourceBank = 30, tPiston = 40, lowerZone = 15, upperZone = 30, deckZone = 10, transport = 15, dice = 10, GUI = 95, camera = 100}

	public enum TypeOfSound {spawned = 0, pickedUp, moved, macro} 

	public enum Location {onBoard = 0, inDrawer}

	public enum Config {raiseHeight = 5, lowerHeight = -5}

	[System.Serializable]
	public class PieceDescriptor{
		public string pieceGuid;
		public string zoneGuid;

		public string prefabDataPath;
		public string spriteDataPath;

		public PieceDescriptor() {
			pieceGuid = System.Guid.NewGuid ().ToString();
		}

		public PieceDescriptor(string pG, string zG, string pDP, string sDP){
			pieceGuid = pG;
			zoneGuid = zG;

			prefabDataPath = pDP;
			spriteDataPath = sDP;
		}

		public PieceDescriptor(JSONObject data) {
			pieceGuid = data.GetField ("pieceGuid").str;
			zoneGuid = data.GetField ("zoneGuid").str;

			prefabDataPath = data.GetField ("prefabDataPath").str;
			spriteDataPath = data.GetField ("spriteDataPath").str;
		}

		public void GenerateNewGUID() {
			pieceGuid = System.Guid.NewGuid ().ToString();
		}
	}

	[System.Serializable]
	public class ZoneDescriptor{
		public string zoneGuid;
		public string playerOwnerGuid;
		public WormholeCategories gatewayZoneCategory;		//

		public ZoneDescriptor() {
			zoneGuid = System.Guid.NewGuid ().ToString();
		}

		public ZoneDescriptor(WormholeCategories gZc, string zG, string pG){
			gatewayZoneCategory = gZc;
			zoneGuid = zG;
			playerOwnerGuid = pG;
		}

		public ZoneDescriptor(JSONObject data) {
			gatewayZoneCategory = (WormholeCategories)data.GetField("gatewayZoneCategory").i;
			zoneGuid = data.GetField ("zoneGuid").str;
			playerOwnerGuid = data.GetField ("playerOwnerGuid").str;
		}

		public void GenerateNewGUID() {
			zoneGuid = System.Guid.NewGuid ().ToString();
		}
	}

	//descriptor used by SDK to define zones, etc.
	[System.Serializable]
	public class PlayerDescriptor{
		public PlayerStats gameSpecificStats = new PlayerStats ();
		public string playerName;
		public bool playerSeated;
		public string playerGuid;

		public PlayerDescriptor(){
			playerName = Lexic.NameGenerator.GetNextRandomName();
			playerSeated = false;
		}

		public PlayerDescriptor(string name, bool seat){
			playerName = name;
			playerSeated = seat;
		}

		public PlayerDescriptor(JSONObject data){
			playerName = data.GetField("playerName").str;
			playerSeated = data.GetField("playerSeated").b;
			//gameSpecificStats = JsonUtility.FromJson<PlayerStats>(data.GetField("gameSpecificStats").ToString());

			playerGuid = data.GetField ("playerGuid").str;
		}

		public void GenerateNewGuid(){
			playerGuid = System.Guid.NewGuid ().ToString ();
		}
	}
}

//unique to each game, but still guaranteed to work with SDK
public enum PieceCategories {all, cardZone1, cardZone2, card, coin}	//examples of sorting piece/snapzone interaction
public enum WormholeCategories {graveyard1, graveyard2, hand1, hand2, dock, fold, show, burn, deck}		//rename later?

[System.Serializable]
public class PlayerStats {
	
	public int coins { 				//property for docks
		get { 
			return _coins;
		} 
		set {
			_coins = value; 
		}
	}
	public int _coins;					//field for json serializer

	public PlayerStats() {
		//coins = 2;

	}
}