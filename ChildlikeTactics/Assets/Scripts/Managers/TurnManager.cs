using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


//Handles the enemy's turn, and switching between player/enemy turns
public class TurnManager : MonoBehaviour {

	public bool playerTurn = true;
    public bool inCombat = false;
    public bool doneMoving = true;
    public Object thisLock = new Object();

    public List<GameObject> combatants = new List<GameObject>();
    protected PlayerUnitManager playerUnitManager;
    public int player = 0;

    public SortedList initiativeOrder = new SortedList();

    private int counter;

    private Map map;

    void Update() {
        if (!playerTurn) {
            //EnemyTurn();
        }
        if (map != null) {
            if (map.playerInRoomWithEnemies()) {
                if (!inCombat) {
                    inCombat = true;
                    EnterCombat();
                }
                else {
                    inCombat = true;
                }
            }
            else {
                inCombat = false;
                playerUnitManager.units[0].myTurn = true;
                playerUnitManager.units[1].myTurn = false;
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

    public void NextTurn() {
        bool dead = false;
        
        int key = (int)initiativeOrder.GetKey(0);
        print(key);
        //print(counter);
        GameObject combatant = (GameObject)initiativeOrder[key];
        if (combatant.CompareTag("Enemy")) {
            playerTurn = false;
            if (combatant.GetComponent<EnemyController>().isDead) {
                dead = true;
            }
            else {
                StartCoroutine(combatant.GetComponent<EnemyController>().Attack());
                counter = key + combatant.GetComponent<EnemyController>().initiative;
            }
        }
        else {
            playerTurn = true;
            combatant.GetComponent<PlayerController>().myTurn = true;
            counter = key + combatant.GetComponent<PlayerController>().initiative;
        }
        initiativeOrder.Remove(key);
        if (!dead) {
            key = counter;
            while (initiativeOrder.Contains(key)) {
                key++;
            }
            initiativeOrder.Add(key, combatant);
            if (combatant.CompareTag("Enemy")) {
                NextTurn();
            }
        }
        else {
            NextTurn();
        }

        //print(initiativeOrder.GetKey(0));
    }

    public void EnterCombat() {
        int key;
        initiativeOrder.Clear();
        foreach (GameObject combatant in combatants) {
            if (combatant.CompareTag("Enemy")) {
                if (combatant.GetComponent<EnemyController>().room.containsPlayer) {
                    key = combatant.GetComponent<EnemyController>().initiative;
                    while (initiativeOrder.Contains(key)) {
                        key++;
                    }
                    initiativeOrder.Add(key, combatant);
                }
            }
            else {
                key = combatant.GetComponent<PlayerController>().initiative;
                combatant.GetComponent<PlayerController>().myTurn = false;
                while (initiativeOrder.Contains(key)) {
                    key++;
                }
                initiativeOrder.Add(key, combatant);
            }
        }
        NextTurn();
    }
}
