﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : PlayerUnit
{
	/* vvv These are declared in PlayerUnit vvv
    public float playerSpeed = 3.0f;

	public int maxMoveDistance = 3; //the most spaces the player can move in a turn
    public int playerDmg = 10;
    public int health = 100;

    private bool isMoving; //represents moving vs attacking

	private Vector3 movePosition;
	private Vector3 startPosition;
	private int spacesMoved = 0;
	*/
    
	private Map map;

    void Start()
    {
		//execute PlayerUnit's start code
		base.Start();

        healthText.text = "HP: " + health;
        dmgText.text = "DMG: " + playerDmg;
        map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;
    }

	void Update(){
		base.Update ();
	}

	override public IEnumerator PlayerTurn() {
		//movePosition = transform.position;
		//startPosition = transform.position;

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
			}
            //Removed undo movement capability due to it not being implemented to work with the grid system, as well as conflicts with item pickups. 
            //(e.g. go pickup an item, undo movement, still have item without using up movement)
            /*else if (Input.GetKeyDown (KeyCode.Backspace)) {
                if (isMoving) {
                    spacesMoved = 0;
                    movePosition = startPosition;
                    transform.position = movePosition;
                }
				yield return null;
			}*/
			yield return null;
		}

		yield break;
	}


	override public void AttemptMove(Vector2 moveDirection) {
        //Moving
        if(isMoving){
            //have we already moved the max number of spaces?
            if ((spacesMoved + 1 <= maxMoveDistance) || !map.playerInRoomWithEnemies()) {
                if (map.movePlayerTo((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x))) { //OK, the X and Y being swapped is kinda a problem.
                    spacesMoved++;
                    movePosition = new Vector3(moveDirection.x, moveDirection.y, 0) + transform.position;
                    Vector3 oldPosition = transform.position;
                    transform.position = movePosition;
                    if(!map.playerInRoomWithEnemies())
                    {
                        spacesMoved = 0;
                        if (playerUnitManager.activeUnitIndex != 0) {
                            playerUnitManager.setUnit(0);
                        }
                        else {
                            int index = playerUnitManager.activeUnitIndex + 1;
                            unitManager[index].Follow(oldPosition, index);
                        }
                    }
                }
                else {
                    if (map.getTileAt((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x)) == TileType.ITEM) {
                        int index = map.getPlayerCollidedWith((int)(moveDirection.y + transform.position.y), (int)(moveDirection.x + transform.position.x));
                        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item")) {
                            ItemController ic = item.GetComponent<ItemController>();
                            if (index == ic.index) {
                                ItemType itemType = ic.GetItemType();
                                if(itemType == ItemType.HP) {
                                    foreach (PlayerUnit player in unitManager) {
                                        player.Heal(ic.stats[0]);
                                    }
                                }
                                else if (itemType == ItemType.DMG) {
                                    foreach (PlayerUnit player in unitManager) {
                                        player.IncreaseDmg(ic.stats[0]);
                                    }
                                }
                                else {
                                    print("Unknown Item Type on Pickup");
                                }
                                map.pickupItem(index);
                                item.gameObject.SetActive(false);
                                AttemptMove(moveDirection);
                                return;
                            }
                        }
                    }
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

    override public void Heal(int heal) {
        health += heal;
        if (health >= maxHealth) {
            health = maxHealth;
        }
        healthText.text = "HP: " + health;
    }

    override public void IncreaseDmg(int dmg) {
        playerDmg += dmg;
        dmgText.text = "DMG: " + playerDmg;
    }

    override public void DecreaseDmg(int dmg) {
        playerDmg -= dmg;
        dmgText.text = "DMG: " + playerDmg;
    }

    override public void MeleeAttack(GameObject enemy) {
        enemy.GetComponent<EnemyController>().TakeDmg(playerDmg);
    }

    override public void Die() {
        healthText.text = "YOU DIED!";
        print("You died, loser.");
    }

    override public void Follow(Vector3 pos, int index) {
        transform.position = pos;
        playerUnitManager.updateMap(index, (int)pos.y, (int)pos.x);
    }
}
