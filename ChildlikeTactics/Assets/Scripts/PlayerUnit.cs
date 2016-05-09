using UnityEngine;
using System.Collections;

public abstract class PlayerUnit : MonoBehaviour {

	public float playerSpeed = 3.0f;

	public int maxMoveDistance = 3; //the most spaces the player can move in a turn
	public int playerDmg = 10;
	public int health = 100;

	protected bool isMoving; //represents moving vs attacking

	protected Vector3 movePosition;
	protected Vector3 startPosition;
	protected int spacesMoved = 0;

	//abstract methods to be defined per player unit
	public abstract void AttemptMove (Vector2 moveDirection);

	public abstract void TakeDmg (int enemyDmg);

	public abstract void MeleeAttack (GameObject enemy);

	public abstract void Die ();

	public void StartMoving()
	{
		isMoving = true;
	}

	public void StopMoving()
	{
		isMoving = false;
	}


}
