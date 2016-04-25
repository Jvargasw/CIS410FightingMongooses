using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float playerSpeed = 3.0f;
    private Vector3 moveTo;

	void Start()
    {
	    
	}
	
	
	void Update()
    {
        // FIXME: There's probably a more elegant way to move the player.
        if(Input.GetKey(KeyCode.A))
        {
            moveTo = new Vector3(transform.position.x - playerSpeed * Time.deltaTime, transform.position.y, 0);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            moveTo = new Vector3(transform.position.x + playerSpeed * Time.deltaTime, transform.position.y, 0);
        }
        else if(Input.GetKey(KeyCode.W))
        {
            moveTo = new Vector3(transform.position.x, transform.position.y + playerSpeed * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            moveTo = new Vector3(transform.position.x, transform.position.y - playerSpeed * Time.deltaTime);
        }

        transform.position = moveTo;
    }
}
