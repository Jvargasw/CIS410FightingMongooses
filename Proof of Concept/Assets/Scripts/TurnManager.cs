using UnityEngine;
using System.Collections;


//Handles the enemy's turn, and switching between player/enemy turns
public class TurnManager : MonoBehaviour {

	public static bool playerTurn = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!playerTurn) {
			enemyTurn ();
		}
	}

	private void enemyTurn() {
		print ("Watch out folks, we're enemies and we're gonna getcha");
		playerTurn = true;
		//fill out with actual enemy movement stuff
		return;
	}
}
