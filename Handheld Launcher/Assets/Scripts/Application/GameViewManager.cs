﻿using UnityEngine;
using System.Collections;


// GameView is the main view in the handheld client 
// managing events and transitions between the following views:
// - PlayerInfoPanel
// - GameViewPanel
// - HeaderPanel
// - MenuPanel
// - FooterPanel

public class GameViewManager : MonoBehaviour {

	public static GameViewManager instance;

	// views are fullscreen UIPanels
	public GameObject playerInfoView;
	public GameObject gameWorldView;

	// fixed UI elements
	public GameObject headerPanel;
	public GameObject footerPanel;
	public GameObject sidebarPanel;

	[SerializeField]
	GameObject currentView;
	GameObject lastView;

	static float transitionTime = 1.0f;


	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}


	// Use this for initialization
	void Start () {

		//SetActiveView(playerInfoView);

		HideView(playerInfoView, OffscreenUp());
		TransitionViewInDown(playerInfoView);
	}

	// LAYOUT HELPERS
	//===============


	Vector3 CenterScreen()
	{
		return new Vector3(Screen.width / 2, Screen.height / 2, 0);
	}

	Vector3 OffscreenUp()
	{
		return new Vector3(Screen.width / 2, Screen.height * 3/2, 0);
	}


	// VIEW TRANSITIONS
	//=================


	void SetActiveView (GameObject view)
	{
		currentView = view;
		view.transform.position = CenterScreen();
		view.SetActive(true);
	}

	void HideView (GameObject view, Vector3 position )
	{
		view.SetActive(false);
		view.transform.position = position;
	}

	public void HideInfoView()
	{
		Debug.Log("HideInfoView");
		TransitionViewOutUp(playerInfoView);
	}

	public void ShowInfoView()
	{
		TransitionViewInDown(playerInfoView);
	}


	public void TransitionViewOutUp(GameObject view)
	{
		lastView = view;
		//iTween.MoveTo(view, new Vector3(0, Screen.height * 2, 0), transitionTime);
		Hashtable ht = iTween.Hash(
			"y", OffscreenUp().y,
		    "time", transitionTime,
			"onComplete", "OnTransitionComplete",
		     "onCompleteTarget", gameObject
		);

		iTween.MoveTo(view, ht);
	}

	public void TransitionViewInDown(GameObject view)
	{
		view.transform.position = OffscreenUp();
		view.SetActive(true);

		iTween.MoveTo(view, CenterScreen(), transitionTime);

		currentView = view;
	}
}
