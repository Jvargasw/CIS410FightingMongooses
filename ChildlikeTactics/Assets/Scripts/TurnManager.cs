using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//Handles the enemy's turn, and switching between player/enemy turns
public class TurnManager : MonoBehaviour {


	public bool playerTurn = true;

    void Update() {
        if (!playerTurn) {
            EnemyTurn();
        }

    }

    //public int maxEnemies = 10;
    public GameObject[] enemies;

    void Start() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void EnemyTurn() {
        foreach (GameObject enemy in enemies) {
            if (enemy.GetComponent<EnemyController>().isActiveAndEnabled) {
                enemy.GetComponent<EnemyController>().Attack();
            }
        }
        playerTurn = true;
        return;
    }
		
}
