﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

    public int index;
    public int health;
    public int dmg;
    public int movement = 5;
    private int range = 1;

    private GameObject tileManager;
    private List<PlayerUnit> unitManager;
    private bool success;

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

    }
	
	public void Attack() {
        if (SeekAndDestroy(range,movement)) {
            MeleeAttack(target);//Change this once we add multiple Players
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
        List<Position> path = Search(tileManager.GetComponent<Generate>().map.getEnemyPosition(index), enemyRange, enemyMovement, new List<Position>());
        if (success) {
            foreach ( Position loc in path) {
                tileManager.GetComponent<Generate>().map.setEnemyPosition(index, loc);
                this.transform.position = new Vector3(loc.y, loc.x, this.transform.position.z); //X and Y swapped cause map.grid is has x and y swapped in it
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
