using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ApplyTransform : MonoBehaviour {
	private GameObject m_DraggingIcon;
	//private RectTransform m_DraggingPlane;

	TransformGesture myTransformGesture;
	PressGesture myPressGesture;
	//ReleaseGesture myReleaseGesture;

	private Vector3 maxScaling = new Vector3();
	private Vector3 minScaling = new Vector3();

	public static float maxScale = 10.0f;
	public static float minScale = 0.10f;

	public int activeTouches = 0;
	void Awake() { 

		myTransformGesture = GetComponent<TransformGesture> ();
		myPressGesture = GetComponent<PressGesture> ();
		//myReleaseGesture = GetComponent<ReleaseGesture> ();
		//Debug.Log ("my press gesture: " + myPressGesture.ToString ());


	}

	void OnEnable() {
		if (myTransformGesture != null)
			myTransformGesture.Transformed += MyTransformGesture_Transformed;
		//myPressGesture.Pressed += MyPressGesture_Pressed;
		//myReleaseGesture.Released += MyReleaseGesture_Released;
		maxScaling = transform.localScale * maxScale;
		minScaling = transform.localScale * minScale;
	}


	void OnDisable() {
		if (myTransformGesture != null)
			myTransformGesture.Transformed -= MyTransformGesture_Transformed;
		//myPressGesture.Pressed -= MyPressGesture_Pressed;
		//myReleaseGesture.Released -= MyReleaseGesture_Released;
	}

	void MyTransformGesture_Transformed (object sender, System.EventArgs e)	{

		myTransformGesture.ApplyTransform (this.transform);

		if (this.transform.localScale.sqrMagnitude > maxScaling.sqrMagnitude) {
			Debug.Log ("we are too big");
			this.transform.localScale = maxScaling;
		}
		if (this.transform.localScale.sqrMagnitude < minScaling.sqrMagnitude) {
			Debug.Log ("we are too small, our min magnitude is: " + minScaling.sqrMagnitude.ToString() + ", when we are this big: " + this.transform.localScale.sqrMagnitude.ToString());
			this.transform.localScale = minScaling;
		}

		//Debug.LogError (myTransformGesture.ScreenPosition);
		//Debug.LogError(Camera.main.ScreenToWorldPoint(new Vector3(myTransformGesture.ScreenPosition.x, myTransformGesture.ScreenPosition.y, Camera.main.farClipPlane)));
		if (m_DraggingIcon != null)
			SetDraggedPosition(myTransformGesture.ScreenPosition);
	}

	void MyPressGesture_Pressed(object sender, System.EventArgs e){
		Debug.LogError ("PRessed gesture");
		Canvas canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcon = new GameObject("icon");

		m_DraggingIcon.transform.SetParent (canvas.transform, false);
		m_DraggingIcon.transform.SetAsLastSibling();

		var image = m_DraggingIcon.AddComponent<RawImage>();
		// The icon will be under the cursor.
		// We want it to be ignored by the event system.
		CanvasGroup group = m_DraggingIcon.AddComponent<CanvasGroup>();
		group.blocksRaycasts = false;

		image.texture = GetComponent<RawImage>().texture;
		image.SetNativeSize();

		//m_DraggingPlane = canvas.transform as RectTransform;

		SetDraggedPosition(myPressGesture.ActiveTouches[activeTouches].Position);	
	}

	void MyReleaseGesture_Released(object sender, System.EventArgs e){
		Debug.LogError ("Released Gesture");
		if (m_DraggingIcon != null)
			Destroy(m_DraggingIcon);
	}



	private void SetDraggedPosition(Vector2 point)
	{
		var rt = m_DraggingIcon.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		globalMousePos = point;
		//globalMousePos = Camera.main.ScreenToWorldPoint(new Vector3(myTransformGesture.ScreenPosition.x, myTransformGesture.ScreenPosition.y, Camera.main.farClipPlane));
		rt.position = globalMousePos;
	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;

		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}





}
