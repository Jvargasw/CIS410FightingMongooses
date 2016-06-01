using UnityEngine;
using System.Collections;

public class PauseMenuManager : MonoBehaviour {

    protected PlayerUnitManager playerUnitManager;

    // Use this for initialization
    void Start () {
        playerUnitManager = GameObject.Find("GameManager").GetComponent<PlayerUnitManager>();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ExitGame()
	{
		Application.Quit();
	}
	public void ResumeGame() {
		Time.timeScale = 1;
        playerUnitManager.paused = false;
		gameObject.SetActive (false);
	}

}
