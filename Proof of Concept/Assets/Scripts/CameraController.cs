using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public GameObject player;

	void Update()
    {
        // FIXME: There's probably a more elegant way to lock the camera to the player. 
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
	}
}
