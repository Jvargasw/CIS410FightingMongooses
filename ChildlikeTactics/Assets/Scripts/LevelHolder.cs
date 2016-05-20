﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelHolder : MonoBehaviour {

	public static int level = 0;

	public static LevelHolder instance = null;

	public GameObject levelText;

	public void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		else if (instance != this)
			Destroy (gameObject);
	}

	public void Start() {
		levelText.GetComponent<Text>().text = "level: " + (level + 1).ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}