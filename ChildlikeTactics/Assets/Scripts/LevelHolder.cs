using UnityEngine;
using System.Collections;

public class LevelHolder : MonoBehaviour {

	public static int level = 0;

	public static LevelHolder instance = null;

	public void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		else if (instance != this)
			Destroy (gameObject);
	}

	public void Start() {
		print ("level: " + level.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
