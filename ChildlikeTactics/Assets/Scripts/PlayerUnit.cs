using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class PlayerUnit : MonoBehaviour {

	public float playerSpeed = 3.0f;

	public int maxMoveDistance = 3; //the most spaces the player can move in a turn
	public int playerDmg = 10;
	public int health = 100;
	public int maxHealth = 100;
	public int level = 1;
	public int nxtlvlxp = 100;
	public int exp = 0;
	public int def = 0;
	public int initiative = 10;

	public bool isMoving; //represents moving vs attacking

	public Vector3 movePosition;
	public Vector3 startPosition;

	public int spacesMoved = 0;

	public GameObject healthBar;
	protected GameObject playerRenderer;
	protected Animator animController;

	public int lvlUp = 0;
	public bool myTurn = false;

	protected Text turnText;

	protected PlayerUnitManager playerUnitManager;
	protected TurnManager turnManager;
	protected List<PlayerUnit> unitManager;
	protected List<IEnumerator> routineManager;

	protected void Start()
	{
		print ("PlayerUnit start");

		turnText = GameObject.Find("TurnText").GetComponent<Text>();

		//get reference to PlayerRenderer parented to this unit
		playerRenderer = transform.Find("EmptyRot/PlayerRenderer").gameObject;
		animController = playerRenderer.GetComponent<Animator>();

		//init reference to PlayerUnitManager script and vars
		playerUnitManager = GameObject.Find("GameManager").GetComponent<PlayerUnitManager>();
		turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
		unitManager = playerUnitManager.units;
		routineManager = playerUnitManager.routines;

		//register this unit with the PlayerUnitManager
		unitManager.Add(this);
		routineManager.Add (PlayerTurn());
		print ("unit registered");

		turnManager.combatants.Add(this.gameObject);

		StartMoving();
	}

	protected void Update(){
		//update healthbar
		healthBar.transform.localScale = new Vector3 (((float) health/(float) maxHealth)*.85f, 0.25f, 1f);
	}

	//abstract methods to be defined per player unit
	public abstract IEnumerator PlayerTurn();

	public abstract void AttemptMove (Vector2 moveDirection);

	public abstract void TakeDmg (int enemyDmg);

	public abstract void Heal(int heal);

	public abstract void IncreaseDmg(int dmg);

	public abstract void DecreaseDmg(int dmg);

	public abstract void IncreaseDef(int dmg);

	public abstract void MeleeAttack (GameObject enemy);

	public abstract void Die ();

	public abstract void Follow(Vector3 pos, int index);

	public abstract void CheckLevelUp();

	public abstract void LevelUp();

	public abstract void ExpDisplayUpdate();

	public abstract void DmgDisplayUpdate();



	public void PlayerEndTurn() {
		turnText.text = "Enemy Turn";
		myTurn = false;
		StartMoving();
		spacesMoved = 0;
		transform.position = movePosition;
		turnManager.NextTurn();
		playerUnitManager.UpdateStatsPanel ();
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

	void OnTriggerEnter(Collider coll) {
		if (coll.CompareTag ("Exit")) {
			SceneManager.LoadScene("Main_Play");
		}
	}


}