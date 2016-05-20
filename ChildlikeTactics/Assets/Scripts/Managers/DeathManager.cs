using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathManager : MonoBehaviour
{
    string[] messageArray = new string[] { "Good work there kid, you're dead", "Nice going dingus", "A mind is a terrible thing to waste...", "What an immature way to die...", "Insert death message here" };
    public GameObject DiedText;
    void OnEnable()
    {
        //change the text to somthing witty
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
