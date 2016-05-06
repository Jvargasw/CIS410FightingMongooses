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

    private int test = 0;

    void Start () {
        if (health == 0) {
            health = 10;
        }
        if(dmg == 0) {
            dmg = 5;
        }
        tileManager = GameObject.FindGameObjectWithTag("TileManager");

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
        tileManager.GetComponent<Generate>().map.destroyEnemy(index);
        gameObject.gameObject.SetActive(false);
    }

    private bool SeekAndDestroy(int enemyRange, int enemyMovement) {
        success = false;
        test = 0;
        List<Position> path = Search(tileManager.GetComponent<Generate>().map.getEnemyPosition(index), enemyRange, enemyMovement, new List<Position>());
        test = 1;
        Position temp = tileManager.GetComponent<Generate>().map.getPlayerPosition();
        if (success) {
            foreach ( Position loc in path) {
                tileManager.GetComponent<Generate>().map.setEnemyPosition(index, loc);
                this.transform.position = new Vector3(loc.y, loc.x, this.transform.position.z); //X and Y swapped cause map.grid is has x and y swapped in it
            }
        }
        return success;
    }

    private void MeleeAttack(GameObject player) {
        player.GetComponent<PlayerController>().TakeDmg(dmg);
    }

    private List<Position> Search(Position location, int enemyRange, int enemyMovement, List<Position> path) {
        if (enemyMovement <= 0) {
            return new List<Position>();
        }
        Position playerLoc = tileManager.GetComponent<Generate>().map.getPlayerPosition();
        Position newPos = new Position(location.x + 1, location.y); //UP (all these have the +1/-1 applied to the "wrong" side due to x and y being flipped)
        Queue searchQ = new Queue();
        if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
            success = true;
            path.Add(location);
            return path;
        }
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        newPos = new Position(location.x - 1, location.y); //DOWN
        if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
            success = true;
            path.Add(location);
            return path;
        }
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        newPos = new Position(location.x, location.y - 1); //LEFT
        if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
            success = true;
            path.Add(location);
            return path;
        }
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        newPos = new Position(location.x, location.y + 1);//RIGHT
        if ((playerLoc.x == newPos.x) && (playerLoc.y == newPos.y)) {
            success = true;
            path.Add(location);
            return path;
        }
        if (tileManager.GetComponent<Generate>().map.canMoveTo(newPos.x, newPos.y)) {
            List<Position> temp = Search(newPos, enemyRange, enemyMovement - 1, path);
            if (success) {
                return temp;
            }
        }
        return new List<Position>();
    }
}
