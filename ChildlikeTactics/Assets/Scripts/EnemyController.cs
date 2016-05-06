using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

    public int index;
    public int health;
    public int dmg;
    public int movement = 5;
    private int range = 1;

    private GameObject tileManager;
    private bool success;
    private Map map;

    void Start () {
        if (health == 0) {
            health = 10;
        }
        if(dmg == 0) {
            dmg = 5;
        }
        map = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Generate>().map;

    }
	
	public void Attack() {
        if (SeekAndDestroy(range,movement)) {
            MeleeAttack(GameObject.FindGameObjectWithTag("Player"));//Change this once we add multiple Players
        }
    }

    public void TakeDmg(int dmgTaken) {
        health -= dmgTaken;
        if(health <= 0) {
            Die();
        }
    }

    public void Die() {
        //placeholder for giving player experience, gold, etc.
        map.destroyEnemy(index);
        gameObject.gameObject.SetActive(false);
    }

    private bool SeekAndDestroy(int enemyRange, int enemyMovement) {
        success = false;
        List<Position> path = Search(map.getEnemyPosition(index), enemyRange, enemyMovement, new List<Position>());
        if (success) {
            print("SUCCESS");
            foreach ( Position loc in path) {
                map.setEnemyPosition(index, loc);
                print(loc.x + " " + loc.y);
                this.transform.position = new Vector3(loc.y, loc.x, this.transform.position.z); //X and Y swapped cause map.grid is has x and y swapped in it
            }
        }
        return success;
    }

    private void MeleeAttack(GameObject player) {
        player.GetComponent<PlayerController>().TakeDmg(dmg);
    }

    private List<Position> Search(Position location, int enemyRange, int enemyMovement, List<Position> path) {
        path.Add(location);
        Position playerLoc = map.getPlayerPosition();
        Position newPos = new Position(location.x, location.y + 1); //UP
        Queue searchQ = new Queue();
        if (playerLoc != newPos) {
            if (enemyMovement > 0) {
                if (map.canMoveTo(newPos.x, newPos.y)) {
                    searchQ.Enqueue(newPos);
                }
            }
        }
        else {
            success = true;
            return path;
        }
        newPos = new Position(location.x, location.y + -1); //DOWN
        if (playerLoc != newPos) {
            if (enemyMovement > 0) {
                if (map.canMoveTo(newPos.x, newPos.y)) {
                    searchQ.Enqueue(newPos);
                }
            }
        }
        else {
            success = true;
            return path;
        }
        newPos = new Position(location.x - 1, location.y); //LEFT
        if (playerLoc != newPos) {
            if (enemyMovement > 0) {
                if (map.canMoveTo(newPos.x, newPos.y)) {
                    searchQ.Enqueue(newPos);
                }
            }
        }
        else {
            success = true;
            return path;
        }
        newPos = new Position(location.x + 1, location.y);//RIGHT
        if (playerLoc != newPos) {
            if (enemyMovement > 0) {
                if (map.canMoveTo(newPos.x, newPos.y)) {
                    searchQ.Enqueue(newPos);
                }
            }
        }
        else {
            success = true;
            return path;
        }
        for (int i = 0; i < searchQ.Count; i++) {
            path = Search((Position)searchQ.Dequeue(), enemyRange, enemyMovement - 1, path);
        }
        return path;
    }
}
