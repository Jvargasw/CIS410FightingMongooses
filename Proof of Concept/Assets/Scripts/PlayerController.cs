using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float playerSpeed = 3.0f;

	public int maxMoveDistance = 3; //the most spaces the player can move in a turn
    public int playerDmg = 10;

    private bool isMoving = true; //represents moving vs attacking

	private Vector3 movePosition;
	private Vector3 startPosition;
	private int spacesMoved = 0;

	void Start()
    {
		StartCoroutine (PlayerTurn ());
	}
	
	
	void Update()
    {

    }

	IEnumerator PlayerTurn() {
		movePosition = transform.position;
		startPosition = transform.position;

		//while it's the player's turn
		while (TurnManager.playerTurn) {

			//horizontal movement
			if (Input.GetButtonDown ("Horizontal")) {
				if (Input.GetAxisRaw ("Horizontal") > 0) {
					AttemptMove (Vector2.right);
				} else {
					AttemptMove (Vector2.left);
				}
				yield return null;
			//vertical movement
			} else if (Input.GetButtonDown ("Vertical")) {
				if (Input.GetAxisRaw ("Vertical") > 0) {
					AttemptMove (Vector2.up);
				} else {
					AttemptMove (Vector2.down);
				}
				yield return null;

			//end turn
			} else if (Input.GetKeyDown (KeyCode.Return)) {
                if (isMoving)
                {
                    isMoving = false;
                }
                else
                {
                    PlayerEndTurn();
                }
				yield return null;
			} else if (Input.GetKeyDown (KeyCode.Backspace)) {
                if (isMoving)
                {
                    spacesMoved = 0;
                    movePosition = startPosition;
                    transform.position = movePosition;
                }
				yield return null;
			}
			yield return null;
		}

		yield break;
	}


	public void AttemptMove(Vector2 moveDirection) {
		int unwalkable = 1 << 9;
        int entities = 1 << 11;
        RaycastHit2D hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y), moveDirection, 1f, unwalkable);
		//there's a wall in the way
		if (hit.rigidbody != null) {
			return;
		}
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), moveDirection, 1f, entities);
        if (hit.rigidbody != null) {
            print(hit.rigidbody.gameObject.tag);
            if (isMoving) {return;}
            MeleeAttack(hit.rigidbody.gameObject);
            PlayerEndTurn();
            return;
        }
        //have we already moved the max number of spaces?
        if ((spacesMoved + 1 <= maxMoveDistance) && isMoving) {
			spacesMoved++;
			movePosition += new Vector3(moveDirection.x, moveDirection.y, 0);
			transform.position = movePosition;
		}
		return;
	}

    public void PlayerEndTurn() {
        isMoving = true;
        TurnManager.playerTurn = false;
        spacesMoved = 0;
        transform.position = movePosition;
    }

    private void MeleeAttack(GameObject enemy) {
        enemy.GetComponent<EnemyController>().TakeDmg(playerDmg);
    }
}
