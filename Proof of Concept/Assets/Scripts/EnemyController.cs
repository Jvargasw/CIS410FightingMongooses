using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int health;
    public int dmg;


    void Start () {
        if (health == 0)
        {
            health = 10;
        }
        if(dmg == 0)
        {
            dmg = 5;
        }
	}
	
	public void Attack() {
        int playerLayer = 1 << 12;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, 1f, playerLayer);
        if (hit.rigidbody == null) {
            hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, 1f, playerLayer);
        }
        if (hit.rigidbody == null) {
            hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.left, 1f, playerLayer);
        }
        if (hit.rigidbody == null) {
            hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right, 1f, playerLayer);
        }
        if(hit.rigidbody != null) {
            print(hit.rigidbody.gameObject.tag);
            MeleeAttack(hit.rigidbody.gameObject);
        }
    }

    public void TakeDmg(int dmgTaken) {
        health -= dmgTaken;
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die() {
        //placeholder for giving player experience, gold, etc.
        gameObject.gameObject.SetActive(false);
    }

    private void MeleeAttack(GameObject player) {
        player.GetComponent<PlayerController>().TakeDmg(dmg);
    }
}
