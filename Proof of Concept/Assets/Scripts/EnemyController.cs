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
	
	public void Attack()
    {

    }

    public void TakeDmg(int dmgTaken) {
        health -= dmgTaken;
        if(health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        //placeholder for giving player experience, gold, etc.
        gameObject.gameObject.SetActive(false);
    }
}
