using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using System.Linq;
using System;


namespace Prizm {

	[System.Serializable]
	public abstract class Piece : MonoBehaviour {

		//default configuration for pieces
		//public PieceCategories myCategory = PieceCategories.all;
		public static Color defaultNewPieceColor = Color.white;

		public abstract PieceCategories myCategory { get; }

		//macros
		public delegate void GestureMacro();
		public delegate void CollisionMacro(Zone collidedZone);

		public GestureMacro TapMacro;
		public GestureMacro FlickMacro;
		public GestureMacro PressMacro;
		public GestureMacro ReleaseMacro;
		public GestureMacro StartMoveMacro;
		public GestureMacro StopMoveMacro;

		public CollisionMacro ZoneEnterMacro;
		public CollisionMacro ZoneExitMacro;


		//variables with getters/setters
		private TypeOfPiece _myType;

		public Zone _myHostZone;			//make private later, public for debugging editor
		private Zone _myPreviousZone;

		//if true, piece rubber bands back to its "home" zone if doesn't collide with acceptable zone
		public bool rubberBanding;


		//used to delay the transportation between multiscreens
		public ThrowZone readyToThrowZone = null;



		//getters/setters
		public TypeOfPiece myType {
			get { return _myType; }
			set 
			{
				//Debug.LogWarning ("setting type of " + gameObject.name + "piece to: " + value.ToString ());
				_myType = value; 
			}
		}
		public Zone myHostZone {				//automatically sets the first instance of previousZone
			get { return _myHostZone; }
			set {
				//Debug.Log ("setting snap zone for " + name + "!");
				if (myPreviousZone == null)
					myPreviousZone = value;
				_myHostZone = value;
			}
		}

		private Zone myPreviousZone {
			get { return _myPreviousZone; }
			set {
				_myPreviousZone = value;
			}
		}



			
		public PieceDescriptor myPieceDescriptor;
		Transaction myToken;

		private AudioSource myAudioSource;


		[SerializeField]
		public List<AudioClip> myAudioClips = new List<AudioClip>();




		protected virtual void Awake(){
			if ((GetComponent<TransformGesture>() != null) && (GetComponent<ApplyTransform> () == null)) {	//if it has a transformGesture, but is missing apply transform
				gameObject.AddComponent<ApplyTransform> ();
			}
			if (GetComponent<AudioSource> () == null) {
				gameObject.AddComponent<AudioSource> ();
			}

			myAudioSource = GetComponent<AudioSource> ();
			myPieceDescriptor.GenerateNewGUID();
		}

		protected virtual void Start() {
			StartCoroutine (LoadAudio ());
		}

		public abstract void ApplyDataChange ();

		public abstract void SaveToPrefabData ();


		protected virtual void OnEnable() {
			if (GetComponent<TapGesture>() != null)
				GetComponent<TapGesture>().Tapped += OnTapHandler;
			if (GetComponent<FlickGesture>() != null)
				GetComponent<FlickGesture>().Flicked += OnFlickHandler;
			if (GetComponent<PressGesture> () != null) 
				GetComponent<PressGesture> ().Pressed += OnPressHandler;
			if (GetComponent<ReleaseGesture>() != null)
				GetComponent<ReleaseGesture>().Released += OnReleaseHandler;
			if (GetComponent<TransformGesture> () != null) {
				GetComponent<TransformGesture> ().TransformStarted += OnTransformStartHandler;
				GetComponent<TransformGesture> ().TransformCompleted += OnTransformCompletedHandler;
			}
		}

		void OnReleaseHandler (object sender, EventArgs e)
		{
			if (ReleaseMacro != null)
				ReleaseMacro ();
		}

		protected virtual void OnDisable() {
			if (GetComponent<TapGesture>() != null)
				GetComponent<TapGesture>().Tapped -= OnTapHandler;
			if (GetComponent<FlickGesture>() != null)
				GetComponent<FlickGesture>().Flicked -= OnFlickHandler;
			if (GetComponent<PressGesture>() != null)
				GetComponent<PressGesture>().Pressed -= OnPressHandler;
			if (GetComponent<ReleaseGesture>() != null)
				GetComponent<ReleaseGesture>().Released -= OnReleaseHandler;
			if (GetComponent<TransformGesture> () != null) {
				GetComponent<TransformGesture> ().TransformStarted -= OnTransformStartHandler;
				GetComponent<TransformGesture> ().TransformCompleted -= OnTransformCompletedHandler;
			}
		}

		//all pieces log their transactions
		void OnTransformCompletedHandler (object sender, System.EventArgs e)
		{
			if (StopMoveMacro != null)
				StopMoveMacro ();

			if (myHostZone != null)
				LogTransaction ();
		}

		void OnTransformStartHandler (object sender, System.EventArgs e)
		{
			transform.SetParent (null);			//set parent to null when move otherwise iTween will act up if object is childed to a parent
			if (StartMoveMacro != null)
				StartMoveMacro ();
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
			
			if (TapMacro != null)
				TapMacro ();
		}


		protected virtual void OnTriggerEnter(Collider other) {
			if (other.GetComponent<Zone> () != null) {
				if (ZoneEnterMacro != null) {
					ZoneEnterMacro (other.GetComponent<Zone> ());
				}
			}
		}

		protected virtual void OnTriggerExit(Collider other) {
			if (other.GetComponent<Zone> () != null) {
				if (ZoneExitMacro != null) {
					ZoneExitMacro (other.GetComponent<Zone> ());
				}
			}
		}


		void LogTransaction() {
			myToken = new Transaction ();
			myToken.piece = this;
			myToken.before = myPreviousZone;
			myToken.after = myHostZone;

			myPreviousZone = myHostZone;

			//PrizmGameManager.instance.AddToRoster (myToken);
		}


		private IEnumerator LoadAudio() {
			//Debug.Log ("loading audio..");
			myAudioClips = new List<AudioClip>(Resources.LoadAll ("Sound/" + myType.ToString (), typeof(AudioClip)).Cast<AudioClip>().ToArray());
			if (myAudioClips.Count == 0) {
				Debug.LogError ("no sounds for: " + name + ", of type: " + myType.ToString ());
			}
			//Debug.Log ("we have sounds! clips are: " + myAudioClips.Count.ToString () + ", for " + name);
			yield return null;
		}

		public void PlayASound() {
			//Debug.Log ("trying to play sound!");
			if (myAudioClips.Count == 0) {
				Debug.LogError ("we have no audioClips for: " + name);
				return;
			}
			//myAudioSource.volume = SoundManager.instance.globalSFXVolume;
			myAudioSource.clip = myAudioClips [UnityEngine.Random.Range (0, myAudioClips.Count - 1)];
			myAudioSource.Play ();
		}

		//TODO: loading pieces
		public void ReloadSprite() {
			Debug.Log ("loading sprite from: " + myPieceDescriptor.spriteDataPath);
		}

		//TODO: define the loading mechanism
		public void LoadData() {
			Debug.Log ("loading data");
		}


		//TODO: 
		//reparents transform
		//removes from hostedpieces
		//sets hostzone
		//adds to list of new hostzone
		public void TransferOwnership() {
			
		}
	}

}