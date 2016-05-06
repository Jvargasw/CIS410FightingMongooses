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
        this.GetComponent<EnemyManager>().EnemyTurn();
		playerTurn = true;
		return;
	}
}
