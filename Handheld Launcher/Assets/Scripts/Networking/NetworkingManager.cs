using UnityEngine;
using System.Collections;
using Prizm;

public class NetworkingManager : MonoBehaviour {

	// Use this for initialization
	void Start () {


		gameObject.AddComponent<NetworkDiscovery> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool HasInternetLANConnection()
	{
		bool isConnectedToInternet = false;
		// Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
		if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			isConnectedToInternet = true;
		}
		return isConnectedToInternet;
	}

	//		To check for Internet Connection,
	IEnumerator CheckForConnection() 
	{
		Ping png = new Ping("139.130.4.5");
		float startTime = Time.time;      
		while (Time.time < startTime + 5.0f)       
		{         
			yield return new WaitForSeconds(0.1f);      
		}
		if(png .isDone)      
		{
			print("Connected!");  
		}     
		else    
		{    
			print("Not Connected!");
		}
	}

}
