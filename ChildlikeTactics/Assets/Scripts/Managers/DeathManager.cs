﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    /// TBA, commented out for warnings reasons
    string[] messageArray = new string[] { "Good work there kid, you're dead", "Nice going dingus", "A mind is a terrible thing to waste...", "What an immature way to die...", "Insert coin to continue" };
	GameObject cameraRef;
    void OnEnable()
    {
		cameraRef = GameObject.Find ("Main Camera");
		MusicManager musicScript = cameraRef.GetComponent<MusicManager> ();
        //change the text to somthing witty
		musicScript.EndMusic();
        Transform DiedText = transform.FindChild("DiedText");
        Text t = DiedText.GetComponent<Text>();
        t.text = messageArray[Random.Range(0, messageArray.Length)];
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start_screen");
    }
    public void ContinueGame()
    {

        Time.timeScale = 1;
        SceneManager.LoadScene("Main_Play");
    }

}
