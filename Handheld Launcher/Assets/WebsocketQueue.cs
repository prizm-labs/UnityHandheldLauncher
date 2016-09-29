using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


namespace AssemblyCSharp
{
	public class WebsocketQueue
	{
		public WebsocketQueue ()
		{
		}
	}
}

public class Wait {
	public Wait(MonoBehaviour mb, float seconds, Action a) {
		mb.StartCoroutine(RunAndWait(seconds, a));
	}

	IEnumerator RunAndWait(float seconds, Action a) {
		yield return new WaitForSeconds(seconds);
		a();
	}
}


public enum State { NotRunning, Running, Connected, Ping, Pong, Done }

public delegate void Handler();

public enum Topics { Unknown, PlayerDescriptor, ZoneDescriptor, PieceDescriptor };

public class WebsocketMessageQueue : MonoBehaviour {
	private readonly object syncLock = new object();
	private readonly Queue<string> pendingMessages = new Queue<string>();

	private readonly Dictionary<Topics, OnMessageDelegate> handlers
	= new Dictionary<Topics, OnMessageDelegate>();

	public delegate void OnMessageDelegate(JSONObject data);

	[SerializeField]
	private string currentMessage = "";


	public void AddHandler(Topics state, OnMessageDelegate handler) {

		if (handlers.ContainsKey(state)) {
			handlers [state] += handler;

		} else {
			handlers.Add(state, handler);
		}
		//handlers.Add(state, handler);
	}

	public void QueueMessage(string message) {
		State cur;
		lock(syncLock) {
			//cur = currentState;
			pendingMessages.Enqueue(message);
		}

	}

	public void Start () {


	}

	public void Update() {
		while (pendingMessages.Count > 0) {

			currentMessage = pendingMessages.Dequeue();
			Debug.Log ("ws message: " + currentMessage);

			JSONObject json;

			try {
				json = new JSONObject (currentMessage);
			} catch (System.Exception e) {
				Debug.LogWarning ("probably improperly formatted json in debugging");
				Debug.LogWarning (e);
			}

			json = new JSONObject (currentMessage);
			string topicString;
			Topics topic; 

			try {
				json.GetField ("type");

			} catch (System.Exception e) {
				Debug.LogWarning ("message without topic");
				Debug.LogWarning (e);
			}

			topicString = json.GetField ("type").str;

			switch (topicString) {

			case "PlayerDescriptor":
				topic = Topics.PlayerDescriptor;
				break;

			case "ZoneDescriptor":
				topic = Topics.ZoneDescriptor;
				break;

			case "PieceDescriptor":
				topic = Topics.PieceDescriptor;
				break;

			default:
				topic = Topics.Unknown;
				break;
			}

			try {
				OnMessageDelegate onMessageDelegate;
				if (handlers.TryGetValue(topic, out onMessageDelegate)) {
					onMessageDelegate (json);
				}
			} catch (System.Exception e) {
				Debug.LogWarning ("message without known topic");
				Debug.LogWarning (e);
			}
				
		}
	}
}

public class StateMachine : MonoBehaviour {
	private readonly object syncLock = new object();
	private readonly Queue<State> pendingTransitions = new Queue<State>();
	private readonly Dictionary<State, Handler> handlers
	= new Dictionary<State, Handler>();

	[SerializeField]
	private State currentState = State.NotRunning;

	public void Run() {
		Transition(State.Running);
	}

	public void AddHandler(State state, Handler handler) {
		handlers.Add(state, handler);
	}

	public void Transition(State state) {
		State cur;
		lock(syncLock) {
			cur = currentState;
			pendingTransitions.Enqueue(state);
		}

		Debug.Log("Queued transition from " + cur + " to " + state);
	}

	public void Update() {
		while (pendingTransitions.Count > 0) {
			currentState = pendingTransitions.Dequeue();
			Debug.Log("Transitioned to state " + currentState);

			Handler handler;
			if (handlers.TryGetValue(currentState, out handler)) {
				handler();
			}
		}
	}
}


public class StatefulMain : MonoBehaviour {
	public StateMachine stateMachine;

	private WebSocket ws;

	void Start() {
		ws = new WebSocket("ws://echo.websocket.org");

		ws.OnOpen += OnOpenHandler;
		ws.OnMessage += OnMessageHandler;
		ws.OnClose += OnCloseHandler;

		stateMachine.AddHandler(State.Running, () => {
			new Wait(this, 3, () => {
				ws.ConnectAsync();
			});
		});

		stateMachine.AddHandler(State.Connected, () => {
			stateMachine.Transition(State.Ping);
		});

		stateMachine.AddHandler(State.Ping, () => {
			new Wait(this, 3, () => {
				ws.SendAsync("This WebSockets stuff is a breeze!", OnSendComplete);
			});
		});

		stateMachine.AddHandler(State.Pong, () => {
			new Wait(this, 3, () => {
				ws.CloseAsync();
			});
		});

		stateMachine.Run();
	}

	private void OnOpenHandler(object sender, System.EventArgs e) {
		Debug.Log("WebSocket connected!");
		stateMachine.Transition(State.Connected);
	}

	private void OnMessageHandler(object sender, MessageEventArgs e) {
		Debug.Log("WebSocket server said: " + e.Data);
		stateMachine.Transition(State.Pong);
	}

	private void OnCloseHandler(object sender, CloseEventArgs e) {
		Debug.Log("WebSocket closed with reason: " + e.Reason);
		stateMachine.Transition(State.Done);
	}

	private void OnSendComplete(bool success) {
		Debug.Log("Message sent successfully? " + success);
	}
}

public class Main : MonoBehaviour {
	private WebSocket ws;

	void Start() {
		ws = new WebSocket("ws://echo.websocket.org");

		ws.OnOpen += OnOpenHandler;
		ws.OnMessage += OnMessageHandler;
		ws.OnClose += OnCloseHandler;

		ws.ConnectAsync();        
	}

	private void OnOpenHandler(object sender, System.EventArgs e) {
		Debug.Log("WebSocket connected!");
		Thread.Sleep(3000);
		ws.SendAsync("This WebSockets stuff is a breeze!", OnSendComplete);
	}

	private void OnMessageHandler(object sender, MessageEventArgs e) {
		Debug.Log("WebSocket server said: " + e.Data);
		Thread.Sleep(3000);
		ws.CloseAsync();
	}

	private void OnCloseHandler(object sender, CloseEventArgs e) {
		Debug.Log("WebSocket closed with reason: " + e.Reason);
	}

	private void OnSendComplete(bool success) {
		Debug.Log("Message sent successfully? " + success);
	}
}