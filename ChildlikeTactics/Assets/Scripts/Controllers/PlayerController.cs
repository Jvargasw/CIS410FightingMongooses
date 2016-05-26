using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

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

	new void Start()
	{
		//execute PlayerUnit's start code
		base.Start();

		//.text = "HP: " + health;
		DmgDisplayUpdate();
		map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;
	}

	new void Update(){
		base.Update ();
	}

    private bool Done() {
        return turnManager.isMoving == -1;
    }

    private bool MyTurn() {
        return turnManager.playerTurn;
    }

    override public IEnumerator PlayerTurn() {
		//movePosition = transform.position;
		//startPosition = transform.position;
		//print("TURN");

		//while it's the player's turn
		while (/*myTurn &&*/ turnManager.playerTurn) {
            if (!myTurn) {
                playerUnitManager.NextPlayer();
            }
            if (turnManager.isMoving != -1) {
                yield return new WaitUntil(Done);
            }
            if (turnManager.isMoving == -1) {
                //horizontal movement
                if (Input.GetButtonDown("Horizontal")) {
                    if (Input.GetAxisRaw("Horizontal") > 0) {
                        AttemptMove(Vector2.right);
                        //look right
                        playerRenderer.transform.rotation = Quaternion.Euler(0f, 270f, -270f);
                    }
                    else {
                        AttemptMove(Vector2.left);
                        //look left
                        playerRenderer.transform.rotation = Quaternion.Euler(0f, 90f, -90f);
                    }
                    yield return null;
                    //vertical movement
                }
                else if (Input.GetButtonDown("Vertical")) {
                    if (Input.GetAxisRaw("Vertical") > 0) {
                        AttemptMove(Vector2.up);
                        //look up
                        playerRenderer.transform.rotation = Quaternion.Euler(-270f, 180f, 0f);
                    }
                    else {
                        AttemptMove(Vector2.down);
                        //look down
                        playerRenderer.transform.rotation = Quaternion.Euler(-90f, 0f, -0f);
                    }
                    yield return null;

                    //end turn
                }
                else if (Input.GetKeyDown(KeyCode.Return)) {
                    if (isMoving) {
                        StopMoving();
                        turnText.text = "Attacking";
                    }
                    else {
                        PlayerEndTurn();
                    }
                    yield return null;
                }
                if (!turnManager.playerTurn) {
                    yield return new WaitUntil(MyTurn);
                }
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
        print("OH GOD HELP");
        yield return new WaitUntil(Done);
        StartCoroutine(PlayerTurn());
        yield break;
	}

	override public void AttemptMove(Vector2 moveDirection) {
		//Moving
		if(isMoving){
			//have we already moved the max number of spaces?
			if ((spacesMoved + 1 <= maxMoveDistance) || !turnManager.inCombat) {
				if (map.movePlayerTo((int)(moveDirection.x + transform.position.x), (int)(moveDirection.y + transform.position.y))) {
					spacesMoved++;
					movePosition = new Vector3(moveDirection.x, moveDirection.y, 0) + transform.position;
					Vector3 oldPosition = transform.position;
					transform.position = movePosition;
					if(!turnManager.inCombat)
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
					playerUnitManager.UpdateStatsPanel ();
				}
				else {
                    TileType thing = map.getTileAt((int)(moveDirection.x + transform.position.x), (int)(moveDirection.y + transform.position.y));
                    if (thing == TileType.ITEM) {
						int index = map.getPlayerCollidedWith((int)(moveDirection.x + transform.position.x), (int)(moveDirection.y + transform.position.y));
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
								else if (itemType == ItemType.DEF) {
									foreach (PlayerUnit player in unitManager) {
										player.IncreaseDef(ic.stats[0]);
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
                    else if(thing == TileType.PLAYER) {
                        if (!turnManager.inCombat) {
                            map.swapPlayerPositions();
                            movePosition = new Vector3(moveDirection.x, moveDirection.y, 0) + transform.position;
                            Vector3 oldPosition = transform.position;
                            transform.position = movePosition;
                            unitManager[1].transform.position = oldPosition;
                        }
                    }
				}
			}
			return;
		}

		//Attacking
		if (map.getTileAt((int)(moveDirection.x + transform.position.x), (int)(moveDirection.y + transform.position.y)) == TileType.ENEMY ||
			map.getTileAt((int)(moveDirection.x + transform.position.x), (int)(moveDirection.y + transform.position.y)) == TileType.BOSS){

			int index = map.getPlayerCollidedWith((int)(moveDirection.x + transform.position.x), (int)(moveDirection.y + transform.position.y));
			foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
				if(index == enemy.GetComponent<EnemyController>().index) {
					MeleeAttack(enemy);
					PlayerEndTurn();
					return;
				}
			}
			print("Error with fighting enemies");
		}
		return;
	}

	override public void TakeDmg(int enemyDmg) {
		int dmgTaken = enemyDmg - def;
		if(dmgTaken > 0) {
			health -= dmgTaken;
		}
		if(health <= 0) {
			Die();
		}
		playerUnitManager.UpdateStatsPanel ();
	}

	override public void Heal(int heal) {
		health += heal;
        AudioSource.PlayClipAtPoint(playerUnitManager.healSound, Camera.main.transform.position);
        if (health >= maxHealth) {
			health = maxHealth;
		}
		playerUnitManager.UpdateStatsPanel ();
	}

	override public void IncreaseDmg(int dmg) {
		playerDmg += dmg;
        if(playerNum == 1) {
            PersistentStorage.playerDamage1 = playerDmg;
        }
        else {
            PersistentStorage.playerDamage2 = playerDmg;
        }
		playerUnitManager.UpdateStatsPanel ();
	}

	override public void DecreaseDmg(int dmg) {
		playerDmg -= dmg;
        if (playerNum == 1) {
            PersistentStorage.playerDamage1 = playerDmg;
        }
        else {
            PersistentStorage.playerDamage2 = playerDmg;
        }
        playerUnitManager.UpdateStatsPanel ();
	}

	override public void IncreaseDef(int defense) {
		def += defense;
        if (playerNum == 1) {
            PersistentStorage.playerDefense1 = def;
        }
        else {
            PersistentStorage.playerDefense2 = def;
        }
        playerUnitManager.UpdateStatsPanel ();
	}

	override public void MeleeAttack(GameObject enemy) {
		//trigger attack animation
		animController.SetTrigger ("Attack");
        //play attack sound
        AudioSource.PlayClipAtPoint(playerUnitManager.attackSound, Camera.main.transform.position);
        EnemyController enemyC = enemy.GetComponent<EnemyController>();
		int enemyXP = enemyC.exp;
		if (enemyC.TakeDmg(playerDmg)) { //TakeDamage returns true when the enemy is killed
			exp += enemyXP;
			ExpDisplayUpdate();
			CheckLevelUp();
			playerUnitManager.UpdateStatsPanel ();
		}
	}

	override public void Die() {
        //healthText.text = "YOU DIED!";
        Time.timeScale = 0;
        playerUnitManager.DiedMenu.SetActive(true);
        
	}

	override public void Follow(Vector3 pos, int index) {
		transform.position = pos;
		playerUnitManager.updateMap(index, (int)pos.x, (int)pos.y);
        movePosition = new Vector3(pos.x, pos.y, 0);
    }

	override public void CheckLevelUp() {
		if(exp >= nxtlvlxp) {
			exp -= nxtlvlxp;
			nxtlvlxp = nxtlvlxp * 2;
			ExpDisplayUpdate();
			health = maxHealth;
			LevelUp();
			CheckLevelUp();
		}
	}

	public override void LevelUp() {
		lvlUp +=1;
	}

	public override void ExpDisplayUpdate() {
		//expText.text = exp + "/" + nxtlvlxp;
	}

	public override void DmgDisplayUpdate() {
		//dmgText.text = "DMG: " + playerDmg;
	}

	void OnGUI() {
		int w = Screen.width;
		int h = Screen.height;
		if (lvlUp > 0) {
			if (GUI.Button(new Rect(0, (h * 2) / 5, w / 4, h / 5), "HP+50%")) {
				maxHealth += maxHealth / 2;
				health = maxHealth;
                if (playerNum == 1) {
                    PersistentStorage.playerHealth1 = maxHealth;
                }
                else {
                    PersistentStorage.playerHealth2 = maxHealth;
                }
                lvlUp -= 1;
				playerUnitManager.UpdateStatsPanel ();
			}
			if (GUI.Button(new Rect(w/4, (h * 2) / 5, w / 4, h / 5), "DMG+50%")) {
				playerDmg += playerDmg / 2;
				DmgDisplayUpdate();
                if (playerNum == 1) {
                    PersistentStorage.playerDamage1 = playerDmg;
                }
                else {
                    PersistentStorage.playerDamage2 = playerDmg;
                }
                lvlUp -= 1;
				playerUnitManager.UpdateStatsPanel ();
			}
			if (GUI.Button(new Rect(w/2, (h * 2) / 5, w / 4, h / 5), "MOV+1")) {
				maxMoveDistance += 1;
                if (playerNum == 1) {
                    PersistentStorage.playerMovement1 = maxMoveDistance;
                }
                else {
                    PersistentStorage.playerMovement2 = maxMoveDistance;
                }
                lvlUp -= 1;
				playerUnitManager.UpdateStatsPanel ();
			}
			if (GUI.Button(new Rect(3*w/4, (h * 2) / 5, w / 4, h / 5), "DEF+1")) {
				def += 1;
                if (playerNum == 1) {
                    PersistentStorage.playerDefense1 = def;
                }
                else {
                    PersistentStorage.playerDefense2 = def;
                }
                lvlUp -= 1;
				playerUnitManager.UpdateStatsPanel ();
			}
		}
	}

}