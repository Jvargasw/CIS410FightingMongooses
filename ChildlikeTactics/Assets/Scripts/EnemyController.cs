using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int health;
    public int dmg;
    public int movement = 5;
    private int range = 1;

    private GameObject target;

    void Start () {
        if (health == 0) {
            health = 10;
        }
        if(dmg == 0) {
            dmg = 5;
        }
	}
	
	public void Attack() {
        if (SeekAndDestroy(range,movement)) {
            MeleeAttack(target);
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
        gameObject.gameObject.SetActive(false);
    }

    private bool SeekAndDestroy(int enemyRange, int enemyMovement) {
        target = null;
        Search(new Vector2(transform.position.x, transform.position.y), enemyRange, enemyMovement);
        return !(target == null);
    }

    private void MeleeAttack(GameObject player) {
        player.GetComponent<PlayerController>().TakeDmg(dmg);
    }

    private Vector2 Search(Vector2 location, int enemyRange, int enemyMovement) {
        int playerLayer = 1 << 12;
        Vector2 direction = Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(location, direction, 1f, playerLayer);
        if (hit.rigidbody == null) {
            if (enemyMovement > 0) {
                Search(location + direction, enemyRange, enemyMovement - 1);
            }
            direction = Vector2.down;
            hit = Physics2D.Raycast(location, direction, 1f, playerLayer);
            if (hit.rigidbody == null) {
                if (enemyMovement > 0) {
                    Search(location + direction, enemyRange, enemyMovement - 1);
                }
                direction = Vector2.left;
                hit = Physics2D.Raycast(location, direction, 1f, playerLayer);
                if (hit.rigidbody == null) {
                    if (enemyMovement > 0) {
                        Search(location + direction, enemyRange, enemyMovement - 1);
                    }
                    direction = Vector2.right;
                    hit = Physics2D.Raycast(location, direction, 1f, playerLayer);
                    if (hit.rigidbody == null) {
                        if (enemyMovement > 0) {
                            Search(location + direction, enemyRange, enemyMovement - 1);
                        }
                    }
                    else {
                        transform.position = new Vector3(location.x, location.y, transform.position.z);
                        target = hit.rigidbody.gameObject;
                    }
                }
                else {
                    transform.position = new Vector3(location.x, location.y, transform.position.z);
                    target = hit.rigidbody.gameObject;
                }
            }
            else {
                transform.position = new Vector3(location.x, location.y, transform.position.z);
                target = hit.rigidbody.gameObject;
            }
        }
        else {
            transform.position = new Vector3 (location.x, location.y, transform.position.z);
            target = hit.rigidbody.gameObject;
        }
        return direction;
    }
}
