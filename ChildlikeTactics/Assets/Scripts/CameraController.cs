using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");        
    }

	void Update()
    {
        // FIXME: There's probably a more elegant way to lock the camera to the player. 
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 12, transform.position.z);
        transform.LookAt(player.transform);
    }
}
