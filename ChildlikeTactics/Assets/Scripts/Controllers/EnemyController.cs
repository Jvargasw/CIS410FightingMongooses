using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour {

    public int index;
    public int health;
    public int dmg;
    public int movement = 5;
    public int initiative = 15;
    //bool for the boss prefab
    public bool isBoss;
    public int def = 0;
    public int exp = 10;

    private int range = 1;

    private GameObject tileManager;
    private TurnManager turnManager;
	private Map map;
    private bool success;

	private TileType[,] grid;
	private bool[,] tiles; //array of bools for whether or not each tile is walkable

    private int test = 0;
    private List<PlayerUnit> unitManager;

    private PlayerUnit target;

    void Start () {
        if (health == 0) {
            health = 10;
        }
        if(dmg == 0) {
            dmg = 5;
        }
        tileManager = GameObject.FindGameObjectWithTag("TileManager");
        unitManager = GameObject.Find("GameManager").GetComponent<PlayerUnitManager>().units;
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        //turnManager.combatants.Add(this.gameObject);

        map = tileManager.GetComponent<Generate> ().map;

    }

	private void updateWalkables() {

	}

	private void FindPath (Vector3 start, Vector3 end) {
		List<Vector3> openSet = new List<Vector3>();
		HashSet<Vector3> closedSet = new HashSet<Vector3> ();
		openSet.Add (start);
		openSet.Add (end);

		while (openSet.Count > 0) {

		}
	}
	
	public void Attack() {
        if (SeekAndDestroy(range,movement)) {
            MeleeAttack(target); //Change this once we add multiple Players
        }
        turnManager.NextTurn();
    }

    public void TakeDmg(int playerDmg) {
        int dmgTaken = playerDmg - def;
        if (dmgTaken > 0) {
            health -= dmgTaken;
        }
        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        //placeholder for giving player experience, gold, etc.
        tileManager.GetComponent<Generate>().map.destroyEnemy(index);
        gameObject.gameObject.SetActive(false);
        if (isBoss)
        {
            //Re-generate new level? (simple restart of the room does generate a new map, but gets an error immediately freezing the game)
			LevelHolder.level++;
			SceneManager.LoadScene("Main_Play");
        }
    }

    private bool SeekAndDestroy(int enemyRange, int enemyMovement) {
        success = false;
        List<Position> path = Search(tileManager.GetComponent<Generate>().map.getEnemyPosition(index), enemyRange, enemyMovement, new List<Position>());
        if (success) {
            foreach ( Position loc in path) {
                tileManager.GetComponent<Generate>().map.setEnemyPosition(index, loc);
                this.transform.position = new Vector3(loc.x, loc.y, this.transform.position.z);
            }
        }
        return success;
    }

    private void MeleeAttack(PlayerUnit player) {
        player.GetComponent<PlayerController>().TakeDmg(dmg);
    }

    private List<Position> Search(Position location, int enemyRange, int enemyMovement, List<Position> path) {
        if (enemyMovement <= 0) {
            return new List<Position>();
        }
        List<Position> players = tileManager.GetComponent<Generate>().map.getPlayerPositions();
        Position newPos = new Position(location.x + 1, location.y); //UP (all these have the +1/-1 applied to the "wrong" side due to x and y being flipped)
        foreach (Position playerLoc in players) {
            if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
                target = whichUnit(playerLoc);
                success = true;
                path.Add(location);
                return path;
            }
            newPos = new Position(location.x - 1, location.y); //DOWN
            if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
                target = whichUnit(playerLoc);
                success = true;
                path.Add(location);
                return path;
            }
            newPos = new Position(location.x, location.y - 1); //LEFT
            if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
                target = whichUnit(playerLoc);
                success = true;
                path.Add(location);
                return path;
            }
            newPos = new Position(location.x, location.y + 1);//RIGHT
            if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
                target = whichUnit(playerLoc);
                success = true;
                path.Add(location);
                return path;
            }
        }
        newPos = new Position(location.x + 1, location.y); //UP
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        newPos = new Position(location.x - 1, location.y); //DOWN
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        newPos = new Position(location.x, location.y - 1); //LEFT
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        newPos = new Position(location.x, location.y + 1);//RIGHT
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        return new List<Position>();
    }

    private PlayerUnit whichUnit(Position pos) {
        int i = 0;
        foreach (Position posPlayer in tileManager.GetComponent<Generate>().map.getPlayerPositions()) {
            if(pos.x == posPlayer.x && pos.y == posPlayer.y) {
                return unitManager[i];
            }
            i++;
        }
        print("Error, could not find player for enemy attack");
        return null;
    }
}
