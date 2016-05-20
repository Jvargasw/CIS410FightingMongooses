using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    private GameObject player;

    void Start()
    {
        player = GameObject.Find("ActiveUnitIndicator");    
		transform.position = new Vector3 (player.transform.position.x, player.transform.position.y - 12, transform.position.z);
		transform.LookAt (player.transform);
    }

	void Update()
    {
        // FIXME: There's probably a more elegant way to lock the camera to the player. 
		Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y - 12, transform.position.z);
		transform.position = Vector3.Lerp(transform.position, targetPos, .16f);
		if (Vector3.Distance(transform.position, targetPos) < .04f) {
			transform.position = targetPos;
		}
        //transform.LookAt(player.transform);


    }
}