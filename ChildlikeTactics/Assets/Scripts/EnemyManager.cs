using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    //public int maxEnemies = 10;
    public GameObject[] enemies;
    
    void Start() {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
