using UnityEngine;
using System;

public class PersistentStorage : MonoBehaviour
{
    public static int level;
    public static int playerDamage;
    public static int playerHealth;
    public static int playerDefense;
    public static int seed;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        seed = (int)DateTime.Now.Ticks;
    }
}
