﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : PlayerUnit
{

    //public float playerSpeed = 3.0f;

	//public int maxMoveDistance = 3; //the most spaces the player can move in a turn
    //public int playerDmg = 10;
    //public int health = 100;

    //private bool isMoving; //represents moving vs attacking

	//private Vector3 movePosition;
	//private Vector3 startPosition;
	//private int spacesMoved = 0;
    private Text turnText;
    private Text healthText;
    private Map map;

    void Start()
    {
		StartCoroutine (PlayerTurn ());

        turnText = GameObject.Find("TurnText").GetComponent<Text>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        StartMoving();
        turnText.text = "Moving";
        healthText.text = "HP: " + health;
        map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;
    }

	IEnumerator PlayerTurn() {
		movePosition = transform.position;
		startPosition = transform.position;

		//while it's the player's turn
		while (TurnManager.playerTurn) {

			//horizontal movement
			if (Input.GetButtonDown ("Horizontal")) {
				if (Input.GetAxisRaw ("Horizontal") > 0) {
					AttemptMove (Vector2.right);
				} else {
					AttemptMove (Vector2.left);
				}
				yield return null;
			//vertical movement
			} else if (Input.GetButtonDown ("Vertical")) {
				if (Input.GetAxisRaw ("Vertical") > 0) {
					AttemptMove (Vector2.up);
				} else {
					AttemptMove (Vector2.down);
				}
				yield return null;

			//end turn
			} else if (Input.GetKeyDown (KeyCode.Return)) {
                if (isMoving) {
                    StopMoving();
                    turnText.text = "Attacking";
                }
                else {
                    PlayerEndTurn();
                }
				yield return null;
			} else if (Input.GetKeyDown (KeyCode.Backspace)) {
                if (isMoving) {
                    spacesMoved = 0;
                    movePosition = startPosition;
                    transform.position = movePosition;
                }
				yield return null;
			}
			yield return null;
		}

		yield break;
	}


	override public void AttemptMove(Vector2 moveDirection) {
        //Moving
        if(isMoving){
            //have we already moved the max number of spaces?
            if ((spacesMoved + 1 <= maxMoveDistance)) {
                if (map.movePlayerTo((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x))) { //OK, the X and Y being swapped is kinda a problem.
                    spacesMoved++;
                    movePosition += new Vector3(moveDirection.x, moveDirection.y, 0);
                    transform.position = movePosition;
                }
            }
            return;
        }

        //Attacking
        if (map.getTileAt((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x)) == TileType.ENEMY ||
            map.getTileAt((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x)) == TileType.BOSS){

            int index = map.getPlayerCollidedWith((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x));
            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
                if(index == enemy.GetComponent<EnemyController>().index) {
                    MeleeAttack(enemy);
                    PlayerEndTurn();
                    return;
                }
                else {
                    print("Error with fighting enemies");
                }
            }
        }
		return;
	}

    public void PlayerEndTurn() {
        turnText.text = "Enemy Turn";
        TurnManager.playerTurn = false;
        StartMoving();
        turnText.text = "Moving";
        spacesMoved = 0;
        transform.position = movePosition;
    }
	/*
    override public void StopMoving() {
        isMoving = false;
        turnText.text = "Attacking";
    }

    override public void StartMoving() {
        isMoving = true;
        turnText.text = "Moving";
    }
	*/
    override public void TakeDmg(int enemyDmg) {
        health -= enemyDmg;
        healthText.text = "HP: " + health;
        if(health <= 0) {
            Die();
        }
    }

    override public void MeleeAttack(GameObject enemy) {
        enemy.GetComponent<EnemyController>().TakeDmg(playerDmg);
    }

    override public void Die() {
        healthText.text = "YOU DIED!";
        print("You died, loser.");
    }
}
