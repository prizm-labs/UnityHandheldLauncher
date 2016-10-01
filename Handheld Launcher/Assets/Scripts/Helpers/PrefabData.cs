using UnityEngine;
using System.Collections;
using Prizm;

//parent wrapper class for all other developer-defined data
[System.Serializable]
public class PrefabData {
	public string spritePath;
	public PieceCategories myCategory;
}

[System.Serializable]
public class DeckPrefabData : PrefabData {
	public string targetOceanZoneName;
	public bool takesOwnership;
	public bool generateByScript;
	public bool completeDeal;
}

[System.Serializable]
public class SnapPrefabData : PrefabData {
	public bool letGoOnCollide;

}

[System.Serializable]
public class GamePiecePrefabData : PrefabData {
	
}

[System.Serializable]
public class DicePrefabData : PrefabData {
	public bool hasDefaultFlick;
}

[System.Serializable]
public class CardPrefabData : PrefabData {

}