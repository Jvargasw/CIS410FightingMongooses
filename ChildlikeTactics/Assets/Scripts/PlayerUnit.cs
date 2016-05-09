﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class PlayerUnit : MonoBehaviour {

	public float playerSpeed = 3.0f;

	public int maxMoveDistance = 3; //the most spaces the player can move in a turn
	public int playerDmg = 10;
	public int health = 100;
    public int maxHealth = 100;

	public bool isMoving; //represents moving vs attacking

	public Vector3 movePosition;
	public Vector3 startPosition;
	public int spacesMoved = 0;

	protected Text turnText;
	protected Text healthText;
    protected Text dmgText;

    protected List<PlayerUnit> manager;

	protected void Start()
	{
		print ("PlayerUnit start");

		turnText = GameObject.Find("TurnText").GetComponent<Text>();
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
        dmgText = GameObject.Find("DmgText").GetComponent<Text>();

        manager = GameObject.Find ("GameManager").GetComponent<PlayerUnitManager>().units;
		//register this unit with the PlayerUnitManager
		manager.Add(this);
		print ("unit registered");

		StartMoving();
		//healthText.text = "HP: " + health;
	}

	//abstract methods to be defined per player unit
	public abstract IEnumerator PlayerTurn();

	public abstract void AttemptMove (Vector2 moveDirection);

	public abstract void TakeDmg (int enemyDmg);

    public abstract void Heal(int heal);

    public abstract void IncreaseDmg(int dmg);

    public abstract void DecreaseDmg(int dmg);

    public abstract void MeleeAttack (GameObject enemy);

	public abstract void Die ();


	public void PlayerEndTurn() {
		turnText.text = "Enemy Turn";
		TurnManager.playerTurn = false;
		StartMoving();
		spacesMoved = 0;
		transform.position = movePosition;
	}

	public void StartMoving()
	{
		isMoving = true;
		turnText.text = "Moving";
	}

	public void StopMoving()
	{
		isMoving = false;
		turnText.text = "Attacking";
	}


}
