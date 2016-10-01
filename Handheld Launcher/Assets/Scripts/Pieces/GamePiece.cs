using UnityEngine;
using System.Collections;
using TouchScript.Gestures;


namespace Prizm {
	public class GamePiece : Piece {

		private Vector3 originalScale;

		//[System.NonSerialized]
		public GamePiecePrefabData myPrefabData;



		private Vector3 originalPosition;
		private Vector3 originalRotation;

		private Vector3 travelVector;
		private Vector3 myPosition;

		private IEnumerator wobbleDuringMovementCoroutine;
		private IEnumerator raiseCoroutine;

		//TODO: put all configuration variables in one location
		private static float raiseBy = 10.0f;		//amount to reduce the travel vector by.  smaller number means more movement
		private static float raiseTime = 0.5f;
		float noiseScaler = 50.0f;

		float shakeTime = 1.0f;




		TransformGesture myTransformGesture;

		public override PieceCategories myCategory {
			get {
				return myPrefabData.myCategory;
			}
		}

		protected override void Awake() {
			base.Awake ();
			myType = TypeOfPiece.gamePiece;
			myTransformGesture = GetComponent<TransformGesture> ();
		}

		protected override void Start() {
			base.Start ();
			originalScale = transform.localScale;
		}

		protected override void OnEnable() {
			base.OnEnable ();

			//by default, all game pieces play a sound and are enlarged
			StartMoveMacro = StartMovingPiece;

			//by default, all game pieces snap to their hosted zone on release
			StopMoveMacro = StopMovingPiece;

		}

		protected override void OnDisable() {
			base.OnDisable ();
		}



		public override void ApplyDataChange ()
		{
			if (myPrefabData != null) {
				if (myPrefabData.spritePath != null && GetComponent<SpriteRenderer>() != null) {
					GetComponent<SpriteRenderer> ().sprite = Resources.Load (myPrefabData.spritePath, typeof(Sprite)) as Sprite;
				}
			}
		}

		public override void SaveToPrefabData ()
		{
			if (myPrefabData == null) {
				myPrefabData = new GamePiecePrefabData ();
			}

			if (GetComponent<SpriteRenderer> () != null) {
				myPrefabData.spritePath = "Sprites/" + GetComponent<SpriteRenderer> ().sprite.name;
			}
		}
			







		//restores last position
		//snaps back if there was a zone
		//should play a sound
		//stops wiggling
		//drops piece
		protected void StopMovingPiece() {
			PlayASound ();

			//iTween.StopByName ("raise" + myPieceDescriptor.pieceGuid);
			if (wobbleDuringMovementCoroutine != null)
				StopCoroutine (wobbleDuringMovementCoroutine);
			LowerPiece ();



			//Debug.Log ("setting card to original rotation, " + originalRotation.ToString());
			//transform.localEulerAngles = originalRotation;

			Hashtable rotateConfig = new Hashtable ();
			rotateConfig.Add ("time", 0.2f);
			rotateConfig.Add ("rotation", originalRotation);
			iTween.RotateTo (gameObject, rotateConfig);

			((Behaviour) GetComponent ("Halo")).enabled = false;

			if (rubberBanding && myHostZone != null) {
				Hashtable ht = new Hashtable ();
				ht.Add ("time", 0.2f);
				ht.Add ("position", myHostZone.transform);
				iTween.MoveTo(gameObject, ht);
			} else {
				//Debug.LogWarning("not rubber banding gamepiece!: " + name);
			}
		}

		//saves position
		//raises the piece
		//begins wiggling piece
		protected void StartMovingPiece() {
			PlayASound ();

			originalPosition = transform.position;
			originalRotation = transform.localEulerAngles;

			((Behaviour) GetComponent ("Halo")).enabled = true;

			RaisePiece ();
			WiggleOnce ();

			wobbleDuringMovementCoroutine = wobbleDuringMovement ();
			StartCoroutine (wobbleDuringMovementCoroutine);
		}

		private void RaisePiece() {
			Hashtable ht = new Hashtable ();
			ht.Add ("name", "raise" + myPieceDescriptor.pieceGuid);
			ht.Add ("time", raiseTime);


			//ht.Add ("amount", transform.up * raiseBy);
			Vector3 tarPosition = Camera.main.transform.position;
			myPosition = transform.position;
			travelVector = tarPosition - myPosition;

			if (IsUpsideDown ()) {
				ht.Add ("amount", travelVector / raiseBy);
			} else {
				ht.Add ("amount", travelVector / -raiseBy);
			}



			iTween.MoveBy(gameObject, ht);

		}

		private IEnumerator wobbleDuringMovement() {

			float maxAngleZ = 60.0f;
			float maxAngleX = 60.0f;

			float wobbleUpdateTime = 0.1f;
			float wiggleResolveTime = 1.0f;
			Vector3 rotationVector = new Vector3 ();
			Vector3 previousRotationVector = new Vector3 ();

			float maxDeltaPos = 1.0f;


			while (true) {

				//Debug.Log("we moved by this much: " + myTransformGesture.DeltaPosition.ToString());

				float rotationRatioX = Mathf.Clamp (myTransformGesture.DeltaPosition.x, -maxDeltaPos, maxDeltaPos) / maxDeltaPos;
				float rotationRatioZ = Mathf.Clamp (myTransformGesture.DeltaPosition.z, -maxDeltaPos, maxDeltaPos) / maxDeltaPos;

				//have to swap rotation axes
				rotationVector.z = rotationRatioX * -maxAngleX + originalRotation.z;
				rotationVector.x = rotationRatioZ * maxAngleZ + originalRotation.x;

				Hashtable ht = new Hashtable ();
				ht.Add ("name", "wiggle" + myPieceDescriptor.pieceGuid);
				ht.Add ("time", wiggleResolveTime);

				//if we have a different delta position then last time, re-orient to that one
				if (!ApproximatelyEqualVector (rotationVector, previousRotationVector)) {
					ht.Add ("rotation", rotationVector);
					iTween.StopByName ("wiggle" + myPieceDescriptor.pieceGuid);
					iTween.RotateTo (gameObject, ht);
					previousRotationVector = rotationVector;

					//or if we are holding the card still, we have to move it back to default flat position
				} else if (ApproximatelyEqualVector (myTransformGesture.DeltaPosition, Vector3.zero)) {
					//Debug.Log ("moving to default position");
					ht.Add ("rotation", originalRotation);

					iTween.StopByName ("wiggle" + myPieceDescriptor.pieceGuid);
					iTween.RotateTo (gameObject, ht);
					previousRotationVector = Vector3.zero;
				}

				yield return new WaitForSeconds (wobbleUpdateTime);		//must have a yield statement in while true or else infinite loop
			}
		}

		private bool ApproximatelyEqualVector(Vector3 a, Vector3 b) {
			return Vector3.SqrMagnitude (b - a) < 0.1f;
		}

		//calculates random amount to shake the card by initially
		private void WiggleOnce() {
			Vector3 noiseVector = new Vector3 ();

			noiseVector.x = Random.Range (-noiseScaler, noiseScaler);
			noiseVector.y = Random.Range (-noiseScaler, noiseScaler);
			noiseVector.z = Random.Range (-noiseScaler, noiseScaler);
			iTween.PunchRotation (gameObject, noiseVector, shakeTime);

		}

		private bool IsUpsideDown() {
			if (Mathf.Abs(Mathf.Abs (transform.localEulerAngles.z) - 180) < 30) {
				return false;
			}

			return true;
		}

		protected void LowerPiece() {
			Hashtable ht = new Hashtable ();
			ht.Add ("name", "raise" + myPieceDescriptor.pieceGuid);
			ht.Add ("time", raiseTime);

			Vector3 currentPos = transform.position;
			float deltaY = (float) GameLayers.lowerPiece - currentPos.y;

			Vector3 amtVector = travelVector / raiseBy;
			amtVector.y = deltaY;

			if (IsUpsideDown ()) {
				ht.Add ("amount", amtVector);
			} else {
				ht.Add ("amount", -amtVector);
			}


			iTween.MoveBy(gameObject, ht);

		}

	}
}