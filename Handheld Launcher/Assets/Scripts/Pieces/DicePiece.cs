using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using System.Linq;
using System;


namespace Prizm {
	public class DicePiece : Piece {
		public DicePrefabData myPrefabData;

		public override PieceCategories myCategory {
			get {
				return myPrefabData.myCategory;
			}
		}

		protected override void OnEnable() {
			base.OnEnable ();
		}

		protected override void OnDisable() {
			base.OnDisable ();
		}

		protected override void Start() {
			base.Start ();
			if (myPrefabData.hasDefaultFlick) {
				if (GetComponent<DiceMacros> () == null) {
					gameObject.AddComponent<DiceMacros> ();
				}
			}
		}


		public override void ApplyDataChange ()
		{

		}

		public override void SaveToPrefabData ()
		{

		}
	}

}