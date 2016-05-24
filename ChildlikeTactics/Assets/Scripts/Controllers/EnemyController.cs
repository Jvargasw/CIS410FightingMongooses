using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour {

	public GameObject exitTile;

    public int index;
    public int maxHP;
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
	//private Map map;
    private bool success;
	public bool isDead;
    public Room room;

	private TileType[,] grid;
	private bool[,] tiles; //array of bools for whether or not each tile is walkable

    private List<PlayerUnit> unitManager;

    private PlayerUnit target;

	private MeshRenderer meshRenderer;

    void Start () {
        if (maxHP == 0) {
            maxHP = 10;
        }
        if(dmg == 0) {
            dmg = 5;
        }
        if(LevelHolder.level != 0) {
            maxHP = maxHP * (LevelHolder.level * 2);
            dmg = dmg * (LevelHolder.level * 2);
        }
        health = maxHP;
        tileManager = GameObject.FindGameObjectWithTag("TileManager");
        unitManager = GameObject.Find("GameManager").GetComponent<PlayerUnitManager>().units;
        turnManager = GameObject.Find("GameManager").GetComponent<TurnManager>();
        turnManager.combatants.Add(this.gameObject);

        //map = tileManager.GetComponent<Generate> ().map;

		meshRenderer = GetComponent<MeshRenderer> ();

    }

	//stub of an A* function to be added later
	private void FindPath (Vector3 start, Vector3 end) {
		List<Vector3> openSet = new List<Vector3>();
		//HashSet<Vector3> closedSet = new HashSet<Vector3> ();
		openSet.Add (start);
		openSet.Add (end);

		while (openSet.Count > 0) {

		}
	}
	
	public void Attack() {
        if (SeekAndDestroy(range,movement)) {
            MeleeAttack(target);
        }
    }

    public bool TakeDmg(int playerDmg) {
        int dmgTaken = playerDmg - def;
        if (dmgTaken > 0) {
            health -= dmgTaken;
        }
        if (health <= 0) {
            Die();
			return true;
        }
		return false;
    }

    public void Die() {
        //placeholder for giving player experience, gold, etc.
		isDead = true;
        if (isBoss)
        {
			LevelHolder.level++;
			Instantiate (exitTile, transform.position - new Vector3(0, 0, .3f), Quaternion.identity);
        }
		StartCoroutine (fadeOutAndDeactivate ());
    }

	IEnumerator fadeOutAndDeactivate() {
		while (meshRenderer.material.color.a > 0) {
			Color color = meshRenderer.material.color;
			color.a -= 0.1f;
			meshRenderer.material.color = color;		
			yield return null;
		}
		tileManager.GetComponent<Generate>().map.destroyEnemy(index);
		gameObject.SetActive(false);
		yield break;
	}

    private bool SeekAndDestroy(int enemyRange, int enemyMovement) {
        success = false;
        Stack<Position> path = Search(tileManager.GetComponent<Generate>().map.getEnemyPosition(index));
        turnManager.doneMoving = false;
        StartCoroutine(Move(path));
        return success;
    }

    private void MeleeAttack(PlayerUnit player) {
		if (!isDead)
       		player.GetComponent<PlayerController>().TakeDmg(dmg);
    }

    IEnumerator Move(Stack<Position> path) {
        int distance = path.Count;
        if (distance <= movement + range) {
            success = true;
        }
        else {
            distance = movement;
        }
        while (distance >= 0) {
            distance--;
            try {
                Position loc = path.Pop();
                tileManager.GetComponent<Generate>().map.setEnemyPosition(index, loc);
                this.transform.position = new Vector3(loc.x, loc.y, this.transform.position.z);
            }
            catch {
            }
            yield return new WaitForSeconds(0.1f);
        }
        turnManager.doneMoving = true;
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


    private Stack<Position> Search(Position location) {
        Position[,] p = new Position[64,64];
        List<Position> players = tileManager.GetComponent<Generate>().map.getPlayerPositions();
        Position newPos = location;
        Stack<Position> stack = new Stack<Position>();
        Queue<Position> q = new Queue<Position>();

        q.Enqueue(newPos);
        bool stay = true;
        while (stay) {
            //print("x: " + newPos.x + ", y: " + newPos.y);
            try {
                newPos = q.Dequeue();
            }
            catch {
                break;
            }
            foreach (Position playerLoc in players) {
                if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
                    print("Player x: " + newPos.x + ", y: " + newPos.y);
                    target = whichUnit(playerLoc);
                    while (stay) {
                        if(newPos.x == location.x && location.y == newPos.y) {
                            stay = false;
                        }
                        newPos = p[newPos.x, newPos.y];
                        if (newPos != null) {
                            stack.Push(newPos);
                        }
                        print("Enemy x: " + location.x + ", y: " + location.y);
                    }
                    break;
                }
            }
            if (stay) {
                if ((tileManager.GetComponent<Generate>().map.getTileAt(newPos.x + 1, newPos.y) == TileType.WALKABLE || tileManager.GetComponent<Generate>().map.getTileAt(newPos.x + 1, newPos.y) == TileType.PLAYER) && p[newPos.x + 1, newPos.y] == null) { //UP
                    q.Enqueue(new Position(newPos.x + 1, newPos.y));
                    p[newPos.x + 1, newPos.y] = newPos;
                }
                if ((tileManager.GetComponent<Generate>().map.getTileAt(newPos.x - 1, newPos.y) == TileType.WALKABLE || tileManager.GetComponent<Generate>().map.getTileAt(newPos.x - 1, newPos.y) == TileType.PLAYER) && p[newPos.x - 1, newPos.y] == null) { //DOWN
                    q.Enqueue(new Position(newPos.x - 1, newPos.y));
                    p[newPos.x - 1, newPos.y] = newPos;
                }
                if ((tileManager.GetComponent<Generate>().map.getTileAt(newPos.x, newPos.y + 1) == TileType.WALKABLE || tileManager.GetComponent<Generate>().map.getTileAt(newPos.x, newPos.y + 1) == TileType.PLAYER) && p[newPos.x, newPos.y + 1] == null) { //RIGHT
                    q.Enqueue(new Position(newPos.x, newPos.y + 1));
                    p[newPos.x, newPos.y + 1] = newPos;
                }
                if ((tileManager.GetComponent<Generate>().map.getTileAt(newPos.x, newPos.y - 1) == TileType.WALKABLE || tileManager.GetComponent<Generate>().map.getTileAt(newPos.x, newPos.y - 1) == TileType.PLAYER) && p[newPos.x, newPos.y - 1] == null) { //LEFT
                    q.Enqueue(new Position(newPos.x, newPos.y - 1));
                    p[newPos.x, newPos.y - 1] = newPos;
                }
            }
        }
        return stack;
    }
}
