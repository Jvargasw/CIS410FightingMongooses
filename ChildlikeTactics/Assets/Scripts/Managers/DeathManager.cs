﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    /// TBA, commented out for warnings reasons
    string[] messageArray = new string[] { "Good work there kid, you're dead", "Nice going dingus", "A mind is a terrible thing to waste...", "What an immature way to die...", "Insert death message here" };
    
    void OnEnable()
    {
        //change the text to somthing witty
        Transform DiedText = transform.FindChild("DiedText");
        Text t = DiedText.GetComponent<Text>();
        t.text = messageArray[Random.Range(0, messageArray.Length)];
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("Start_screen");
    }
    public void ContinueGame()
    {

        SceneManager.LoadScene("Main_Play");
    }

}