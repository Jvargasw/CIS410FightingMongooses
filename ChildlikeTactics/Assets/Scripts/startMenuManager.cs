using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class startMenuManager : MonoBehaviour {

    public Canvas quitMenu;
    public Button playButton;
    public Button exitButton;
	// Use this for initialization
	void Start ()
    {
        quitMenu = quitMenu.GetComponent<Canvas>();
        quitMenu.enabled = false;
        playButton = playButton.GetComponent<Button>();
        exitButton = exitButton.GetComponent<Button>();
	
	}

    public void ExitPress()
    {
        quitMenu.enabled = true;
        playButton.enabled = false;
        exitButton.enabled = false;
    }
    public void NoPress()
    {
        quitMenu.enabled = false;
        playButton.enabled = true;
        exitButton.enabled = true;
    }

    public void StartLevel()
    {
        SceneManager.LoadScene("Main_Play");
    }

}
