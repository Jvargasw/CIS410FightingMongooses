using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	
	FMODUnity.StudioEventEmitter emitter;
	// Use this for initialization
	GameObject manager;
	void OnEnable () 
	{
		emitter = GetComponent<FMODUnity.StudioEventEmitter> ();
		manager = GameObject.Find ("GameManager");

	}
	
	// Update is called once per frame
	void Update () 
	{
		
		TurnManager managerScript = manager.GetComponent<TurnManager> ();

		if( managerScript.inCombat == true)
		{ 
			emitter.SetParameter("Battle", 1);
		
		}

		else
		{
			emitter.SetParameter("Battle", 0);
		}
	}

	public void EndMusic()
	{

		emitter.Stop ();
	}
}
