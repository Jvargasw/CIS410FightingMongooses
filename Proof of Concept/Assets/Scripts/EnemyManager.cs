using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    //public int maxEnemies = 10;
    public GameObject[] enemies;
    
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void EnemyTurn()
    {
        foreach (GameObject enemy in enemies) {
            if (enemy.GetComponent<EnemyController>().isActiveAndEnabled) {
                enemy.GetComponent<EnemyController>().Attack();
            }
        }
    }
}
