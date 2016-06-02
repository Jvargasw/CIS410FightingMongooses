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
    public int playerNum;

	public bool isMoving; //represents moving vs attacking

	public Vector3 movePosition;
	public Vector3 startPosition;

	public int spacesMoved = 0;

	public GameObject healthBar;
	protected GameObject playerRenderer;
	protected Animator animController;

	public int lvlUp = 0;
	public bool myTurn = false;

	//refrence to camera for music managment
	GameObject cameraRef;

	protected Text turnText;

	protected PlayerUnitManager playerUnitManager;
	protected TurnManager turnManager;
	protected List<PlayerUnit> unitManager;
	protected List<IEnumerator> routineManager;
    protected PersistentStorage persistentStorage;

	protected void Start()
	{
		cameraRef = GameObject.Find ("Main Camera");
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
        if(playerNum == 1) {
            if(PersistentStorage.playerDamage1 == 0) {
                PersistentStorage.playerDamage1 = playerDmg;
            }
            playerDmg = PersistentStorage.playerDamage1;
            if(PersistentStorage.playerHealth1 == 0) {
                PersistentStorage.playerHealth1 = maxHealth;
            }
            maxHealth = PersistentStorage.playerHealth1;
            if(PersistentStorage.playerDefense1 == 0) {
                PersistentStorage.playerDefense1 = def;
            }
            def = PersistentStorage.playerDefense1;
            if(PersistentStorage.playerInitiative1 == 0) {
                PersistentStorage.playerInitiative1 = initiative;
            }
            initiative = PersistentStorage.playerInitiative1;
            if(PersistentStorage.playerMovement1 == 0) {
                PersistentStorage.playerMovement1 = maxMoveDistance;
            }
            maxMoveDistance = PersistentStorage.playerMovement1;
            if (PersistentStorage.playerExp1 == 0) {
                PersistentStorage.playerExp1 = exp;
            }
            exp = PersistentStorage.playerExp1;
            if (PersistentStorage.playerNextExp1 == 0) {
                PersistentStorage.playerNextExp1 = nxtlvlxp;
            }
            nxtlvlxp = PersistentStorage.playerNextExp1;
        }
        else {
            if (PersistentStorage.playerDamage2 == 0) {
                PersistentStorage.playerDamage2 = playerDmg;
            }
            playerDmg = PersistentStorage.playerDamage2;
            if (PersistentStorage.playerHealth2 == 0) {
                PersistentStorage.playerHealth2 = maxHealth;
            }
            maxHealth = PersistentStorage.playerHealth2;
            if (PersistentStorage.playerDefense2 == 0) {
                PersistentStorage.playerDefense2 = def;
            }
            def = PersistentStorage.playerDefense2;
            if (PersistentStorage.playerInitiative2 == 0) {
                PersistentStorage.playerInitiative2 = initiative;
            }
            initiative = PersistentStorage.playerInitiative2;
            if (PersistentStorage.playerMovement2 == 0) {
                PersistentStorage.playerMovement2 = maxMoveDistance;
            }
            maxMoveDistance = PersistentStorage.playerMovement2;
            if (PersistentStorage.playerExp2 == 0) {
                PersistentStorage.playerExp2 = exp;
            }
            exp = PersistentStorage.playerExp2;
            if (PersistentStorage.playerNextExp2 == 0) {
                PersistentStorage.playerNextExp2 = nxtlvlxp;
            }
            nxtlvlxp = PersistentStorage.playerNextExp2;
        }

        //register this unit with the PlayerUnitManager
        health = maxHealth;
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
	//refrence to the music manager script


	public void PlayerEndTurn() {
		turnText.text = "Enemy Turn";
		myTurn = false;
		StartMoving();
		spacesMoved = 0;
		transform.position = movePosition;
        StartCoroutine(turnManager.NextTurn());
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
		MusicManager musicScript = cameraRef.GetComponent<MusicManager> ();
		if (coll.CompareTag ("Exit")) {
			if (PersistentStorage.level >= 4) {  //First level is level 0 so fifth level is level 4
				//do yo thang
			} else {
				//stop the music so levels don't overlap
				musicScript.EndMusic ();
				LevelDelay ();
				SceneManager.LoadScene("Main_Play");
			}
		}
	}
	//method to place a delay into the level ending (doesnt seem to work??)
	IEnumerator LevelDelay()
	{
		yield return new WaitForSeconds (100);
	}


}