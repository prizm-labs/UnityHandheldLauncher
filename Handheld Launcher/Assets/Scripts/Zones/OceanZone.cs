using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

namespace Prizm {
	public class OceanZone : Zone {

		[System.Serializable]
		public class OceanZoneSettings
		{
			public GameObject snapZonePrefab;
			public float width;
			public float height;
			public int xNumZones;
			public int zNumZones;
		}

		[System.NonSerialized]
		public List<SnapZone> mySnapZones = new List<SnapZone>();

		public bool generateZones;	//set in the editor

		public OceanZoneSettings myGenerationSettings;

		protected override void Awake() {
			base.Awake ();
			myType = TypeOfZone.oceanZone;

			if (generateZones) {
				GenerateZones ();
			} else {
				foreach (Transform child in transform) {
					mySnapZones.Add (child.GetComponent<SnapZone> ());
				}
			}
		}


		private void GenerateZones() {
			float deltaX = myGenerationSettings.width / myGenerationSettings.xNumZones;
			float deltaZ = myGenerationSettings.height / myGenerationSettings.zNumZones;

			//starting position
			Vector3 originalPosition = new Vector3 (-myGenerationSettings.width / 2.0f, 0, myGenerationSettings.height / 2.0f);
			Vector3 snapPosition = originalPosition;

			for (int row = 0; row < myGenerationSettings.zNumZones; row++) {
				for (int col = 0; col < myGenerationSettings.xNumZones; col++) {
					snapPosition.x = snapPosition.x + deltaX;
					GameObject tempSnapZone = Instantiate (myGenerationSettings.snapZonePrefab, transform.position, Quaternion.identity) as GameObject;
					tempSnapZone.transform.parent = transform;
					tempSnapZone.transform.localPosition = snapPosition;

					mySnapZones.Add (tempSnapZone.GetComponent<SnapZone> ());
				}
				snapPosition.x = originalPosition.x;

				snapPosition.z = snapPosition.z - deltaZ;
			}
		}

		public override void ApplyDataChange ()
		{
			throw new System.NotImplementedException ();
		}

		public override void SaveToPrefabData ()
		{
			throw new System.NotImplementedException ();
		}
	}


}