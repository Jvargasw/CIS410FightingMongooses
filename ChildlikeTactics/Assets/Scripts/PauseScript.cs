using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    private Text pauseText;
    private bool pauseGame = false;
    private bool showPauseMenu = false;
    void InitGame()
    {
        pauseText = GameObject.Find("PauseText").GetComponent<Text>();
        pauseText.enabled = false;
    }
	void Update ()
    {
        if (Input.GetKeyDown("p"))
        {
            pauseGame = !pauseGame;
            if (pauseGame == true)
            {
                Time.timeScale = 0;
                showPauseMenu = true;
            }
        }
        if (pauseGame == false)
        {
            Time.timeScale = 1;
            showPauseMenu = false;
        }

        if (showPauseMenu == true)
        {
            pauseText.enabled = true;
        }
        else
        {
            pauseText.enabled = false;
        }
    }
}
