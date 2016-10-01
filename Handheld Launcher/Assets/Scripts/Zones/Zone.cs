using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

namespace Prizm {
	public abstract class Zone : MonoBehaviour {

		public delegate void GestureMacro();
		public delegate void CollisionMacro(Piece collidedPiece);

		public GestureMacro TapMacro;
		public GestureMacro FlickMacro;
		public GestureMacro PressMacro;
		public GestureMacro StartMoveMacro;
		public GestureMacro StopMoveMacro;
		public GestureMacro LongPressMacro;

		public CollisionMacro PieceEnterMacro;
		public CollisionMacro PieceExitMacro;

		//only pieces of certain types are allowed to be thrown on specific throw zones
		public List<PieceCategories> acceptedCategories;



		private TypeOfZone _myType;
		public TypeOfZone myType {
			get { return _myType; }
			set 
			{
				//Debug.LogWarning ("setting type of " + gameObject.name + "zone to: " + value.ToString ());
				_myType = value; 
			}
		}

		//[System.NonSerialized]
		public List<Piece> hostedPieces = new List<Piece>();

		public ZoneDescriptor myZoneDescriptor;


		protected virtual void Awake() {
			if (acceptedCategories.Count == 0) {
				Debug.LogWarning (name + " has no accepted categories!");
			}
		}

		//reserved for future use
		//assume all inherited classes implement base.Start()
		protected virtual void Start() {

		}

		//used to update prefab data for all zones
		public abstract void ApplyDataChange ();
		public abstract void SaveToPrefabData ();


		protected virtual void OnEnable() {
			if (GetComponent<TapGesture>() != null)
				GetComponent<TapGesture>().Tapped += OnTapHandler;
			if (GetComponent<FlickGesture>() != null)
				GetComponent<FlickGesture>().Flicked += OnFlickHandler;
			if (GetComponent<PressGesture>() != null)
				GetComponent<PressGesture>().Pressed += OnPressHandler;
			if (GetComponent<LongPressGesture>() != null)
				GetComponent<LongPressGesture>().LongPressed += OnLongPressHandler;
			if (GetComponent<TransformGesture> () != null) {
				GetComponent<TransformGesture> ().TransformStarted += OnTransformStartHandler;
				GetComponent<TransformGesture> ().TransformCompleted += OnTransformCompletedHandler;
			}
		}

		protected virtual void OnDisable() {
			if (GetComponent<TapGesture>() != null)
				GetComponent<TapGesture>().Tapped -= OnTapHandler;
			if (GetComponent<FlickGesture>() != null)
				GetComponent<FlickGesture>().Flicked -= OnFlickHandler;
			if (GetComponent<PressGesture>() != null)
				GetComponent<PressGesture>().Pressed -= OnPressHandler;
			if (GetComponent<LongPressGesture>() != null)
				GetComponent<LongPressGesture>().LongPressed -= OnLongPressHandler;
			if (GetComponent<TransformGesture> () != null) {
				GetComponent<TransformGesture> ().TransformStarted -= OnTransformStartHandler;
				GetComponent<TransformGesture> ().TransformCompleted -= OnTransformCompletedHandler;
			}
		}

		void OnTransformCompletedHandler (object sender, System.EventArgs e)
		{
			if (StopMoveMacro != null)
				StopMoveMacro ();
		}

		void OnTransformStartHandler (object sender, System.EventArgs e)
		{
			if (StartMoveMacro != null)
				StartMoveMacro ();
		}

		void OnLongPressHandler (object sender, System.EventArgs e)
		{
			if (LongPressMacro != null)
				LongPressMacro ();
		}

		void OnPressHandler (object sender, System.EventArgs e)
		{
			if (PressMacro != null)
				PressMacro ();
		}

		void OnFlickHandler (object sender, System.EventArgs e)
		{
			if (FlickMacro != null)
				FlickMacro ();
		}

		void OnTapHandler (object sender, System.EventArgs e)
		{
			Debug.Log ("ontaphandler for zone");
			if (TapMacro != null) 
				TapMacro ();
		}


		protected virtual void OnTriggerEnter(Collider other) {
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();

				if (acceptedCategories.Contains (otherPiece.myCategory)) {

					//hostedPieces.Add (otherPiece);
					//other.transform.SetParent (this.transform);


					if (PieceEnterMacro != null) {
						PieceEnterMacro (other.GetComponent<Piece> ());
					}
				}
			}
		}

		protected virtual void OnTriggerExit(Collider other) {
			if (other.GetComponent<Piece> () != null) {
				Piece otherPiece = other.GetComponent<Piece> ();

				if (acceptedCategories.Contains (otherPiece.myCategory)) {
					hostedPieces.Remove (otherPiece);
					//other.transform.SetParent (this.transform);


					if (PieceExitMacro != null) {
						PieceExitMacro (other.GetComponent<Piece> ());
					}
				}
			}
		}





		//default behaviour for moving pieces
		//helper function for zones' manipulation of piece position
		//by default, we will use the PrizmGameManager's settings for cardMovementTime, otherwise, use provided parameter
		public static void MovePieceToZone(Piece pieceToMove, Zone targetZone, bool rotateAngles = true, GameLayers pieceLayer = GameLayers.lowerPiece, float travelTime = -1.0f) {
			float movementTime = travelTime;
			if (travelTime < 0)
				movementTime = PrizmGameManager.instance.myGameConfig.cardMovementTime;
			Vector3 targetPosition = targetZone.transform.position;
			targetPosition = new Vector3 (targetPosition.x, (float)pieceLayer, targetPosition.z);
			//Debug.Log ("piece to move: " + pieceToMove.ToString ());
			//Debug.Log ("target position: " + targetPosition.ToString ());
			//Debug.Log ("game config: " + PrizmGameManager.instance.myGameConfig.cardMovementTime.ToString ());
			try {
				iTween.MoveTo (pieceToMove.gameObject, targetPosition, movementTime);
				if (rotateAngles)
					iTween.RotateTo(pieceToMove.gameObject, targetZone.transform.localEulerAngles, movementTime);
			} catch (System.NullReferenceException ex) {
				Debug.LogError (ex);
			}
		}

	}
}