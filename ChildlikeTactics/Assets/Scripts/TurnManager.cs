using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


//Handles the enemy's turn, and switching between player/enemy turns
public class TurnManager : MonoBehaviour {


	public static bool playerTurn = true;
    public bool inCombat = false;

    public List<GameObject> combatants = new List<GameObject>();
    protected PlayerUnitManager playerUnitManager;
    public int player = 0;

    private Map map;

    void Update() {
        if (!playerTurn) {
            EnemyTurn();
        }
        if (map != null) {
            if (map.playerInRoomWithEnemies()) {
                if (!inCombat) {
                    inCombat = true;
                    //EnterCombat();
                }
                else {
                    inCombat = true;
                }
            }
            else {
                inCombat = false;
            }
        }

    }

    //public int maxEnemies = 10;
    public GameObject[] enemies;

    void Start() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;
        playerUnitManager = GameObject.Find("GameManager").GetComponent<PlayerUnitManager>();
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

    public void NextTurn() {
        /*playerUnitManager.units[player].myTurn = false;
        player++;
        if(player >= 2) {*/
            playerTurn = false;/*
            player = 0;
        }
        playerUnitManager.units[player].myTurn = true;
        playerUnitManager.NextPlayer();*/
    }
		
}
