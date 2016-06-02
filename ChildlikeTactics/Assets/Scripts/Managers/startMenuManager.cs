using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class startMenuManager : MonoBehaviour {

    public Canvas quitMenu;
    public Canvas seedMenu;
    public Button playButton;
    public Button exitButton;
    public InputField textBox;

	// Use this for initialization
	void Start ()
    {
        quitMenu = quitMenu.GetComponent<Canvas>();
        quitMenu.enabled = false;

        seedMenu = seedMenu.GetComponent<Canvas>();
        seedMenu.enabled = false;

        playButton = playButton.GetComponent<Button>();
        exitButton = exitButton.GetComponent<Button>();
	
	}

    public void GeneratePress()
    {
        SceneManager.LoadScene("Main_Play");
        PersistentStorage.seed = int.Parse(textBox.text);
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
        seedMenu.enabled = true;
        textBox.text = PersistentStorage.seed.ToString();
    }

	public void TutorialButton() {
		SceneManager.LoadScene ("Controls_Scene");
	}

    public void ExitGame()
    {
        Application.Quit();
    }
}
