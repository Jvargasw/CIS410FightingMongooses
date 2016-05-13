using UnityEngine;
using System.Collections;

public class FollowActiveUnit : MonoBehaviour {

	PlayerUnit unit;

	void Start () {
		unit = GameObject.Find ("GameManager").GetComponent<PlayerUnitManager> ().activeUnit;
		StartCoroutine (WaitForUnit ());
	}

	//waits for the active unit to be set, then starts the FollowUnit routine
	IEnumerator WaitForUnit(){
		while (unit == null) {
			unit = GameObject.Find ("GameManager").GetComponent<PlayerUnitManager> ().activeUnit;
			yield return null;
		}
		StartCoroutine (FollowUnit ());
	}

	//sets this object's position to the active unit's
	IEnumerator FollowUnit(){
		while (unit != null) {
			transform.position = unit.transform.position;
			yield return null;
		}
	}

	//updates the active unit to be followed. To be called from PlayerUnitManager
	public void UpdateActiveUnit(PlayerUnit newUnit){
		unit = newUnit;
	}

}