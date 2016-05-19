using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//Handles the enemy's turn, and switching between player/enemy turns
public class TurnManager : MonoBehaviour {
	public GameObject enemyTurnText;


	public static bool playerTurn = true;
	//public bool playerTurnTest = true;

	float startXPos;
	RectTransform textTransform;



	// Use this for initialization
	void Start () {
		textTransform = enemyTurnText.GetComponent<RectTransform> ();
		startXPos = textTransform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (!playerTurn) {
			StartCoroutine (enemyTurnSwitch ());
		}

    }

	/*private void enemyTurn() {
        this.GetComponent<EnemyManager>().EnemyTurn();
		playerTurn = true;
		return;
	}*/


	//Scrolls "Enemy Phase" Text across the screen and then starts the enemy turn
	IEnumerator enemyTurnSwitch() {
		while (textTransform.position.x < 900) {
			print (textTransform.position.x.ToString ());
			textTransform.position += new Vector3(400, 0, 0) * Time.deltaTime;
			yield break;
		}
		textTransform.position = new Vector3 (startXPos, textTransform.position.y, textTransform.position.z);
		this.GetComponent<EnemyManager>().EnemyTurn();
		playerTurn = true;
		yield return null;
	}
}
