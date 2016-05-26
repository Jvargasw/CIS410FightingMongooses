using UnityEngine;
using System;

public class PersistentStorage : MonoBehaviour
{
    public static int level;
    public static int playerDamage1 = 0;
    public static int playerHealth1 = 0;
    public static int playerDefense1 = 0;
    public static int playerInitiative1 = 0;
    public static int playerMovement1 = 0;
    public static int playerDamage2 = 0;
    public static int playerHealth2 = 0;
    public static int playerDefense2 = 0;
    public static int playerInitiative2 = 0;
    public static int playerMovement2 = 0;
    public static int seed;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        seed = (int)DateTime.Now.Ticks;
    }
}
