﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
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
        if (level >= 4)
        {
            SceneManager.LoadScene("Win_Scene");
        }
        else
        {
            levelText.GetComponent<Text>().text = "Level: " + (level + 1).ToString();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
